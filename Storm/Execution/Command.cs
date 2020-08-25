using Storm.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using Storm.Filters;
using SqlKata;
using Storm.SQLParser;

namespace Storm.Execution
{
    public abstract class BaseCommand
    {
        internal String rootEntity;
        internal SchemaNavigator navigator;
        internal Query query;

        internal BaseCommand(SchemaNavigator navigator, String from)
        {
            this.navigator = navigator;
            this.rootEntity = from;
            this.query = new Query($"{navigator.GetEntity(from).DBName} as A0");
        }
    }

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
                    FullPath = from,
                    Edge = null,
                    Entity = navigator.GetEntity(from),
                    children = new List<FromNode>()
                }
            };
        }

        internal virtual void ParseSQL()
        {
            SQLFromParser fromParser = new SQLFromParser(from, navigator, query);
            query = fromParser.Parse();

            SQLWhereParser whereParser = new SQLWhereParser(from, where, navigator, query);
            query = whereParser.Parse();
        }

        public virtual C With(String requestPath)
        {
            from.Resolve(requestPath);
            return (C)(BaseCommand)this;
        }

        public virtual C Where(Func<Expression, Filter> where)
        {
            this.where = where(new Expression());
            return (C)(BaseCommand)this;
        }



    }
}
