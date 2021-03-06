﻿using SqlKata;
using Storm.Filters;
using Storm.Helpers;
using Storm.Origins;
using Storm.Schema;
using Storm.SQLParser;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Storm.Execution
{
    public class DeleteCommand : BaseCommand
    {

        internal Func<FilterContext, Filter> whereLambda;
        internal OriginTree from;

        internal DeleteCommand(SchemaNavigator navigator, String from) : base(navigator, from)
        {
            this.from = new OriginTree()
            {
                navigator = navigator,
                root = new Origin()
                {
                    Alias = "",
                    FullPath = new EntityPath(from, ""),
                    Edge = null,
                    Entity = navigator.GetEntity(from),
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
                    .SubQuery(s => s.From(this.from.root.Entity.ID).Where(whereLambda));

                SQLWhereParser whereParser = new SQLWhereParser(from, _whereDelete(new FilterContext()), navigator, query);
                query = whereParser.Parse();
            }

            query = query.AsDelete();
        }

        internal override object Read(IDataReader dataReader)
        {
            return new StormDeleteResult(dataReader.RecordsAffected);
        }

        public new StormDeleteResult Execute()
        {
            return (StormDeleteResult)base.Execute();
        }
    }
}
