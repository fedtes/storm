using System.Collections.Generic;
using System.Data;
using System;
using Storm.Helpers;
using Storm.Filters;
using Storm.Linq;
using Storm.Schema;
using System.Linq;

namespace Storm.Execution
{

    /// <summary>
    /// Command used to read data from database.
    /// </summary>
    /// <remarks>
    /// This should be used instead of GetCommand and SelectCommand
    /// </remarks>
    public class ReadCommand : BaseCommand
    {

        internal List<SelectNode> select = new List<SelectNode>();
        internal TableTree from;
        internal Filter where;
        internal List<(SelectNode, bool)> orderBy = null;
        internal (int, int) paging = (-1, -1);

        public ReadCommand(Context ctx, string from) : base(ctx, from)
        {
            this.from = new TableTree(ctx, from);
        }

        internal override void ParseSQL()
        {
            throw new System.NotImplementedException();
        }

        internal override object Read(IDataReader dataReader)
        {
            throw new System.NotImplementedException();
        }


        #region "Public Methods" 

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
        public virtual ReadCommand ForPage(int page, int pageSize)
        {
            if (page < 1)
                throw new ArgumentException("First page is 1. 0 or less value not allowed.");
            if (pageSize < 0)
                throw new ArgumentException("Page size cannot be less than 0.");

            this.paging = (page, pageSize);
            ((BaseCommand)this).CommandLog(LogLevel.Trace, "Command", $"{{\"Action\":\"Paging\", \"Page\":\"{page}\", \"PageSize\":\"{pageSize}\"}}");
            return this;
        }

        /// <summary>
        /// Include additional entities in the command. If a Get command is used, each entity is extracted.
        /// </summary>
        /// <remarks>
        /// There must be a valid connection between the Root entity and the one requested in the schema.
        /// </remarks>
        /// <param name="requestPath"></param>
        /// <returns></returns>
        public virtual ReadCommand With(String requestPath)
        {
            from.Resolve(requestPath);
            ((ReadCommand)this).CommandLog(LogLevel.Trace, "Command", $"{{\"Action\":\"With\", \"Entity\":\"{requestPath}\"}}");
            return (ReadCommand)this;
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
        public virtual ReadCommand Where(Func<FilterContext, Filter> where)
        {
            if (this.where == null)
                this.where = where(new FilterContext());
            else
                this.where = this.where * where(new FilterContext());

            return this;
        }

        /// <summary>
        /// Add an order to your command. You can order by multiple field by calling this method many times.
        /// </summary>
        /// <param name="requestPath"></param>
        /// <param name="asc"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public virtual ReadCommand OrderBy(String requestPath, bool asc = true)
        {
            var _requestPath = new EntityPath(from.Root.Entity.Id, requestPath).Path;
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

            // var f = SelectCommandHelper.GenerateSingleSelectNode(item, from);

            // if (this.orderBy == null)
            //     this.orderBy = new List<(SelectNode, bool)>();

            // this.orderBy.Add((f, asc));

            // ((BaseCommand)this).CommandLog(LogLevel.Trace, "Command", $"{{\"Action\":\"OrderBy\", \"Entity\":\"{requestPath}\", \"Direction\":\"{(asc ? "ASC" : "DESC")}\"}}");

            return this;
        }



        #endregion

    }



    internal class TableTree
    {
        private readonly Context ctx;
        private string from;
        private Table root;
        private int aliasIndex = 0;

        public Table Root => root;

        public TableTree(Context ctx, string from)
        {
            this.ctx = ctx;
            this.from = from;
            this.root = CreateTable(ctx.Navigator.GetEntity(from), null);
        }

        internal void Resolve(string path)
        {
            var p = new Path(path);
            p = EnsurePath(p);
            var fullExpandedPath = ctx.Navigator.GetFullPath(p);
            Resolve(root, fullExpandedPath);
        }

        internal void Resolve(Table table, IEnumerable<AbstractSchemaItem> path)
        {
            if (!path.Any()) return;

            switch (path.First())
            {
                case Schema.Entity e:
                    if (table.Entity == e)
                    {
                        Resolve(table, path.Skip(1));
                    }
                    else
                    {
                        throw new ApplicationException("Should not pass here!!");
                    }
                    break;
                case NavigationProperty np:

                    if (table.Entity == ctx.Navigator.GetEntity(np.OwnerEntityId))
                    {

                        if (table.Joins.Any(x => x.LookupProperty == np))
                        {
                            Resolve(table.Joins.First(x => x.LookupProperty == np), path.Skip(1));
                        }
                        else
                        {
                            Table t = CreateTable(ctx.Navigator.GetEntity(np.TargetEntity), np);
                            table.Joins.Add(t);
                            Resolve(t, path.Skip(1));
                        }
                    }

                    break;
                case SimpleProperty sp:
                    return;
                default:
                    throw new ApplicationException("Should not pass here!!");
            }

        }

        private Table CreateTable(Schema.Entity entity, NavigationProperty navigationProperty)
        {
            var t = new Table()
            {
                Alias = $"A{aliasIndex}",
                Entity = entity,
                LookupProperty = navigationProperty,
                Joins = new List<Table>()
            };
            aliasIndex++;
            return t;
        }

        private Path EnsurePath(Path p)
        {
            if (p.First() != this.root.Entity.Id) {
                return new Path(this.root.Entity.Id) + p;
            } else {
                return p;
            }
        }

        internal class Table
        {
            /// <summary>
            /// Alias used in the sql query
            /// </summary>
            public string Alias;
            /// <summary>
            /// Entity referenced by this table. If not root, Entity should equals to LookupProperty.TargetEntity
            /// </summary>
            public Schema.Entity Entity;
            /// <summary>
            /// Lookup property that takes to this table. Null if this table is root. 
            /// </summary>
            public NavigationProperty LookupProperty;
            /// <summary>
            /// All table in join with this throught NavigationProperties of this entity.
            /// </summary>
            public List<Table> Joins;

            public override string ToString()
            {
                var _join = LookupProperty != null ? $" on {LookupProperty.Id}" : "";
                return $"{Entity.Id} as {Alias}" + _join;
            }

        }

    }





}