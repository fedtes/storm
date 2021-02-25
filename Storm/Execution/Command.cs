using Storm.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using Storm.Filters;
using SqlKata;
using Storm.SQLParser;

namespace Storm.Execution
{
    public abstract class Command<C> : BaseCommand where C : BaseCommand
    {    
        internal Filter where;
        internal FromTree from;
        
        internal Command(SchemaNavigator navigator, String from) : base(navigator, from)
        {
            this.from = new FromTree()
            {
                navigator = navigator,
                root = new FromNode()
                {
                    Alias = "A0",
                    FullPath = new EntityPath(from, ""),
                    Edge = null,
                    Entity = navigator.GetEntity(from),
                    children = new List<FromNode>()
                }
            };
        }

        internal override void ParseSQL()
        {
            SQLWhereParser whereParser = new SQLWhereParser(from, where, navigator, query);
            query = whereParser.Parse();

            SQLFromParser fromParser = new SQLFromParser(from, navigator, query);
            query = fromParser.Parse();
        }

        public virtual C With(String requestPath)
        {
            from.Resolve(requestPath);
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



    }
}
