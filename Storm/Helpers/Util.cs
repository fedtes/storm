using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;


namespace Storm.Helpers
{
    internal static class Util
    {
        public static string UCode()
        {
            byte[] barray = new byte[8];
            (new Random(DateTime.UtcNow.Millisecond)).NextBytes(barray);
            return Encoding.ASCII.GetString(barray.Select(b => b % 25 + 97).Select(b => Convert.ToByte(b)).ToArray());
        }

    }
}
