using Storm.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using Storm.Filters;

namespace Storm.Execution
{
    public abstract class Command
    {
        internal String rootEntity;
        internal FromTree from;
        internal Filter where;
        internal SchemaNavigator navigator;
        internal SQLParser.SQLParser parser;

        internal Command(SchemaNavigator navigator, String from)
        {
            this.navigator = navigator;
            this.rootEntity = from;
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

        internal void ParseSQL()
        {
            parser = new SQLParser.SQLParser(this, navigator);
            parser.BuildFrom(from.root);
            parser.BuildWhere(from.root, where);
            InternalParseSQL();
        }

        protected abstract void InternalParseSQL();

    }
}
