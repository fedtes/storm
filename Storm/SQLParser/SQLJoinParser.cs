using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlKata;
using Storm.Execution;
using Storm.Filters;
using Storm.Schema;
using Storm.Origins;

namespace Storm.SQLParser
{
    class SQLJoinParser : SQLWhereParser
    {
        private readonly Origin sourceNode;
        private readonly Origin targetNode;

        public SQLJoinParser(Origin sourceNode, Origin targetNode, OriginTree fromTree, Filter join, Context ctx, Query query) : base(fromTree, join, ctx, query)
        {
            this.sourceNode = sourceNode;
            this.targetNode = targetNode;
        }

        public override Query Parse()
        {
            base.query = base.query.LeftJoin($"{targetNode.Entity.DBName} as {targetNode.Alias}", j => 
                ParseFilter<Join>(filter, j, Op.And)
            );
            
            return query;
        }

        protected override string ParseReferenceFilterValue(ReferenceFilterValue rfv)
        {
            var ps = rfv.Path.Split('.');

            if (2 != ps.Length)
                throw new WrongSchemaDefinitionException("Only one level navigation allowed in join expression. " + rfv.Path);

            var path = ps.Take(ps.Length - 1);
            var name = ps.Last();

            if ("source"== path.First().ToLowerInvariant())
            {
                var field = sourceNode.Entity.entityFields.FirstOrDefault(ef => ef.CodeName.ToLowerInvariant() == name.ToLowerInvariant());
                return $"{sourceNode.Alias}.{field.DBName}";

            } else if ("target" == path.First().ToLowerInvariant())
            {
                var field = targetNode.Entity.entityFields.FirstOrDefault(ef => ef.CodeName.ToLowerInvariant() == name.ToLowerInvariant());
                return $"{targetNode.Alias}.{field.DBName}";
            }
            else
            {
                throw new WrongSchemaDefinitionException("Error on path definition in join expression. All path should start with 'source' or 'target' instead of " + rfv.Path);
            }  
        }
    }
}
