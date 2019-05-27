using Aeternum.Daemon.Monitoring.Logging;

namespace Aeternum.Daemon.Logs
{
	public struct ErrorLog : ILog
	{
		public LogLevel Level => LogLevel.Error;
		public string Message { get; }

		public ErrorLog(string message)
		{
			Message = message;
		}
	}
}