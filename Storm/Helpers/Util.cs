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

    }
}
