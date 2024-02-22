using System;
using System.Data;
using SqlKata.Compilers;
using Storm.Helpers;
using System.Data.Common;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Storm.Linq;
using Remotion.Linq.Parsing.Structure;

namespace Storm.Execution
{
    /// <summary>
    /// Wraps the logics of interaction with the database. Use the methods Get, Set, Delete, Projection here or begin a new transaction if you want to operate on transactional scope.
    /// </summary>
    public class StormConnection : IAsyncDisposable
    {

        protected bool isOpen;
        internal DbConnection connection;
        protected StormTransaction currentTransaction;
        internal readonly SQLEngine engine;
        internal readonly Context ctx;
        internal readonly String connectionId;

        internal StormConnection(Context ctx, DbConnection connection, SQLEngine engine)
        {
            isOpen = false;
            this.ctx = ctx;
            this.connection = connection;
            this.engine = engine;
            this.connectionId = Util.UCode();
        }

        /// <summary>
        /// Open a new StormTransaction to execute queries in a transactional scope. If AutoCommit is set to True then when the Dispose method is call the transaction is automattically commited else is automatically rollbacked (unless rollback and commit are called explicitally). StormTransaction implements IDisposable and should be disposed.
        /// </summary>
        /// <param name="AutoCommit"></param>
        /// <returns></returns>
        public StormTransaction BeginTransaction(bool AutoCommit = false)
        {
            EnsureTransaction();
            var t = connection.BeginTransaction();
            currentTransaction = new StormTransaction(this, t, AutoCommit);
            ctx.GetLogger().Info("Connection", $"{{\"Action\":\"Begin Transaction\",\"AutoCommit\":\"{AutoCommit}\"}}", this.connectionId);
            return currentTransaction;
        }
        
        /// <summary>
        /// Execute a Get command to fetch one or more entities from the database given some conditions
        /// </summary>
        /// <param name="EntityIdentifier"></param>
        /// <returns></returns>
        public GetCommand Get(String EntityIdentifier)
        {
            return new GetCommand(ctx, EntityIdentifier)
            {
                connection = this,
                compiler = GetCompiler(),
                transaction = null
            };
        }
        
        /// <summary>
        /// Execute a Projection command to fetch some columns of one or more entities from the database given some conditions
        /// </summary>
        /// <param name="EntityIdentifier"></param>
        /// <returns></returns>
        public SelectCommand Projection(String EntityIdentifier)
        {
            return new SelectCommand(ctx, EntityIdentifier)
            {
                connection = this,
                compiler = GetCompiler(),
                transaction = null
            };
        }

        /// <summary>
        /// Execute a Set command to INSERT a new record into the database referencing a specific Entity
        /// </summary>
        /// <param name="EntityIdentifier"></param>
        /// <returns></returns>
        public SetCommand Insert(String EntityIdentifier)
        {
            return new SetCommand(ctx, EntityIdentifier)
            {
                connection = this,
                compiler = GetCompiler(),
                transaction = null
            };
        }

        /// <summary>
        /// Execute a Set command to UPDATAE a record into the database referencing a specific Entity. id is the PrimaryKey value of the Entity to update.
        /// </summary>
        /// <param name="EntityIdentifier"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public SetCommand Update(String EntityIdentifier, object id)
        {
            return new SetCommand(ctx, EntityIdentifier, id)
            {
                connection = this,
                compiler = GetCompiler(),
                transaction = null
            };
        }
        
        /// <summary>
        /// Execute a DELETE command removing records referencing a specific Entity from the database give some conditions
        /// </summary>
        /// <param name="EntityIdentifier"></param>
        /// <returns></returns>
        public DeleteCommand Delete(String EntityIdentifier)
        {
            return new DeleteCommand(ctx, EntityIdentifier)
            {
                connection = this,
                compiler = GetCompiler(),
                transaction = null
            };
        }

        internal async Task OpenAsync()
        {
            ctx.GetLogger().Info("Connection", $"{{\"Action\":\"Open\"}}", this.connectionId);
            if (!isOpen)
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                isOpen = true;
            }
        }


        private void EnsureTransaction()
        {
            if (currentTransaction != null && !currentTransaction.isCompleted)
            {
                throw new TransactionAlreadyOpenException();
            }
        }      

        internal Compiler GetCompiler()
        {
            switch (engine)
            {
                case SQLEngine.MSSQLServer:
                    return new SqlServerCompiler();
                case SQLEngine.MySQL:
                    return new MySqlCompiler();
                case SQLEngine.SQLite:
                    return new SqliteCompiler();
                case SQLEngine.PostgreSQL:
                    return new PostgresCompiler();
                default:
                    return new SqlServerCompiler();
            }
        }

#region "LINQ"

        public IQueryable<StormQueryContext> From(String EntityIdentifier)
        {
            return new StormQuerable<StormQueryContext>(QueryParser.CreateDefault(), new StormQueryExecutor(this, EntityIdentifier));
        }

#endregion


#region IDisposable Support
        public async ValueTask DisposeAsync()
        {
            if (isOpen)
                {
                    ctx.GetLogger().Info("Connection", $"{{\"Action\":\"Close\"}}", this.connectionId);
                    await connection.CloseAsync();
                }
        }
        
#endregion
    }
}
