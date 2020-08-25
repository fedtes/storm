using SqlKata;
using Storm.Filters;
using Storm.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Storm.Execution
{
    public class GetCommand : Command
    {
        protected List<FromNode> requests = new List<FromNode>();
        public GetCommand(SchemaNavigator navigator, String from) : base(navigator, from)
        {
            requests.Add(base.from.root);
        }

        public GetCommand With(String requestPath)
        {
            var path = requestPath.Split('.');
            path = path[0] == rootEntity ? path : (new string[] { rootEntity }).Concat(path).ToArray();
            var x = base.from.Resolve(path);
            requests.Add(x);
            return this;
        }

        public GetCommand Where(Func<Expression, Filter> where)
        {
            this.where = where(new Expression());
            return this;
        }

        protected override void InternalParseSQL()
        {
            var selectStatement = requests
                .SelectMany(r => r.Entity.entityFields.Select(ef => (r.Alias, ef)))
                .Select(x => $"{x.Alias}.{x.ef.DBName} AS {x.Alias}${x.ef.CodeName}")
                .ToArray();

            parser.ctx.query.Select(selectStatement);
        }
    }
}
