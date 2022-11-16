using System.Data;

namespace Storm.Test
{
    public class EmptyTransaction : IDbTransaction
    {
        public IDbConnection Connection => new EmptyConnection();

        public IsolationLevel IsolationLevel => IsolationLevel.Chaos;

        public void Commit()
        {
            //throw new NotImplementedException();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public void Rollback()
        {
            //throw new NotImplementedException();
        }
    }
}
