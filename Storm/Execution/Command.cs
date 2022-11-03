using Storm.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using Storm.Filters;
using Storm.SQLParser;
using Storm.Helpers;
using Storm.Origins;

namespace Storm.Execution
{
    public abstract class Command<C> : BaseCommand where C : BaseCommand
    {    
        internal Filter where;
        internal OriginTree from;
        internal List<(SelectNode, bool)> orderBy = null;
        internal (int,int) paging = (-1, -1);
        
        internal Command(Context ctx, String from) : base(ctx, from)
        {
            this.from = new OriginTree()
            {
                ctx = ctx,
                root = new Origin()
                {
                    Alias = "A0",
                    FullPath = new EntityPath(from, ""),
                    Edge = null,
                    Entity = ctx.Navigator.GetEntity(from),
                    children = new List<Origin>()
                }
            };

            ((BaseCommand)this).CommandLog(LogLevel.Trace, "Command", $"{{\"Action\":\"From\", \"Entity\":\"{from}\"}}");
        }

        internal override void ParseSQL()
        {
            SQLWhereParser whereParser = new SQLWhereParser(from, where, ctx, query);
            query = whereParser.Parse();

            SQLFromParser fromParser = new SQLFromParser(from, ctx, query);
            query = fromParser.Parse();

            if (paging.Item1 != -1 && paging.Item2 != -1)
            {
                SQLPagingParser pagingParser = new SQLPagingParser(paging, ctx, query);
                query = pagingParser.Parse();
            }

            if (orderBy != null && orderBy.Any())
            {
                SQLOrderByParser orderByParser = new SQLOrderByParser(orderBy, ctx, query);
                query = orderByParser.Parse();
            }

        }

        /// <summary>
        /// Include additional entities in the command. If a Get command is used, each entity is extracted.
        /// </summary>
        /// <remarks>
        /// There must be a valid connection between the Root entity and the one requested in the schema.
        /// </remarks>
        /// <param name="requestPath"></param>
        /// <returns></returns>
        public virtual C With(String requestPath)
        {
            from.Resolve(requestPath);
            ((BaseCommand)this).CommandLog(LogLevel.Trace, "Command", $"{{\"Action\":\"With\", \"Entity\":\"{requestPath}\"}}");
            return (C)(BaseCommand)this;
        }

        /// <summary>
        /// Define a filter that will be applied to the command. Calling this method more times put the various filters in And relationship.
        /// </summary>
        /// <remarks>
        /// Filter is described by a lambda such as 
        /// <code>x => x["my.path.to.field"].EqualTo.Val("abc")</code>
        /// <para>
        /// Use '*' ("And"), '+' ("Or"), () ("Brackets") operator to describe boolean expressions.
        /// </para>
        /// </remarks>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual C Where(Func<FilterContext, Filter> where)
        {
            if (this.where == null)
                this.where = where(new FilterContext());
            else
                this.where = this.where * where(new FilterContext());

            return (C)(BaseCommand)this;
        }

        /// <summary>
        /// Add an order to your command. You can order by multiple field by calling this method many times.
        /// </summary>
        /// <param name="requestPath"></param>
        /// <param name="asc"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public virtual C OrderBy(String requestPath, bool asc = true)
        {
            var _requestPath = new EntityPath(from.root.Entity.ID, requestPath).Path;
            var p = SelectCommandHelper.ValidatePath(_requestPath);

            (string[], string) item;

            if (p.Count() > 1)
                throw new ArgumentException("Only one field should be used in OrderBy. If you need more field in your OrderBy then call this method multiple times.");
            else if (p.Count() == 0)
                throw new ArgumentException("At least one field should be used in OrderBy");
            else
                item = p.First();

            if (item.Item2 == "*")
                throw new ArgumentException("* is not allowed in OrderBy.");

            var f = SelectCommandHelper.GenerateSingleSelectNode(item, from);

            if (this.orderBy == null)
                this.orderBy = new List<(SelectNode, bool)>();

            this.orderBy.Add((f, asc));

            ((BaseCommand)this).CommandLog(LogLevel.Trace, "Command", $"{{\"Action\":\"OrderBy\", \"Entity\":\"{requestPath}\", \"Direction\":\"{(asc ? "ASC" : "DESC")}\"}}");

            return (C)(BaseCommand)this;
        }

        /// <summary>
        /// Slice the result in page. <paramref name="page"/> is a 1 index base page number where each page size is basede on <paramref name="pageSize"/>. 
        /// </summary>
        /// <remarks>
        /// If no OrderBy is specified for the command a default order is icluded as root Entity primary key order by asc
        /// </remarks>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public virtual C ForPage(int page, int pageSize)
        {
            if (page < 1)
                throw new ArgumentException("First page is 1. 0 or less value not allowed.");
            if (pageSize < 0)
                throw new ArgumentException("Page size cannot be less than 0.");

            this.paging = (page, pageSize);
            ((BaseCommand)this).CommandLog(LogLevel.Trace, "Command", $"{{\"Action\":\"Paging\", \"Page\":\"{page}\", \"PageSize\":\"{pageSize}\"}}");
            return (C)(BaseCommand)this;
        }

    }
}
