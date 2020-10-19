using SqlKata;
using Storm.Execution.Results;
using Storm.Filters;
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
        protected List<FromNode> requests = new List<FromNode>();

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
                            return new ReaderMetadata()
                            {
                                OwnerEntity = r.Entity,
                                EntityField = f,
                                FullPath = $"{r.FullPath}.{f.CodeName}",
                                Alias = r.Alias
                            };
                        });
                });

            sr.ReadData(dataReader, metadata);

            var result = new List<StormResult>();

            var or = sr.ObjectRanges.First();

            foreach (var item in sr.Select((x, i) => new StormRow(sr, i, or.Value.Start, or.Value.End)))
            {
                var r1 = new StormResult(item, or.Key);
                if (!result.Contains(r1))
                {
                    result.Add(r1);
                }
            }

            return sr;
            throw new NotImplementedException();
        }
    }
}
