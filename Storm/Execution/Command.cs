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
        
        internal Command(SchemaNavigator navigator, String from) : base(navigator, from)
        {
            this.from = new OriginTree()
            {
                navigator = navigator,
                root = new Origin()
                {
                    Alias = "A0",
                    FullPath = new EntityPath(from, ""),
                    Edge = null,
                    Entity = navigator.GetEntity(from),
                    children = new List<Origin>()
                }
            };

            ((BaseCommand)this).CommandLog(LogLevel.Info, "Command", $"{{\"Action\":\"From\", \"Entity\":\"{from}\"}}");
        }

        internal override void ParseSQL()
        {
            SQLWhereParser whereParser = new SQLWhereParser(from, where, navigator, query);
            query = whereParser.Parse();

            SQLFromParser fromParser = new SQLFromParser(from, navigator, query);
            query = fromParser.Parse();

            if (paging.Item1 != -1 && paging.Item2 != -1)
            {
                SQLPagingParser pagingParser = new SQLPagingParser(paging, navigator, query);
                query = pagingParser.Parse();
            }

            if (orderBy != null && orderBy.Any())
            {
                SQLOrderByParser orderByParser = new SQLOrderByParser(orderBy, navigator, query);
                query = orderByParser.Parse();
            }

        }

        public virtual C With(String requestPath)
        {
            from.Resolve(requestPath);
            ((BaseCommand)this).CommandLog(LogLevel.Info, "Command", $"{{\"Action\":\"With\", \"Entity\":\"{requestPath}\"}}");
            return (C)(BaseCommand)this;
        }

        public virtual C Where(Func<FilterContext, Filter> where)
        {
            if (this.where == null)
                this.where = where(new FilterContext());
            else
                this.where = this.where * where(new FilterContext());

            return (C)(BaseCommand)this;
        }

        public virtual C OrderBy(String requestPath, bool asc = true)
        {
            var _requestPath = new EntityPath(from.root.Entity.ID, requestPath).Path;
            var p = SelectCommandHelper.ValidatePath(_requestPath);

            (string[], string) item;

            if (p.Count() > 1)
                throw new ArgumentException("Only one field should be used in OrderBy. If you need more field in you're OrderBy call this method multiple times.");
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

            ((BaseCommand)this).CommandLog(LogLevel.Info, "Command", $"{{\"Action\":\"OrderBy\", \"Entity\":\"{requestPath}\", \"Direction\":\"{(asc ? "ASC" : "DESC")}\"}}");

            return (C)(BaseCommand)this;
        }

        public virtual C ForPage(int page, int pageSize)
        {
            if (page < 1)
                throw new ArgumentException("First page is 1. 0 or less value not allowed.");
            if (pageSize < 0)
                throw new ArgumentException("Page size cannot be less than 0.");

            this.paging = (page, pageSize);
            ((BaseCommand)this).CommandLog(LogLevel.Info, "Command", $"{{\"Action\":\"Paging\", \"Page\":\"{page}\", \"PageSize\":\"{pageSize}\"}}");
            return (C)(BaseCommand)this;
        }

    }
}
