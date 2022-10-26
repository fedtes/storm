using Storm.Plugin;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Storm
{
    public enum LogLevel
    {
        Trace = 0,
        Debug = 1,
        Info= 2
    }

    public class LogEvent
    {
        public string EventID { get; internal set; }
        public DateTime EventTime { get; internal set; }
        public LogLevel Level { get; internal set; }
        public String Source { get; internal set; }
        public String Payload { get; internal set; }
    }

    internal class Logger
    {
        private readonly ILogService[] service;

        public Logger(ILogService[] service)
        {
            this.service = service;
        }

        //public void Error(String source, String payload, String connectionId = "00000000", String transactionId = "00000000", String commandId = "00000000")
        //{
        //    var e = new LogEvent()
        //    {
        //        EventID = $"{connectionId}.{transactionId}.{commandId}",
        //        EventTime = DateTime.UtcNow,
        //        Level = LogLevel.Error,
        //        Source = source,
        //        Payload = payload
        //    };

        //    foreach (var s in service)
        //    {
        //        s.OnEvent(e);
        //    }
        //}

        public void Info(String source, String payload, String connectionId = "00000000", String transactionId = "00000000", String commandId = "00000000")
        {
            var e = new LogEvent()
            {
                EventID = $"{connectionId}.{transactionId}.{commandId}",
                EventTime = DateTime.UtcNow,
                Level = LogLevel.Info,
                Source = source,
                Payload = payload
            };

            foreach (var s in service)
            {
                s.OnEvent(e);
            }
        }

        public void Trace(String source, String payload, String connectionId = "00000000", String transactionId = "00000000", String commandId = "00000000")
        {
            var e = new LogEvent()
            {
                EventID = $"{connectionId}.{transactionId}.{commandId}",
                EventTime = DateTime.UtcNow,
                Level = LogLevel.Trace,
                Source = source,
                Payload = payload
            };

            foreach (var s in service)
            {
                s.OnEvent(e);
            }
        }

        public void Debug(String source, String payload, String connectionId = "00000000", String transactionId = "00000000", String commandId = "00000000")
        {
            var e = new LogEvent()
            {
                EventID = $"{connectionId}.{transactionId}.{commandId}",
                EventTime = DateTime.UtcNow,
                Level = LogLevel.Debug,
                Source = source,
                Payload = payload
            };

            foreach (var s in service)
            {
                s.OnEvent(e);
            }
        }
    }
}
