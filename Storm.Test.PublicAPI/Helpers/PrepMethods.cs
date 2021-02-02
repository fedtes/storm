using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using Storm.Test.PublicAPI.MockAndModels;

namespace Storm.Test.PublicAPI.Helpers
{
    static class PrepMethods
    {
        static public IDbConnection PrepareDB()
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

        static public Storm PrepareStorm()
        {
            var s = new Storm(SQLEngine.SQLite);

            Schema.SchemaEditor editor(Schema.SchemaEditor e)
            {
                return e.Add<User>("User", "Users")
                    .Add<Task>("Task", "Tasks")
                    .Connect("User", "Task", "User", "UserID", "ID");
            };

            s.EditSchema(editor);

            return s;
        }
    }
}
