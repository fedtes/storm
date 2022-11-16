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

        private static IEnumerable<StormResult> RecursiveGroupResults(
            StormDataSet data,
            IEnumerable<StormRow> subset,
            List<Origin> requests,
            int length)
        {
            var result = new List<StormResult>();
            if (subset.Count() == 0) return result; 
            var thisReq = requests.First();
            var identityFieldPath = data.IdentityIndexes[thisReq.FullPath];
            var range = data.ObjectRanges[thisReq.Entity];
            
            foreach(var g in subset.GroupBy(x => x[identityFieldPath]))
            {
                var b = g.First();
                var sr = new StormResult(new StormRow(data, b.index, range.Start, range.End), thisReq.Entity);
                var childRequest = requests.Where(x => x.FullPath.Count() == length + 1 && x.FullPath.Path.StartsWith(thisReq.FullPath.Path));
                foreach (var r in childRequest)
                {
                    var cr = childRequest.Where(x => x.FullPath.Path.StartsWith(r.FullPath.Path)).ToList();
                    sr.Relations.Add(r.FullPath.Path, RecursiveGroupResults(data, g, cr, length + 1));

                }
                result.Add(sr);

            }

            return result ;
        }


        public static IList<StormResult> ToResults(StormDataSet data, 
                                            Context ctx, 
                                            List<Origin> requests,
                                            OriginTree fromTree)
        {

            return RecursiveGroupResults(data, data, requests.OrderBy(x => x.FullPath.Count()).ToList(), 1).ToList();
        }
    }
}
