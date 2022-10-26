using System;
using System.Collections.Generic;
using System.Text;
using SqlKata;
using Storm.Schema;

namespace Storm.SQLParser
{
    class SQLPagingParser : SQLParser
    {
        private readonly (int, int) paging;

        public SQLPagingParser((int, int) paging, Context ctx, Query query) : base(ctx, query)
        {
            this.paging = paging;
        }

        public override Query Parse()
        {
            return query.ForPage(paging.Item1, paging.Item2);
        }
    }
}
