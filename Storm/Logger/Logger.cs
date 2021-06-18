using Storm.Plugin;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Storm
{
    public enum LogLevel
    {
        Profile = 0,
        Info = 1,
        Error = 2
    }

    public class LogEvent
    {
        public Guid ConnectionID { get; internal set; }
        public DateTime EventTime { get; internal set; }
        public LogLevel Level { get; internal set; }
        public String Source { get; internal set; }
        public String Payload { get; internal set; }
    }

    internal class Logger
    {
        private readonly ILogService[] service;
        private Guid connectionID;

        public Logger(ILogService[] service, Guid connectionID)
        {
            this.service = service;
            this.connectionID = connectionID;
        }

        public void Error(String source, String payload)
        {
            var e = new LogEvent()
            {
                ConnectionID = this.connectionID,
                EventTime = DateTime.UtcNow,
                Level = LogLevel.Error,
                Source = source,
                Payload = payload
            };

            foreach (var s in service)
            {
                s.OnEvent(e);
            }
        }

        public void Info(String source, String payload)
        {
            var e = new LogEvent()
            {
                ConnectionID = this.connectionID,
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

        public void Profile(String source, String payload)
        {
            var e = new LogEvent()
            {
                ConnectionID = this.connectionID,
                EventTime = DateTime.UtcNow,
                Level = LogLevel.Profile,
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
