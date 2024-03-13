using Microsoft.Data.Sqlite;
using System;
using System.Data;
using System.IO;
using Storm.Test.PublicAPI.MockAndModels;
using System.Data.Common;

namespace Storm.Test.PublicAPI.Helpers
{
    static class PrepMethods
    {
        static public DbConnection PrepareDB()
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

            Schema.SchemaModelBuilder editor(Schema.SchemaModelBuilder e)
            {
                return e.Add<User>("User", "Users")
                    .Add<Task>("Task", "Tasks")
                    .Connect("User", "Task", "User", "UserID", "ID")
                    .Add("TaskInfo", "TaskInfos", builder =>
                    {
                        return builder.Add(new Schema.FieldConfig()
                        {
                            CodeName = "ID",
                            CodeType = typeof(Int32),
                            DBName = "ID",
                            DBType = DbType.Int32,
                            IsPrimary = true
                        }).Add(new Schema.FieldConfig()
                        {
                            CodeName = "Field1",
                            CodeType = typeof(string),
                            DBName = "Field1",
                            DBType = DbType.String,
                        }).Add(new Schema.FieldConfig()
                        {
                            CodeName = "Field2",
                            CodeType = typeof(Int32),
                            DBName = "Field2",
                            DBType = DbType.Int32,
                        }).Add(new Schema.FieldConfig()
                        {
                            CodeName = "Field3",
                            CodeType = typeof(bool),
                            DBName = "Field3",
                            DBType = DbType.Boolean,
                        }).Add(new Schema.FieldConfig()
                        {
                            CodeName = "Field4",
                            CodeType = typeof(string),
                            DBName = "Field4",
                            DBType = DbType.String,
                        });
                    }).Connect("Info", "Task", "TaskInfo", "ID", "ID");
            };

            s.EditSchema(editor);

            return s;
        }
    }
}
