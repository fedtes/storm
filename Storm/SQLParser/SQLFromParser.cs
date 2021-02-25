using SqlKata;
using Storm.Execution;
using Storm.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Storm.SQLParser
{
    class SQLFromParser: SQLParser
    {
        protected FromTree fromTree;

        public SQLFromParser(FromTree fromTree, SchemaNavigator schemaNavigator, Query query) : base(schemaNavigator, query)
        {
            this.fromTree = fromTree;
        }

        public override Query Parse()
        {
            base.query = fromTree.root.children.Aggregate(query, (q, n) => ParseTableTree(fromTree.root, n, q));
            return query;
        }

        /// <summary>
        /// Parse each join where the left (source) are the parent and the right(target) is a child in the FromTree rappresentation
        /// </summary>
        /// <remarks>
        /// --sourceNode
        ///      |
        ///      +--targetNode
        ///      |
        ///      +--targetNode
        ///      ...
        /// </remarks>
        /// <param name="sourceNode"></param>
        /// <param name="targetNode"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        private Query ParseTableTree(FromNode sourceNode, FromNode targetNode, Query query)
        {
            if (targetNode.Edge.OnExpression is null)
            {
                String sourceCol = resolveSourceColumnName(targetNode);
                String targetCol = resolveTargetColumnName(targetNode);
                query.LeftJoin($"{targetNode.Entity.DBName} as {targetNode.Alias}", $"{sourceNode.Alias}.{sourceCol}", $"{targetNode.Alias}.{targetCol}");
            }
            else
            {
                var joinParser = new SQLJoinParser(sourceNode, targetNode, fromTree, targetNode.Edge.OnExpression, navigator, query);
                this.query = joinParser.Parse();
            }
            return targetNode.children.Aggregate(query, (q, n) => ParseTableTree(targetNode, n, q));
        }

        private string resolveTargetColumnName(FromNode subTree)
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

        private string resolveSourceColumnName(FromNode subTree)
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
