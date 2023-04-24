using System;
using Storm.Helpers;
using System.Data.Common;
using System.Threading.Tasks;

namespace Storm.Execution
{
    public class StormTransaction : IAsyncDisposable
    {
        protected bool autoCommit = false;
        internal DbTransaction transaction;
        protected StormConnection connection;
        internal bool isCompleted = false;
        internal readonly String transactionid;

        internal StormTransaction(StormConnection connection, DbTransaction transaction)
        {
            this.transaction = transaction;
            this.connection = connection;
            this.transactionid = Util.UCode();
        }

        internal StormTransaction(StormConnection connection, DbTransaction transaction, bool AutoCommit)
        {
            this.autoCommit = AutoCommit;
            this.transaction = transaction;
            this.connection = connection;
            this.transactionid = Util.UCode();
        }

        /// <summary>
        /// Execute a Get command to fetch one or more entities from the database given some conditions
        /// </summary>
        /// <param name="EntityIdentifier"></param>
        /// <returns></returns>
        public GetCommand Get(String EntityIdentifier)
        {
            return new GetCommand(connection.ctx, EntityIdentifier)
            {
                connection = this.connection,
                compiler = connection.GetCompiler(),
                transaction = this
            };
        }

        /// <summary>
        /// Execute a Projection command to fetch some columns of one or more entities from the database given some conditions
        /// </summary>
        /// <param name="EntityIdentifier"></param>
        /// <returns></returns>
        public SelectCommand Projection(String EntityIdentifier)
        {
            return new SelectCommand(connection.ctx, EntityIdentifier)
            {
                connection = this.connection,
                compiler = connection.GetCompiler(),
                transaction = this
            };
        }

        /// <summary>
        /// Execute a Set command to INSERT a new record into the database referencing a specific Entity
        /// </summary>
        /// <param name="EntityIdentifier"></param>
        /// <returns></returns>
        public SetCommand Insert(String EntityIdentifier)
        {
            return new SetCommand(connection.ctx, EntityIdentifier)
            {
                connection = this.connection,
                compiler = connection.GetCompiler(),
                transaction = this
            };
        }

        /// <summary>
        /// Execute a Set command to UPDATAE a record into the database referencing a specific Entity. id is the PrimaryKey value of the Entity to update.
        /// </summary>
        /// <param name="EntityIdentifier"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public SetCommand Update(String EntityIdentifier, object id)
        {
            return new SetCommand(connection.ctx, EntityIdentifier, id)
            {
                connection = this.connection,
                compiler = connection.GetCompiler(),
                transaction = this
            };
        }

        /// <summary>
        /// Execute a DELETE command removing records referencing a specific Entity from the database give some conditions
        /// </summary>
        /// <param name="EntityIdentifier"></param>
        /// <returns></returns>
        public DeleteCommand Delete(String EntityIdentifier)
        {
            return new DeleteCommand(connection.ctx, EntityIdentifier)
            {
                connection = this.connection,
                compiler = connection.GetCompiler(),
                transaction = this
            };
        }


        public async Task Commit()
        {
            if (!isCompleted)
            {
                await transaction.CommitAsync();
                isCompleted = true;
                connection.ctx.GetLogger().Info("Transaction", $"{{\"Action\":\"Commit\"}}", this.connection.connectionId, this.transactionid);
            }
        }

        public async Task Rollback()
        {
            if (!isCompleted)
            {
                await transaction.RollbackAsync();
                isCompleted = true;
                connection.ctx.GetLogger().Info("Transaction", $"{{\"Action\":\"Rollback\"}}", this.connection.connectionId, this.transactionid);
            }
        }

#region IDisposable Support
       

        public async ValueTask DisposeAsync()
        {
            if (autoCommit && !isCompleted)
            {
                await this.Commit();
            }
            else if (!autoCommit && !isCompleted)
            {
                await this.Rollback();
            }
        }

#endregion
    }
}
