namespace Aeternum.Daemon.Monitoring
{
    [System.Serializable]
    public struct MonitorSettings
    {
        public string Flags { get; set; }
        public string Command { get; set; }
        public string FileName { get; set; }
        public string Name { get; set; }
        public bool ShouldLog { get; set; }
        public bool ShouldLogError { get; set; }

        public int MaxLogs { get; set; }
        public int MaxErrorLogs { get; set; }
        public int MinimumUptime { get; set; }

        public int MaxRetries { get; set; }
    }
}