
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Remotion.Linq;
using Remotion.Linq.Parsing.Structure;
using Storm.Execution;

namespace Storm.Linq 
{
    public static class LinqExtensions
    {
        public static IQueryable<T> Select<T>(this IQueryable<T> elements, string path)
        {

            // if (typeof(StormQuerable<T>) == elements.GetType()) {
            //     var _executor = (StormQueryExecutor)((DefaultQueryProvider)elements.Provider).Executor;
            //     _executor.command = new SelectCommand(_executor.command.ctx, _executor.command.rootEntity);
            //     (_executor.command as SelectCommand).Select(path);
                
            // } else {
            //     throw new ArgumentException("Only Storm query are supported");
            // }
            // return elements;

            throw new NotImplementedException();
        }


        public static async Task<IEnumerable<dynamic>> ToListAsync<T>(this IQueryable<T> elements) where T : Entity
        {
            if (typeof(StormQuerable<T>) == elements.GetType()) {
                var _executor = (StormQueryExecutor)((DefaultQueryProvider)elements.Provider).Executor;
                elements.ToList();
                return await (_executor.command as GetCommand).Execute();

                // if (typeof(SelectCommand) == _executor.command.GetType()) {
                //     return await (_executor.command as SelectCommand).Execute();
                // } else {
                //     return await (_executor.command as GetCommand).Execute();
                // }

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