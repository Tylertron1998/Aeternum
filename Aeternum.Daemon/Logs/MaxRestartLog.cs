using Aeternum.Daemon.Monitoring.Logging;

namespace Aeternum.Daemon.Logs
{
	public struct MaxRestartLog : ILog
	{
		public LogLevel Level => LogLevel.Warning;
		public string Message { get; }

		public MaxRestartLog(string name)
		{
			Message = $"MAX RESTARTS: {name} hit max unstable restarts, aborting auto-restart.";
		}
	}
}