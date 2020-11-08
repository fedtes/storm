using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Data;
using Storm;
using Storm.Execution.Results;
using System.Linq;

namespace Storm.Test.PublicAPI.SQL
{
    public class Read
    {

        public IDbConnection prepareDB()
        {
            var db = new SqliteConnection("Data Source=TestDB;Mode=Memory;");
            db.Open();

            var tslq = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "DBCreation\\CREATE_DB.sql"));

            var cmd = db.CreateCommand();
            cmd.CommandText = tslq;
            cmd.ExecuteNonQuery();


            tslq = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "DBCreation\\ADD_DUMMY_INFO.sql"));

            cmd = db.CreateCommand();
            cmd.CommandText = tslq;
            cmd.ExecuteNonQuery();
            return db;
        }

        [Fact]
        public void It_should_have_prefills_db()
        {
            var db = prepareDB();

            var cmd = db.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Tickets";
            int cnt = 0;
            using (var r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    cnt = r.GetInt32(0);
                }
            }

            Assert.Equal(21, cnt);

            cmd = db.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM UserContacts";
            cnt = 0;
            using (var r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    cnt = r.GetInt32(0);
                }
            }

            Assert.Equal(3, cnt);

            cmd = db.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM UserAddresses";
            cnt = 0;
            using (var r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    cnt = r.GetInt32(0);
                }
            }

            Assert.Equal(3, cnt);

            cmd = db.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Users";
            cnt = 0;
            using (var r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    cnt = r.GetInt32(0);
                }
            }

            Assert.Equal(3, cnt);

            db.Close();
        }


        [Fact]
        public void It_should_get_users()
        {
            var s = new Storm(SQLEngine.SQLite);
            StormDataSet result;
            using (var stormConnection = s.OpenConnection(prepareDB()))
            {
                using (var tran = stormConnection.BeginTransaction())
                {
                    result = tran.Select("User")
                        .Select("ID")
                        .Select("FirstName")
                        .Select("FirstName")
                        .Execute();
                }
            }

            Assert.Equal(3, result.Count());
        }
    }
}
