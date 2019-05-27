using System.Collections.Generic;

namespace Aeternum.Daemon.Monitoring.Logging
{
    public struct MonitorLogs
    {
        public List<string> StandardOut { get; set; }
        public List<string> StandardError { get; set; }
    }
}