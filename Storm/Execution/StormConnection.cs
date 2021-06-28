using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using Storm.Schema;
using SqlKata.Compilers;
using Storm.Helpers;

namespace Storm.Execution
{
    /// <summary>
    /// Wraps the logics of interaction with the database. Use the methods Get, Set, Delete, Projection here or begin a new transaction if you want to operate on transactional scope.
    /// </summary>
    public class StormConnection : IDisposable
    {

        protected bool isOpen;
        internal IDbConnection connection;
        protected StormTransaction currentTransaction;
        internal readonly SQLEngine engine;
        internal readonly SchemaNavigator navigator;
        internal readonly String connectionId;

        internal StormConnection(SchemaNavigator navigator, IDbConnection connection, SQLEngine engine)
        {
            isOpen = false;
            this.navigator = navigator;
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
            navigator.GetLogger().Info("Connection", $"{{\"Action\":\"Begin Transaction\",\"AutoCommit\":\"{AutoCommit}\"}}", this.connectionId);
            return currentTransaction;
        }
        
        /// <summary>
        /// Execute a Get command to fetch one or more entities from the database given some conditions
        /// </summary>
        /// <param name="EntityIdentifier"></param>
        /// <returns></returns>
        public GetCommand Get(String EntityIdentifier)
        {
            return new GetCommand(navigator, EntityIdentifier)
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
            return new SelectCommand(navigator, EntityIdentifier)
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
        public SetCommand Set(String EntityIdentifier)
        {
            return new SetCommand(navigator, EntityIdentifier)
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
        public SetCommand Set(String EntityIdentifier, object id)
        {
            return new SetCommand(navigator, EntityIdentifier, id)
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
            return new DeleteCommand(navigator, EntityIdentifier)
            {
                connection = this,
                compiler = GetCompiler(),
                transaction = null
            };
        }

        internal void Open()
        {
            navigator.GetLogger().Info("Connection", $"{{\"Action\":\"Open\"}}", this.connectionId);
            if (!isOpen)
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
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
                default:
                    return new SqlServerCompiler();
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (isOpen)
                    {
                        navigator.GetLogger().Info("Connection", $"{{\"Action\":\"Close\"}}", this.connectionId);
                        connection.Close();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~StormConnection() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
#endregion
    }
}
