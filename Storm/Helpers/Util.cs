using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Storm.Execution;

namespace Storm.Helpers
{
    internal static class Util
    {
        public static string UCode()
        {
            byte[] barray = new byte[8];
            new Random().NextBytes(barray);
            return Encoding.ASCII.GetString(barray.Select(b => b % 25 + 97).Select(b => Convert.ToByte(b)).ToArray());
        }

        public static void CommandLog(this BaseCommand command,
                                      LogLevel level,
                                      String source,
                                      String payload)
        {
            var conid = command.connection == null ? "00000000" : command.connection.connectionId;
            var tranid = command.transaction == null ? "00000000" : command.transaction.transactionid;
            var cmdid = command.commandId;
            switch (level)
            {
                case LogLevel.Trace:
                    command.ctx.GetLogger().Trace(source, payload, conid, tranid, cmdid);
                    break;
                case LogLevel.Debug:
                    command.ctx.GetLogger().Debug(source, payload, conid, tranid, cmdid);
                    break;
                case LogLevel.Info:
                    command.ctx.GetLogger().Info(source, payload, conid, tranid, cmdid);
                    break;
                //case LogLevel.Error:
                //    command.ctx.GetLogger().Error(source, payload, conid, tranid, cmdid);
                //    break;

                default:

                    break;
            }

        }


        public static string JSONClean(string s)
        {
            if (s == null || s.Length == 0)
            {
                return "";
            }

            char c = '\0';
            int i;
            int len = s.Length;
            StringBuilder sb = new StringBuilder(len + 4);
            String t;

            for (i = 0; i < len; i += 1)
            {
                c = s[i];
                switch (c)
                {
                    case '\\':
                    case '"':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '/':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    default:
                        if (c < ' ')
                        {
                            t = "000" + String.Format("{0:X}", (int)c);
                            sb.Append("\\u" + t.Substring(t.Length - 4));
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }
            return sb.ToString();
        }


    }
}
