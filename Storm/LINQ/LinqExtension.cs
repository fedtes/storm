
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Remotion.Linq;
using Remotion.Linq.Parsing.Structure;

namespace Storm.Linq 
{
    public static class LinqExtensions
    {
        public static IQueryable<T> Select<T>(this IQueryable<T> elements, string path)
        {
            throw new NotImplementedException();
        }


        public static async Task<IEnumerable<dynamic>> ToListAsync<T>(this IQueryable<T> elements)
        {
            if (typeof(StormQuerable<T>) == elements.GetType()) {
                var _executor = (StormQueryExecutor)((DefaultQueryProvider)elements.Provider).Executor;
                elements.ToList();
                return await _executor.command.Execute();
            } else {
                throw new ArgumentException("Only Storm query are supported");
            }
        }


        public static Task<IEnumerable<T>> ToArrayAsync<T>(this IQueryable<T> elements)
        {
            throw new NotImplementedException();
        }


        public static Task<T> FirstAsync<T>(this IQueryable<T> elements)
        {
            throw new NotImplementedException();
        }

        public static Task<T> FirstOrDefaultAsync<T>(this IQueryable<T> elements)
        {
            throw new NotImplementedException();
        }


        public static Task<int> CountAsync<T>(this IQueryable<T> elements)
        {
            throw new NotImplementedException();
        }
    }
}