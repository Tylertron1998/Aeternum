using Aeternum.Daemon.Monitoring.Logging;

namespace Aeternum.Daemon.IPC.Packets.Outgoing
{
	public struct MonitorLogsPacket
	{
		public MonitorLogs Logs { get; set; }
	}
}