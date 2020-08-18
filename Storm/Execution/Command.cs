using Storm.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using Storm.Filters;

namespace Storm.Execution
{
    public abstract class Command
    {
        internal String from;
        internal TableTree fromTree;
        internal Filter where;
        internal SchemaNavigator navigator;
        internal Dictionary<string, TableTree> nodes;
        internal SQLParser.SQLParser parser;


        protected void ParseSQL()
        {
            parser = new SQLParser.SQLParser(this, navigator);
            parser.BuildFrom(fromTree);
            parser.BuildWhere(fromTree, where);
            InternalParseSQL();
        }

        protected abstract void InternalParseSQL();

        protected TableTree Resolve(TableTree subTree, int idx, IEnumerable<string> path)
        {
            var head = path.Take(idx);
            var current = path.ElementAt(idx);
            var tail = path.Skip(idx + 1);
            var partialPath = String.Join(".", head.Concat(new string[] { current }));

            if (!nodes.ContainsKey(partialPath))
            {
                var _edge = navigator.GetEdge($"{subTree.Entity.ID}.{current}");
                var node = new TableTree()
                {
                    Alias = $"A{nodes.Count}",
                    children = new List<TableTree>(),
                    Edge = _edge,
                    Entity = navigator.GetEntity(_edge.TargetID)
                };
                nodes.Add(partialPath, node);
                subTree.children.Add(node);
            }

            if (tail.Count() == 0)
                return nodes[partialPath];
            else
                return Resolve(nodes[partialPath], idx + 1, path);
        }

        
    }
}
