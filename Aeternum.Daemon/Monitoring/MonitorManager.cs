using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aeternum.Daemon.IPC.Packets.Incoming;
using Aeternum.Daemon.Logs;
using Aeternum.Daemon.Monitoring.Logging;

namespace Aeternum.Daemon.Monitoring
{
	public class MonitorManager
	{
		public int MaxLogs { get; }

		private List<Monitor> _monitors = new List<Monitor>();

		private Queue<ILog> _logs = new Queue<ILog>();

		public MonitorManager(int maxLogs = 100)
		{
			MaxLogs = maxLogs;
		}

		public bool HasMonitorWithUserName(string name) =>
			_monitors.Any(m => m.Settings.UserName == name && m.IsRunning);

		public void AddMonitor(StartPacket packet)
		{
			try
			{
				var monitor = new Monitor(packet.Name, packet.FileName, packet.Args, packet.UserName, packet.MaxRetries,
					packet.MinimumUptime);
				_monitors.Add(monitor);
			}
			catch (Exception e)
			{
				Log(new ErrorLog(e.Message));
				throw;
			}
		}

		public void StopMonitor(string name)
		{
			var possible = _monitors.First(monitor =>
				monitor.Settings.FileName == name || monitor.Settings.UserName == name);

			if (possible != null)
			{
				possible.Stop();
			}

			throw new Exception($"Cannot find monitor with name {name}.");
		}

		public async Task StartAsync()
		{
			await Task.Run(() =>
			{
				while (true)
				{
					foreach (var monitor in _monitors)
					{
						if (monitor.IsRunning) continue;
						if (monitor.ShouldRestart)
						{
							Log(new RestartLog(monitor.Settings.Name, monitor.Stable,
								monitor.CurrentRetries, monitor.CurrentRetries));
							monitor.Restart();
						}

						Log(new MaxRestartLog(monitor.Settings.Name));
					}
				}
				// ReSharper disable once FunctionNeverReturns
			});
		}

		private void Log(ILog log)
		{
			if (_logs.Count > MaxLogs)
			{
				_logs.Dequeue();
			}

			_logs.Enqueue(log);
		}

		public void RestartMonitor(string name)
		{
			var possible = _monitors.First(monitor =>
				monitor.Settings.FileName == name || monitor.Settings.UserName == name);

			possible?.Restart();

			throw new Exception($"Cannot find monitor with name {name}.");
		}

		public List<string> GetLogs(LogLevel filter, int maxResults)
		{
			return _logs.Where(log => (log.Level & filter) == log.Level)
				.Take(maxResults).Select(log => log.Message).ToList();
		}

		public string GetMonitorLogs(string name)
		{
			var possible = _monitors.First(monitor =>
				monitor.Settings.FileName == name || monitor.Settings.UserName == name);

			if (possible == null) throw new Exception($"Cannot find monitor with name {name}.");
			if (possible.Settings.ShouldLog)
			{
				return possible.Logs;
			}

			throw new Exception($"Monitor {name} is set to ignore logs.");
		}
	}
}