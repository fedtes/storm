using System;
using System.Collections.Generic;
using System.Text;

namespace Storm.Execution
{
    public class StormDeleteResult
    {
        public readonly int rowAffected;

        internal StormDeleteResult(int rowAffected)
        {
            this.rowAffected = rowAffected;
        }

    }
}
