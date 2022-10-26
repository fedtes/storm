using SqlKata;
using Storm.Schema;
using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using SqlKata.Compilers;
using System.Diagnostics;
using System.Linq;
using Storm.Helpers;

namespace Storm.Execution
{
    public abstract class BaseCommand
    {
        internal String rootEntity;
        internal Context ctx;
        internal Query query;
        internal StormConnection connection;
        internal StormTransaction transaction;
        internal Compiler compiler;
        internal String commandId;
        internal Stopwatch sw;

        internal BaseCommand(Context ctx, String from)
        {
            this.ctx = ctx;
            this.rootEntity = ctx.Navigator.GetEntity(from).ID;
            this.query = new Query($"{ctx.Navigator.GetEntity(from).DBName} as A0");
            commandId = Helpers.Util.UCode();
            sw = new Stopwatch();
        }

        internal abstract void ParseSQL();

        internal abstract Object Read(IDataReader dataReader);

        public Object Execute()
        {

            this.CommandLog(LogLevel.Trace, "Command", $"{{\"Action\":\"Begin\"}}");
            IDbCommand cmd;
            sw.Start();

            try
            {
                compiler = compiler == null ? new SqlServerCompiler() : compiler;
                this.CommandLog(LogLevel.Trace, "Command", $"{{\"Action\":\"Selected Compiler\", \"Time\":\"{sw.ElapsedMilliseconds}\", \"Compiler\":\"{compiler.GetType().Name}\" }}");

                this.ParseSQL();
                this.CommandLog(LogLevel.Trace, "Command", $"{{\"Action\":\"Parsed SQL\", \"Time\":\"{sw.ElapsedMilliseconds}\" }}");

                SqlResult result = compiler.Compile(query);

                cmd = connection.connection.CreateCommand();
                cmd.CommandText = result.Sql;
                

                foreach (var binding in result.NamedBindings)
                {
                    var p = cmd.CreateParameter();
                    p.ParameterName = binding.Key;
                    p.Value = binding.Value;
                    cmd.Parameters.Add(p);
                }

                this.CommandLog(LogLevel.Trace, "Command", $"{{\"Action\":\"Compiled SQL\", \"Time\":\"{sw.ElapsedMilliseconds}\" }}");
                this.CommandLog(LogLevel.Debug, "Command", $"{{\"SQL\":\"{result.Sql}\", \"Params\":\"{result.NamedBindings.Select(nb => nb.Key + "=" + nb.Value)}\" }}");
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error parsing query", ex);
            }

            bool isLocalTransaction = transaction == null;
            transaction = isLocalTransaction ? connection.BeginTransaction(true) : transaction;

            try
            {
                this.CommandLog(LogLevel.Trace, "Command", $"{{\"Action\":\"Execute\", \"Transaction\":\"{(isLocalTransaction ? "Local" : "External")}\"}}");
                cmd.Transaction = transaction.transaction;
                using (var reader = cmd.ExecuteReader())
                {
                    this.CommandLog(LogLevel.Trace, "Command", $"{{\"Action\":\"Executed\", \"Time\":\"{sw.ElapsedMilliseconds}\" }}");
                    return Read(reader);
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new ApplicationException("Error executing query", ex);
            }
            finally
            {
                if (isLocalTransaction)
                {
                    transaction.Dispose();
                }
            }
            
        }

    }

}

