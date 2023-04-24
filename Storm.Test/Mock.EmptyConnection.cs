using System;
using System.Data;
using System.Data.Common;

namespace Storm.Test
{
    public class EmptyConnection : DbConnection
    {
        private string _x;
        public override string ConnectionString { get => "MOCK CONNECTION STRING"; set => _x = value; }

        public override int ConnectionTimeout => 0;

        public override string Database => "MOCKDB";

        public override ConnectionState State => ConnectionState.Open;

        public override string DataSource => throw new NotImplementedException();

        public override string ServerVersion => throw new NotImplementedException();


        public override void ChangeDatabase(string databaseName)
        {
            //throw new NotImplementedException();
        }

        public override void Close()
        {
            //throw new NotImplementedException();
        }


        public override void Open()
        {
            //throw new NotImplementedException();
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            throw new NotImplementedException();
        }

        protected override DbCommand CreateDbCommand()
        {
            throw new NotImplementedException();
        }
    }
}
