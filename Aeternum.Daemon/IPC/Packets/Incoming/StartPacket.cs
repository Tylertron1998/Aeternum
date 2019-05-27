using Newtonsoft.Json;

namespace Aeternum.Daemon.IPC.Packets.Incoming
{
	public struct StartPacket
	{
		public string Name { get; set; }
		public string FileName { get; set; }
		public string Args { get; set; }
		public string UserName { get; set; }
		public int MaxRetries { get; set; }
		public int MinimumUptime { get; set; }
		public bool SaveLogs { get; set; }

		[JsonConstructor]
		public StartPacket(string name, string fileName, string args = "", string userName = "", int maxRetries = 0,
			int minimumUptime = 0, bool saveLogs = false)
		{
			Name = name;
			FileName = fileName;
			Args = args;
			UserName = userName;
			MaxRetries = maxRetries;
			MinimumUptime = minimumUptime;
			SaveLogs = saveLogs;
		}
	}
}