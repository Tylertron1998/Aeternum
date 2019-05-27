using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Aeternum.Daemon.Logs;
using Aeternum.Daemon.Monitoring.Logging;
using Aeternum.Utililty;

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

        public void AddMonitor(string name, string command, string file, string flags, bool shouldSave,
            bool captureStdin, bool captureStdout, int maxLogs, int maxErrorLogs,
            int maxRetries , int minStableUptime)
        {
            var possible = FindMonitor(name);

            if (possible != null)
            {
                throw new Exception($"A monitor with the name {name} already exists.");
            }

            var settings = new MonitorSettings
            {
                Name = name,
                Command = command,
                FileName = file,
                Flags = flags,
                ShouldLog = captureStdin,
                ShouldLogError = captureStdout,
                MaxLogs = maxLogs,
                MaxErrorLogs = maxErrorLogs,
                MaxRetries = maxRetries,
                MinimumUptime = minStableUptime
            };
            
            _monitors.Add(new Monitor(settings));

            if (shouldSave)
            {
                SaveMonitorSettings(settings);
            }
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
                            Log(new RestartLog(monitor.Settings.Command, monitor.Stable,
                                monitor.CurrentRetries, monitor.CurrentRetries));
                            monitor.Restart();
                        }

                        Log(new MaxRestartLog(monitor.Settings.Command));
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

        public void StartMonitor(string name)
        {
            var possible = FindMonitor(name);

            if (possible == null)
            {
                throw new Exception(
                    $"The monitor with the name {name} does not exist. Create it with 'aeternum create'");
            }
        }
        
        public void EditMonitorSettings(string name, bool save, string? command = null, string? file = null, string? flags = null, bool? shouldSave = null,
            bool? captureStdin = null, bool? captureStdout = null, int? maxLogs = null, int? maxErrorLogs = null,
            int? maxRetries = null, int? minStableUptime = null)
        {
            var possible = FindMonitor(name);

            if (possible == null)
            {
                throw new Exception($"A monitor with the name {name} does not exists.");
            }

            var clone = possible.Settings;

            var settings = new MonitorSettings
            {
                Name = name,
                Command = command ?? clone.Command,
                FileName = file ?? clone.FileName,
                Flags = flags ?? clone.Flags,
                ShouldLog = captureStdin ?? clone.ShouldLog,
                ShouldLogError = captureStdout ?? clone.ShouldLogError,
                MaxLogs = maxLogs ?? clone.MaxLogs,
                MaxErrorLogs = maxErrorLogs ?? clone.MaxErrorLogs,
                MaxRetries = maxRetries ?? clone.MaxRetries,
                MinimumUptime = minStableUptime ?? clone.MinimumUptime
            };

            possible.Settings = settings;

            if (save)
            {
                SaveMonitorSettings(settings);
            }
        }

        public void StopMonitor(string name)
        {
            var possible = FindMonitor(name);

            possible?.Stop();

            throw new Exception($"Cannot find monitor with name {name}.");
        }

        public void RestartMonitor(string name)
        {
            var possible = FindMonitor(name);

            possible?.Restart();

            throw new Exception($"Cannot find monitor with name {name}.");
        }

        public List<string> GetLogs(LogLevel filter, int maxResults)
        {
            return _logs.Where(log => (log.Level & filter) == log.Level)
                .Take(maxResults).Select(log => $"{log.Level.ToString().ToUpper()}::{log.Message}").ToList();
        }

        public MonitorLogs GetMonitorLogs(string name)
        {
            var possible = FindMonitor(name);

            if (possible == null) throw new Exception($"Cannot find monitor with name {name}.");
            if (!possible.Settings.ShouldLog && possible.Settings.ShouldLogError)
            {
                throw new Exception($"The monitor {name} is set to ignore both stdin and stdout.");
            }

            return possible.Logs;
        }


        private Monitor FindMonitor(string name)
        {
            return _monitors.FirstOrDefault(monitor => monitor.Settings.Name == name);
        }

        private void SaveMonitorSettings(MonitorSettings settings)
        {
            var path = Path.Combine(Path.GetTempPath(), "aeternum", "monitors", $"{settings.Name}.monitor");

            Util.EnsureDirectoryExists(path);

            var stream = File.Create(path);
            
            var writer = new BinaryFormatter();
            
            writer.Serialize(stream, settings);
            stream.Close();
        }

        private List<MonitorSettings> LoadAllMonitors()
        {
            var path = Path.Combine(Path.GetTempPath(), "aeternum", "monitors");

            Util.EnsureDirectoryExists(path);

            var settings = new List<MonitorSettings>();

            foreach (var file in Directory.GetFiles(path))
            {
                if (!file.EndsWith(".monitor")) continue;
                
                var stream = File.OpenRead(path);
                
                var reader = new BinaryFormatter();

                var settingsInstance = (MonitorSettings) reader.Deserialize(stream);

                if (settings.Any(setting => setting.Name == settingsInstance.Name))
                {
                    Log(new ErrorLog($"The file {file} seems to contain a duplicate monitor settings instance ({settingsInstance.Name}). Skipping..."));
                }
                else
                {
                    settings.Add(settingsInstance);
                }
            }

            return settings;

        }

        public List<string> ListMonitors()
        {
            return _monitors.Select(monitor => monitor.ToString()).ToList();
        }
    }
}