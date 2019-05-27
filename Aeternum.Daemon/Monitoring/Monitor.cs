using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using Aeternum.Daemon.Monitoring.Logging;
using Microsoft.VisualBasic;

namespace Aeternum.Daemon.Monitoring
{
	public class Monitor
	{
		public MonitorSettings Settings { get; set; }
        public int CurrentRetries { get; private set; }

		public DateTime StartTime { get; private set; }
		public int Id { get; private set; }
        
        public MonitorLogs Logs { get; private set; }

		public Monitor(MonitorSettings settings)
        {
            Settings = settings;

            Logs = new MonitorLogs
            {
                StandardOut = new List<string>(),
                StandardError = new List<string>()
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

		public bool ShouldRestart => !Stopped && CurrentRetries < Settings.MaxRetries && Stable;

        public bool Stopped { get; private set; }
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
            Logs.StandardOut.Clear();
            Logs.StandardError.Clear();
        }

		public void Stop()
		{
			Stopped = true;
			Process.GetProcessById(Id).Kill();
            Reset();
        }

		public void Start(DateTime? startTime = null)
		{
			Stopped = false;
			var process = Process.Start(new ProcessStartInfo
			{
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				FileName = Settings.Command,
				Arguments = Settings.Flags
			});
			StartTime = startTime ?? DateTime.Now;
			Id = process.Id;

			if (Settings.ShouldLog)
			{
				process.OutputDataReceived += (sender, args) => Logs.StandardOut.Add(args.Data);
            }

            if (Settings.ShouldLogError)
            {
                process.ErrorDataReceived += (sender, args) => Logs.StandardError.Add(args.Data);
            }
		}

        public override string ToString()
        {
            var process = Process.GetProcessById(Id);

            var currentCpuUsage = (process.TotalProcessorTime.TotalMilliseconds / Environment.ProcessorCount) * 100;
            var currentMemory = process.WorkingSet64;

            var dividedCpu = currentCpuUsage / 10;

            var builder = new StringBuilder();

            builder.AppendLine(Settings.Name);

            builder.AppendLine($"CPU: {currentCpuUsage} [{new String('=', (int)dividedCpu)}{new String(' ', 100 - (int)dividedCpu)}]");
            builder.AppendLine($"Memory: {currentMemory * 1024 * 1024}MB");
            builder.AppendLine($"Restarts: {CurrentRetries} (Max {Settings.MaxRetries})");
            builder.AppendLine($"Uptime: {Uptime}");
            builder.AppendLine($"Flags: {Settings.Flags}");
            builder.AppendLine($"Logs: {Logs.StandardOut.Count} (Error Logs): {Logs.StandardError.Count}");


            return builder.ToString();

        }
    }
}