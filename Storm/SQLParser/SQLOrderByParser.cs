using System;
using System.Collections.Generic;
using System.Text;
using SqlKata;
using Storm.Execution;
using Storm.Schema;

namespace Storm.SQLParser
{
    class SQLOrderByParser : SQLParser
    {
        private readonly List<(SelectNode, bool)> orderBy;

        public SQLOrderByParser(List<(SelectNode, bool)> orderBy, SchemaNavigator navigator, Query query) : base(navigator, query)
        {
            this.orderBy = orderBy;
        }

        public override Query Parse()
        {
            foreach (var item in orderBy)
            {
                if (item.Item2)
                    query.OrderBy($"{item.Item1.Alias}.{item.Item1.DBName}");
                else
                    query.OrderByDesc($"{item.Item1.Alias}.{item.Item1.DBName}");
            }

            return query;
        }
    }
}
