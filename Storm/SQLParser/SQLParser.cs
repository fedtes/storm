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
        protected SchemaNavigator navigator;
        protected Query query;

        public SQLParser(SchemaNavigator navigator, Query query)
        {
            this.navigator = navigator;
            this.query = query;
        }

        public abstract Query Parse();
    }
}
