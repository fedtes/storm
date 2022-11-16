using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Storm.Test
{
    public class EmptyConnection : IDbConnection
    {
        private string _x;
        public string ConnectionString { get => "MOCK CONNECTION STRING"; set => _x = value; }

        public int ConnectionTimeout => 0;

        public string Database => "MOCKDB";

        public ConnectionState State => ConnectionState.Open;

        public IDbTransaction BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            throw new NotImplementedException();
        }

        public void ChangeDatabase(string databaseName)
        {
            //throw new NotImplementedException();
        }

        public void Close()
        {
            //throw new NotImplementedException();
        }

        public IDbCommand CreateCommand()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public void Open()
        {
            //throw new NotImplementedException();
        }
    }
}
