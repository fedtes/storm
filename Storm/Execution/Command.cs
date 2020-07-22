using Storm.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using Storm.Filters;

namespace Storm.Execution
{
    public class Command
    {
        internal String from;
        internal SchemaNavigator navigator;
        internal TableTree fromTree;
        internal Dictionary<string, TableTree> nodes;
        internal Filter where;
        private SQLParser.SQLParser parser = new SQLParser.SQLParser();


        protected virtual void ParseSQL()
        {
            parser.BuildFrom(fromTree);
            parser.BuildWhere(fromTree, where);
        }

        protected TableTree Resolve(TableTree subTree, int idx, IEnumerable<string> path)
        {
            var head = path.Take(idx);
            var current = path.ElementAt(idx);
            var tail = path.Skip(idx);
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
