using SqlKata;
using Storm.Execution;
using Storm.Filters;
using Storm.Helpers;
using Storm.Schema;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Storm.Execution
{
    public class GetCommand : Command<GetCommand>
    {
        internal List<FromNode> requests = new List<FromNode>();

        public GetCommand(SchemaNavigator navigator, String from) : base(navigator, from)
        {
            requests.Add(base.from.root);
        }

        public override GetCommand With(string requestPath)
        {
            var x = from.Resolve(requestPath);
            requests.Add(x);
            return this;
        }

        internal override void ParseSQL()
        {
            base.ParseSQL();
            var selectStatement = requests
                .SelectMany(r => r.Entity.entityFields.Select(ef => (r.Alias, ef)))
                .Select(x => $"{x.Alias}.{x.ef.DBName} AS {x.Alias}${x.ef.CodeName}")
                .ToArray();

            query.Select(selectStatement);

        }

        internal override object Read(IDataReader dataReader)
        { 
            StormDataSet sr = new StormDataSet(this.rootEntity);

            var metadata = this.requests
                .SelectMany(r => {
                    return r.Entity.entityFields
                        .Select(f => {
                            return new SelectNode()
                            {
                                FromNode = r,
                                EntityField = f,
                                FullPath = new FieldPath(r.FullPath.Root, r.FullPath.Path, f.CodeName),
                            };
                        });
                });

            sr.ReadData(dataReader, metadata);

            return GetCommandHelpers.ToResults(sr, this.navigator, requests, from);
        }

        public new IEnumerable<dynamic> Execute()
        {
            return (IEnumerable<dynamic>)base.Execute();
        }
    }
}
