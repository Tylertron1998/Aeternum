using System.Collections.Generic;

namespace Aeternum.Daemon.IPC.Packets.Outgoing
{
	public struct ManagerLogsPacket
	{
		public OutgoingOpCode OpCode => OutgoingOpCode.Logs;
		public List<string> Logs { get; set; }
	}
}