using System;
using System.Data;
using System.Collections.Generic;
using System.Text;

namespace Storm.Execution
{
    public class StormTransaction : IDisposable
    {
        protected bool autoCommit = false;
        internal IDbTransaction transaction;
        protected StormConnection connection;
        internal bool isCompleted = false;

        internal StormTransaction(StormConnection connection, IDbTransaction transaction)
        {
            this.transaction = transaction;
            this.connection = connection;
        }

        internal StormTransaction(StormConnection connection, IDbTransaction transaction, bool AutoCommit)
        {
            this.autoCommit = AutoCommit;
            this.transaction = transaction;
            this.connection = connection;
        }

        /// <summary>
        /// Execute a Get command to fetch one or more entities from the database given some conditions
        /// </summary>
        /// <param name="EntityIdentifier"></param>
        /// <returns></returns>
        public GetCommand Get(String EntityIdentifier)
        {
            return new GetCommand(connection.navigator, EntityIdentifier)
            {
                connection = this.connection,
                compiler = connection.GetCompiler(),
                transaction = this
            };
        }

        /// <summary>
        /// Execute a Projection command to fetch some columns of one or more entities from the database given some conditions
        /// </summary>
        /// <param name="EntityIdentifier"></param>
        /// <returns></returns>
        public SelectCommand Projection(String EntityIdentifier)
        {
            return new SelectCommand(connection.navigator, EntityIdentifier)
            {
                connection = this.connection,
                compiler = connection.GetCompiler(),
                transaction = this
            };
        }

        /// <summary>
        /// Execute a Set command to INSERT a new record into the database referencing a specific Entity
        /// </summary>
        /// <param name="EntityIdentifier"></param>
        /// <returns></returns>
        public SetCommand Set(String EntityIdentifier)
        {
            return new SetCommand(connection.navigator, EntityIdentifier)
            {
                connection = this.connection,
                compiler = connection.GetCompiler(),
                transaction = this
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
            return new SetCommand(connection.navigator, EntityIdentifier, id)
            {
                connection = this.connection,
                compiler = connection.GetCompiler(),
                transaction = this
            };
        }

        /// <summary>
        /// Execute a DELETE command removing records referencing a specific Entity from the database give some conditions
        /// </summary>
        /// <param name="EntityIdentifier"></param>
        /// <returns></returns>
        public DeleteCommand Delete(String EntityIdentifier)
        {
            return new DeleteCommand(connection.navigator, EntityIdentifier)
            {
                connection = this.connection,
                compiler = connection.GetCompiler(),
                transaction = this
            };
        }


        public void Commit()
        {
            if (!isCompleted)
            {
                transaction.Commit();
                isCompleted = true;
                connection.navigator.GetLogger().Info("Transaction", $"Commit");
            }
        }

        public void Rollback()
        {
            if (!isCompleted)
            {
                transaction.Rollback();
                isCompleted = true;
                connection.navigator.GetLogger().Info("Transaction", $"Rollback");
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
                    if (autoCommit && !isCompleted)
                    {
                        this.Commit();
                    }
                    else if (!autoCommit && !isCompleted)
                    {
                        this.Rollback();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~StormTransaction() {
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
