using SqlKata;
using Storm.Execution;
using Storm.Filters;
using Storm.Schema;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Storm.SQLParser
{
    abstract class SQLParser
    {
        protected Context ctx;
        protected Query query;

        public SQLParser(Context ctx, Query query)
        {
            this.ctx = ctx;
            this.query = query;
        }

        public abstract Query Parse();
    }
}
