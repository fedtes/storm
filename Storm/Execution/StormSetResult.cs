using System;
using System.Collections.Generic;
using System.Text;

namespace Storm.Execution
{
    public class StormSetResult
    {
        public readonly object ObjectId;
        public readonly int RowsAffected;

        internal StormSetResult(object objectId, int rowAffected)
        {
            this.ObjectId = objectId;
            this.RowsAffected = rowAffected;
        }

    }
}
