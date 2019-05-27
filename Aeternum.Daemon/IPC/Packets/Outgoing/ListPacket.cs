using System.Collections.Generic;

namespace Aeternum.Daemon.IPC.Packets.Outgoing
{
    public struct ListPacket
    {
        public List<string> Monitors { get; set; }
    }
}