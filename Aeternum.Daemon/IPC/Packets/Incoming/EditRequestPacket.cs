namespace Aeternum.Daemon.IPC.Packets.Incoming
{
    public struct EditRequestPacket
    {
        public string Name { get; set; }
        public bool Save { get; set; }
        public string? Command { get; set; }
        public string? FileName { get; set; }
        public string? Flags { get; set; }
        
        public bool? ShouldSave { get; set; }
        
        public bool? CaptureStandardIn { get; set; }
        public bool? CaptureStandardOut { get; set; }
        public int? MaxLogs { get; set;  }
        public int? MaxErrorLogs { get; set; }
        
        public int? MaxRetries { get; set; }
        public int? MinimumStableUptime { get; set; }
    }
}