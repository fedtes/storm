using Storm.Helpers;
using Storm.Schema;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Storm.Origins;
using System.Threading.Tasks;

namespace Storm.Execution
{
    public class GetCommand : Command<GetCommand>
    {
        internal List<Origin> requests = new List<Origin>();

        internal GetCommand(Context ctx, String from) : base(ctx, from)
        {
            requests.Add(base.from.root);
        }

        public override GetCommand With(string requestPath)
        {
            var x = from.Resolve(requestPath);
            requests.Add(x);
            ((BaseCommand)this).CommandLog(LogLevel.Info, "GetCommand", $"{{\"Action\":\"With\", \"Entity\":\"{requestPath}\"}}");
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

            return GetCommandHelpers.ToResults(sr, this.ctx, requests, from);
        }

        public async Task<IEnumerable<dynamic>> Execute()
        {
            return (IEnumerable<dynamic>)(await base.InternalExecute<IEnumerable<dynamic>>());
        }
    }
}
