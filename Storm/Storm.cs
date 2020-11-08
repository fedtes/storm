using Storm.Execution;
using Storm.Schema;
using System;
using System.Data;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Storm.Test")]

namespace Storm
{
    public class Storm
    {
        internal readonly Schema.Schema schema;
        internal readonly SQLEngine engine;

        public Storm()
        {
            this.schema = new Schema.Schema();
            this.engine = SQLEngine.MSSQLServer;
        }

        public Storm(SQLEngine engine)
        {
            this.schema = new Schema.Schema();
            this.engine = engine;
        }

        public void EditSchema(Func<SchemaEditor, SchemaEditor> editor)
        {
            schema.EditSchema(editor);
        }

        public StormConnection OpenConnection(IDbConnection sqlConnection)
        {
            var c = new StormConnection(schema.GetNavigator(), sqlConnection, engine);
            c.Open();
            return c;
        }
    }

    public enum SQLEngine
    {
        MSSQLServer = 0,
        MySQL = 1,
        SQLite = 2
    }
}
