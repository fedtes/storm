using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using Storm.Schema;
using SqlKata.Compilers;

namespace Storm.Execution
{
    public class StormConnection : IDisposable
    {

        protected bool isOpen;
        protected IDbConnection connection;
        protected StormTransaction currentTransaction;
        internal readonly SQLEngine engine;
        internal readonly SchemaNavigator navigator;

        internal StormConnection(SchemaNavigator navigator, IDbConnection connection, SQLEngine engine)
        {
            isOpen = false;
            this.navigator = navigator;
            this.connection = connection;
            this.engine = engine;
        }

        public StormTransaction BeginTransaction(bool AutoCommit = false)
        {
            EnsureTransaction();
            var t = connection.BeginTransaction();
            currentTransaction = new StormTransaction(this, t, AutoCommit); ;
            return currentTransaction;
        }

        public GetCommand Get(String EntityIdentifier)
        {
            return new GetCommand(navigator, EntityIdentifier)
            {
                connection = this,
                compiler = GetCompiler(),
                transaction = null
            };
        }

        public SelectCommand Projection(String EntityIdentifier)
        {
            return new SelectCommand(navigator, EntityIdentifier)
            {
                connection = this,
                compiler = GetCompiler(),
                transaction = null
            };
        }

        public void Open()
        {
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
                throw new ApplicationException("There is already an open transaction!");
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
