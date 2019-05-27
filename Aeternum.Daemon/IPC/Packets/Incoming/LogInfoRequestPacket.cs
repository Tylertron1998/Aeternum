using System;
using Aeternum.Daemon.Monitoring.Logging;
using Newtonsoft.Json;

namespace Aeternum.Daemon.IPC.Packets.Incoming
{
	public struct LogInfoRequestPacket
	{
		public LogLevel LevelFilter { get; }
		public int MaxResults { get; }

		[JsonConstructor]
		public LogInfoRequestPacket(LogLevel level = LogLevel.All, int maxResults = Int32.MaxValue)
		{
			LevelFilter = level;
			MaxResults = maxResults;
		}
	}
}