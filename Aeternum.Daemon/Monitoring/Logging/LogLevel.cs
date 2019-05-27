using System;

namespace Aeternum.Daemon.Monitoring.Logging
{
	[Flags]
	public enum LogLevel
	{
		None,
		Information,
		Warning,
		Error,
		All = Error | Warning | Information
	}
}