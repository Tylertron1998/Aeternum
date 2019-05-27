using Newtonsoft.Json;

namespace Aeternum.Daemon.IPC.Packets.Incoming
{
	public struct StartRequestPacket
	{
		public string Name { get; set; }
    }
}