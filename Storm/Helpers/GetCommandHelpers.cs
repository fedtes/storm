using Storm.Execution;
using Storm.Execution.Results;
using Storm.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        static (Object, StormResult) CreateResult(FromNode fromNode,RecCtx ctx)
        {
            var range = ctx.Ranges[fromNode.Entity];
            var row = new StormRow(ctx.Data, ctx.Index, range.Start, range.End);
            var primaryKey = row[ctx.PrimaryKeys[fromNode.FullPath]];
            return (primaryKey, new StormResult(row, fromNode.Entity));
        }
        
        static void CreateResultRelations(RecCtx ctx, 
                                          List<FromNode> requests,
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
                                            SchemaNavigator navigator, 
                                            List<FromNode> requests,
                                            FromTree fromTree)
        {
            var root = navigator.GetEntity(data.root);
            var ranges = data.ObjectRanges;
            var primaryKeys = data.IdentityIndexes;
            var items = new Dictionary<EntityPath, Dictionary<object, StormResult>>();
            var results = new List<StormResult>();

            for (int i = 0; i < data.Count(); i++)
            {
                var ctx = new RecCtx()
                {
                    Data = data,
                    Index = i,
                    PrimaryKeys = primaryKeys,
                    Ranges = ranges,
                    Root = root
                };

                var (pk, r) = CreateResult(fromTree.root, ctx);

                if (!results.Any(x => x.PrimaryKey == pk))
                    results.Add(r);

                r = results.First(x => x.PrimaryKey == pk);
                CreateResultRelations(ctx, requests.Where(x => x.FullPath != fromTree.root.FullPath).ToList(), r);
            }

            return results;
        }
    }
}
