using System;
using System.Diagnostics;

namespace Aeternum.Daemon.Monitoring
{
	public class Monitor
	{
		public MonitorSettings Settings { get; }
		public int CurrentRetries { get; private set; }

		public DateTime StartTime { get; private set; }
		public int Id { get; private set; }
		public string Logs { get; private set; }

		public Monitor(string name, string fileName, string flags = "", string userName = "", int maxRetries = 10,
			int minimumUptime = 100,
			bool shouldLog = true)
		{
			Settings = new MonitorSettings
			{
				Name = name,
				FileName = fileName,
				Flags = flags,
				UserName = userName,
				MaxRetries = maxRetries,
				MinimumUptime = minimumUptime,
				ShouldLog = shouldLog,
			};
		}

		public bool IsRunning
		{
			get
			{
				try
				{
					Process.GetProcessById(Id);
					return true;
				}
				catch
				{
					return false;
				}
			}
		}

		public double Uptime => (DateTime.Now - StartTime).TotalMilliseconds;
		public bool Stable => Uptime > Settings.MinimumUptime;

		public bool ShouldRestart => !_stopped && CurrentRetries < Settings.MaxRetries && Stable;

		private bool _stopped;

		public void Restart()
		{
			Start(StartTime);

			if (Stable)
			{
				CurrentRetries = 0;
			}
			else
			{
				CurrentRetries++;
			}
		}

		public void Reset()
		{
			CurrentRetries = 0;
			Logs = "";
		}

		public void Stop()
		{
			_stopped = true;
			Process.GetProcessById(Id).Kill();
		}

		public void Start(DateTime? startTime = null)
		{
			_stopped = false;
			var process = Process.Start(new ProcessStartInfo
			{
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				FileName = Settings.Name,
				Arguments = Settings.Flags
			});
			StartTime = startTime ?? DateTime.Now;
			Id = process.Id;

			if (Settings.ShouldLog)
			{
				process.OutputDataReceived += (sender, args) => Logs += args.Data;
				process.ErrorDataReceived += (sender, args) => Logs += args.Data;
			}
		}
	}
}