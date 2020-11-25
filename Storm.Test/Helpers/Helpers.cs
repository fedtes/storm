using System;
using System.Collections.Generic;
using System.Text;

namespace Storm.Test
{
    public static class Helpers
    {
        public static string Checksum(string s)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                return BitConverter
                    .ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(s)))
                    .Replace("-", String.Empty);
            }
        }
    }
}
