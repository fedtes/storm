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

        /// <summary>
        /// Edit the schema of Storm. Schema contains all the metadata needed to generate and execute queries. The first time you call this method the schema is empty, you can call it multiple times to add more metadata step by step.
        /// </summary>
        /// <param name="editor"></param>
        public void EditSchema(Func<SchemaEditor, SchemaEditor> editor)
        {
            schema.EditSchema(editor);
        }

        /// <summary>
        /// Open a new StormConnection to execute queries. StormConnection implements IDisposable and should be disposed. When a new StormConnection is open a snapshot of the schema is used. Changes made the schema while a connection is open do not reflects on the queries executed by the connections. Storm DO NOT keep tracks of connection string.
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <returns></returns>
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
