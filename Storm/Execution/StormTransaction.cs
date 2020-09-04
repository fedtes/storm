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

        public GetCommand Get(String EntityIdentifier)
        {
            return new GetCommand(connection.navigator, EntityIdentifier)
            {
                connection = this.connection,
                transaction = this
            };
        }

        public SelectCommand Select(String EntityIdentifier)
        {
            return new SelectCommand(connection.navigator, EntityIdentifier)
            {
                connection = this.connection,
                transaction = this
            };
        }

        public void Commit()
        {
            if (!isCompleted)
            {
                transaction.Commit();
                isCompleted = true;
            }
        }

        public void Rollback()
        {
            if (!isCompleted)
            {
                transaction.Rollback();
                isCompleted = true;
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
