using Aeternum.Daemon.Monitoring.Logging;

namespace Aeternum.Daemon.Logs
{
	public struct AlreadyStartedLog : ILog
	{
		public LogLevel Level => LogLevel.Warning;
		public string Message { get; }

		public AlreadyStartedLog(string name)
		{
			Message = $"PROCESS ALREADY STARTED: {name} was attempted to be started, however, it is already started.";
		}
	}
}