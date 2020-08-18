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
        protected List<TableTree> requests = new List<TableTree>();
        public GetCommand(SchemaNavigator navigator, String from)
        {
            this.navigator = navigator;
            this.from = from;
            this.fromTree = new TableTree()
            {
                Alias = "A0",
                FullPath = from,
                Edge = null,
                Entity = navigator.GetEntity(from),
                children = new List<TableTree>()
            };
            this.nodes = new Dictionary<string, TableTree>() { { fromTree.FullPath, fromTree } };
            requests.Add(fromTree);
        }

        public GetCommand With(String requestPath)
        {
            var path = requestPath.Split('.');
            path = path[0] == from ? path : (new string[] { from }).Concat(path).ToArray();
            var x = Resolve(fromTree, 0, path);
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
                .Select(x => $"{x.ef.DBName} AS {x.Alias}${x.ef.CodeName}")
                .ToArray();

            parser.ctx.query.Select(selectStatement);
        }
    }
}
