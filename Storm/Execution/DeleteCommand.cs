using SqlKata;
using Storm.Filters;
using Storm.Helpers;
using Storm.Origins;
using Storm.Schema;
using Storm.SQLParser;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Storm.Execution
{
    public class DeleteCommand : BaseCommand
    {

        internal Func<FilterContext, Filter> whereLambda;
        internal OriginTree from;

        internal DeleteCommand(Context ctx, String from) : base(ctx, from)
        {
            this.from = new OriginTree()
            {
                ctx = ctx,
                root = new Origin()
                {
                    Alias = "",
                    FullPath = new EntityPath(from, ""),
                    Edge = null,
                    Entity = ctx.Navigator.GetEntity(from),
                    children = new List<Origin>()
                }
            };

            ((BaseCommand)this).CommandLog(LogLevel.Info, "DeleteCommand", $"{{\"Action\":\"Delete\", \"Entity\":\"{from}\"}}");
        }

        public virtual DeleteCommand Where(Func<FilterContext, Filter> where)
        {
            this.whereLambda = where;

            return this;
        }

        internal override void ParseSQL()
        {
            query = new Query($"{this.from.root.Entity.DBName}");

            if (whereLambda != null)
            {
                Filter _whereDelete(FilterContext ctx) => ctx[this.from.root.Entity.PrimaryKey.CodeName].In
                    .SubQuery(s => s.From(this.from.root.Entity.Id).Where(whereLambda));

                SQLWhereParser whereParser = 
                    new SQLWhereParser(from, _whereDelete(new FilterContext()), ctx, query);
                query = whereParser.Parse();
            }

            query = query.AsDelete();
        }

        internal override object Read(IDataReader dataReader)
        {
            return new StormDeleteResult(dataReader.RecordsAffected);
        }

        public async Task<StormDeleteResult> Execute()
        {
            return (StormDeleteResult)(await base.InternalExecute<StormDeleteResult>());
        }
    }
}
