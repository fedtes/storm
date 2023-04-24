using Storm.Execution;
using Storm.Plugin;
using Storm.Schema;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Storm.Test")]

namespace Storm
{
    public class Storm
    {
        internal readonly Schema.Schema schema;
        internal readonly Dictionary<Guid, ILogService> _logServices = new Dictionary<Guid, ILogService>();
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
        public async Task<StormConnection> OpenConnection(DbConnection sqlConnection)
        {
            var c = new StormConnection(this.CreateContext(), sqlConnection, engine);
            await c.OpenAsync();
            return c;
        }

        internal Context CreateContext() => 
            new Context(schema.GetNavigator(), new Logger(this._logServices.Values.ToArray()));

        /// <summary>
        /// Register a logger to Storm. Return the unique id (guid) that allow to de-register it later.
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public Guid RegisterLogger(ILogService log)
        {
            var id = Guid.NewGuid();
            _logServices.Add(id, log);
            return id;
        }


        /// <summary>
        /// Unregister a previously registered logger with a given unique id (guid)
        /// </summary>
        /// <param name="serviceid"></param>
        /// <returns></returns>
        public bool UnRegisterLogger(Guid serviceid)
        {
           return _logServices.Remove(serviceid);
        }
    }

    public enum SQLEngine
    {
        MSSQLServer = 0,
        MySQL = 1,
        SQLite = 2,
        PostgreSQL = 3
    }
}
