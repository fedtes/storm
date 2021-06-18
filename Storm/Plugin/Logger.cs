using System;
using System.Collections.Generic;
using System.Text;

namespace Storm.Plugin
{
    /// <summary>
    /// Implement this to regiter a logging service to storm
    /// </summary>
    public interface ILogService
    {
        /// <summary>
        /// Called when a log event occurs. WARNING: this call is sync with the execution so ensure to handle the event without block or slow down the execution.
        /// </summary>
        void OnEvent(LogEvent @event);
    }
}
