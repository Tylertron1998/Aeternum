namespace Aeternum.Daemon.Monitoring.Logging
{
	public interface ILog
	{
		LogLevel Level { get; }
		string Message { get; }
	}
}