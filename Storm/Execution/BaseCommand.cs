﻿using SqlKata;
using System;
using System.Data;
using SqlKata.Compilers;
using System.Diagnostics;
using System.Linq;
using Storm.Helpers;
using System.Threading.Tasks;
using System.Data.Common;

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

        protected async Task<T> InternalExecute<T>()
        {

            this.CommandLog(LogLevel.Trace, "Command", $"{{\"Action\":\"Begin\"}}");
            DbCommand cmd;
            sw.Start();

            try
            {
                cmd = connection.connection.CreateCommand();
                SqlResult result = Compile();
                PopulateCommandText(cmd, result);
                BindParameters(cmd, result);

                this.CommandLog(LogLevel.Trace, "Command", $"{{\"Action\":\"Compiled SQL\", \"Time\":\"{sw.ElapsedMilliseconds}\" }}");
                this.CommandLog(LogLevel.Debug, "Command", $"{{\"SQL\":\"{Util.JSONClean(cmd.CommandText)}\", \"Params\":\"{Util.JSONClean(String.Join("|",result.NamedBindings.Select(nb => nb.Key + "=" + nb.Value)))}\" }}");
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
                return await ExecuteQuery<T>(cmd);
            }
            catch (Exception ex)
            {
                await transaction.Rollback();
                throw new ApplicationException("Error executing query", ex);
            }
            finally
            {
                if (isLocalTransaction)
                {
                    await transaction.DisposeAsync();
                }
            }
            
        }

        protected virtual async Task<T> ExecuteQuery<T>(DbCommand cmd)
        {
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                this.CommandLog(LogLevel.Trace, "Command", $"{{\"Action\":\"Executed\", \"Time\":\"{sw.ElapsedMilliseconds}\" }}");
                return (T)Read(reader);
            }
        }

        protected virtual void BindParameters(IDbCommand cmd, SqlResult result)
        {
            foreach (var binding in result.NamedBindings)
            {
                var p = cmd.CreateParameter();
                p.ParameterName = binding.Key;
                p.Value = binding.Value;
                cmd.Parameters.Add(p);
            }
        }

        internal virtual void PopulateCommandText(IDbCommand cmd, SqlResult result)
        {
            cmd.CommandText = result.Sql;
        }

        protected virtual SqlResult Compile()
        {
            compiler = compiler == null ? new SqlServerCompiler() : compiler;
            this.CommandLog(LogLevel.Trace, "Command", $"{{\"Action\":\"Selected Compiler\", \"Time\":\"{sw.ElapsedMilliseconds}\", \"Compiler\":\"{compiler.GetType().Name}\" }}");

            this.ParseSQL();
            this.CommandLog(LogLevel.Trace, "Command", $"{{\"Action\":\"Parsed SQL\", \"Time\":\"{sw.ElapsedMilliseconds}\" }}");

            SqlResult result = compiler.Compile(query);
            return result;
        }
    }

}

