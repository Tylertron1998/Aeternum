using Aeternum.Daemon.Monitoring.Logging;

namespace Aeternum.Daemon.Logs
{
	public struct RestartLog : ILog
	{
		public LogLevel Level => LogLevel.Information;
		public string Message { get; }

		public RestartLog(string name, bool stable, int attempts, int maxAttempts)
		{
			Message = $"{(stable ? "UNSTABLE " : "")}RESTART: {name} Attempts: {attempts}/{maxAttempts}";
		}
	}
}