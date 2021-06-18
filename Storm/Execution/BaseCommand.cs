using SqlKata;
using Storm.Schema;
using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using SqlKata.Compilers;
using System.Diagnostics;

namespace Storm.Execution
{
    public abstract class BaseCommand
    {
        internal String rootEntity;
        internal SchemaNavigator navigator;
        internal Query query;
        internal StormConnection connection;
        internal StormTransaction transaction;
        internal Compiler compiler;
        internal Guid commandId;
        internal Stopwatch sw;

        internal BaseCommand(SchemaNavigator navigator, String from)
        {
            this.navigator = navigator;
            this.rootEntity = navigator.GetEntity(from).ID;
            this.query = new Query($"{navigator.GetEntity(from).DBName} as A0");
            commandId = Guid.NewGuid();
            sw = new Stopwatch();
            navigator.GetLogger().Info($"Command-{commandId}", "Initalized");
        }

        internal abstract void ParseSQL();

        internal abstract Object Read(IDataReader dataReader);

        public Object Execute()
        {
            using (var t = transaction == null ? connection.BeginTransaction(true) : transaction)
            {
                IDbCommand cmd;

                try
                {
                    compiler = compiler == null ? new SqlServerCompiler() : compiler;
                    this.ParseSQL();
                    SqlResult result = compiler.Compile(query);
                    cmd = t.transaction.Connection.CreateCommand();
                    cmd.CommandText = result.Sql;
                    cmd.Transaction = t.transaction;

                    foreach (var binding in result.NamedBindings)
                    {
                        var p = cmd.CreateParameter();
                        p.ParameterName = binding.Key;
                        p.Value = binding.Value;
                        cmd.Parameters.Add(p);
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Error parsing query", ex);
                }
                
                try
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        return Read(reader);
                    }
                }
                catch (Exception ex)
                {
                    t.Rollback();
                    throw new ApplicationException("Error executing query", ex);
                }
            }
        }

    }

}

