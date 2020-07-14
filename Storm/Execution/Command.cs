using Storm.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using SqlKata;

namespace Storm.Execution
{
    public class Command
    {
        protected Query CommandQuery;
        protected String from;
        protected SchemaNavigator navigator;
        protected TableTree fromTree;
        protected Dictionary<string, TableTree> nodes;

        protected virtual Query ParseSQL()
        {
            Query parseTree(TableTree parentTree, TableTree subTree, Query query)
            {
                if (subTree.Edge.OnExpression is null)
                {
                    String sourceCol = resolveSourceColumnName(subTree);
                    String targetCol = resolveTargetColumnName(subTree);
                    query.LeftJoin($"{subTree.Entity.DBName} as {subTree.Alias}", $"{parentTree.Alias}.{sourceCol}", $"{subTree.Alias}.{targetCol}");
                }
                else
                {

                }
                return subTree.children.Aggregate(query, (q, n) => parseTree(subTree, n, q));
            }

            var _q = new Query($"{fromTree.Entity.DBName} as {fromTree.Alias}");
            _q = fromTree.children.Aggregate(_q, (q, n) => parseTree(fromTree, n, q));
            return _q;
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

        private string resolveTargetColumnName(TableTree subTree)
        {
            string targetCol = string.Empty;
            var target = navigator.GetEntity(subTree.Edge.TargetID);
            if (target.entityFields != null && target.entityFields.Any())
            {
                var field = target
                    .entityFields
                    .FirstOrDefault(ef => ef.CodeName == subTree.Edge.On.Item2);
                if (field != null)
                    targetCol = field.DBName;
                else
                    throw new ArgumentException($"Error on parsing join condition for table {subTree.Entity.DBName}: no field found on entity {subTree.Edge.TargetID} with name {subTree.Edge.On.Item2}.");
            }
            else
            {
                targetCol = subTree.Edge.On.Item2;
            }

            return targetCol;
        }

        private string resolveSourceColumnName(TableTree subTree)
        {
            string sourceCol = string.Empty;
            var source = navigator.GetEntity(subTree.Edge.SourceID);
            if (source.entityFields != null && source.entityFields.Any())
            {
                var field = source
                    .entityFields
                    .FirstOrDefault(ef => ef.CodeName == subTree.Edge.On.Item1);
                if (field != null)
                    sourceCol = field.DBName;
                else
                    throw new ArgumentException($"Error on parsing join condition for table {subTree.Entity.DBName}: no field found on entity {subTree.Edge.SourceID} with name {subTree.Edge.On.Item1}.");
            }
            else
            {
                sourceCol = subTree.Edge.On.Item1;
            }

            return sourceCol;
        }
    }
}
