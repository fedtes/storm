using Storm.Execution;
using Storm.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Storm.Origins;

namespace Storm.Helpers
{
    static class GetCommandHelpers
    {

        private class RecCtx
        {
            public int Index;
            public StormDataSet Data;
            public SchemaNode Root;
            public Dictionary<SchemaNode, IndexRange> Ranges;
            public Dictionary<EntityPath, FieldPath> PrimaryKeys;
        }

        static (Object, StormResult) CreateResult(Origin fromNode,RecCtx recCtx)
        {
            var range = recCtx.Ranges[fromNode.Entity];
            var row = new StormRow(recCtx.Data, recCtx.Index, range.Start, range.End);
            var primaryKey = row[recCtx.PrimaryKeys[fromNode.FullPath]];
            return (primaryKey, new StormResult(row, fromNode.Entity));
        }
        
        static void CreateResultRelations(RecCtx ctx, 
                                          List<Origin> requests,
                                          StormResult result)
        {
            if (requests.Any())
            {
                var r1 = requests.First();
                var (pk, res) = CreateResult(r1, ctx);

                if (!result.Relations.ContainsKey(r1.FullPath.Path))
                    result.Relations.Add(r1.FullPath.Path, new List<StormResult>());

                if (!result.Relations[r1.FullPath.Path].Any(x => x.PrimaryKey == pk))
                    ((List<StormResult>)result.Relations[r1.FullPath.Path]).Add(res);

                CreateResultRelations(ctx, requests.Skip(1).ToList(), result);
            }
        }

        public static IList<StormResult> ToResults(StormDataSet data, 
                                            Context ctx, 
                                            List<Origin> requests,
                                            OriginTree fromTree)
        {
            var root = ctx.Navigator.GetEntity(data.root);
            var ranges = data.ObjectRanges;
            var primaryKeys = data.IdentityIndexes;
            var items = new Dictionary<EntityPath, Dictionary<object, StormResult>>();
            var results = new List<StormResult>();

            for (int i = 0; i < data.Count(); i++)
            {
                var recCtx = new RecCtx()
                {
                    Data = data,
                    Index = i,
                    PrimaryKeys = primaryKeys,
                    Ranges = ranges,
                    Root = root
                };

                var (pk, r) = CreateResult(fromTree.root, recCtx);

                if (!results.Any(x => x.PrimaryKey == pk))
                    results.Add(r);

                r = results.First(x => x.PrimaryKey == pk);
                CreateResultRelations(recCtx, requests.Where(x => x.FullPath != fromTree.root.FullPath).ToList(), r);
            }

            return results;
        }
    }
}
