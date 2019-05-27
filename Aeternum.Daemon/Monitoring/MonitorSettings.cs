namespace Aeternum.Daemon.Monitoring
{
	[System.Serializable]
	public struct MonitorSettings
	{
		public string Flags { get; set; }
		public string Name { get; set; }
		public string FileName { get; set; }
		public string UserName { get; set; }
		public bool ShouldLog { get; set; }

		public int MinimumUptime { get; set; }

		public int MaxRetries { get; set; }

		public MonitorSettings(string flags, string fileName, string name, string userName, bool shouldLog,
			int minimumUptime,
			int maxRetries)
		{
			Flags = flags;
			Name = name;
			FileName = fileName;
			UserName = userName;
			ShouldLog = shouldLog;
			MinimumUptime = minimumUptime;
			MaxRetries = maxRetries;
		}
	}
}