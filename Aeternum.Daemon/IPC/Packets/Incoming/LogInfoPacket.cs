using System;
using Aeternum.Daemon.Monitoring.Logging;
using Newtonsoft.Json;

namespace Aeternum.Daemon.IPC.Packets.Incoming
{
	public class LogInfoPacket
	{
		public LogLevel LevelFilter { get; set; }
		public int MaxResults { get; }

		[JsonConstructor]
		public LogInfoPacket(LogLevel level = LogLevel.All, int maxResults = Int32.MaxValue)
		{
			LevelFilter = level;
			MaxResults = maxResults;
		}
	}
}