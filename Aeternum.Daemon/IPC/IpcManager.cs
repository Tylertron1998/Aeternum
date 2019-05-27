using System;
using Aeternum.Daemon.IPC.Packets.Incoming;
using Aeternum.Daemon.IPC.Packets.Outgoing;
using Aeternum.Daemon.Monitoring;
using Newtonsoft.Json;
using ZetaIpc.Runtime.Server;

namespace Aeternum.Daemon.IPC
{
	public class IpcManager
	{
		private readonly IpcServer _ipc = new IpcServer();

		private readonly MonitorManager _manager;

		public IpcManager(MonitorManager manager)
		{
			_manager = manager;
		}

		public void Start(int port = 255)
		{
			_ipc.Start(port);

			_ipc.ReceivedRequest += HandleRequest;
		}

		private void HandleRequest(object sender, ReceivedRequestEventArgs request)
		{
			var packet = JsonConvert.DeserializeObject<IncomingPacket>(request.Request);

			switch (packet.OpCode)
			{
				case IncomingOpCode.Start:
				{
					HandleStartRequest(request, packet);
					break;
				}

				case IncomingOpCode.Stop:
				{
					HandleStopRequest(request, packet);
					break;
				}


				case IncomingOpCode.Restart:
				{
					HandleRestartRequest(request, packet);
					break;
				}

				case IncomingOpCode.Info:
				{
					HandleInfoRequest(request, packet);
					break;
				}

				case IncomingOpCode.Log:
				{
					HandleLogRequest(request, packet);
					break;
				}

				default: throw new ArgumentException();
			}
		}

		private void HandleInfoRequest(ReceivedRequestEventArgs request, IncomingPacket packet)
		{
			var infoPacket = JsonConvert.DeserializeObject<MonitorLogsPacket>(packet.Data);

			try
			{
				var logs = _manager.GetMonitorLogs(infoPacket.Name);
				request.Response = JsonConvert.SerializeObject(new LogsPacket
				{
					Logs = logs
				});

				request.Handled = true;
			}
			catch (Exception e)
			{
				request.Response = JsonConvert.SerializeObject(new ErrorPacket
				{
					Error = e.Message
				});
				request.Handled = true;
			}
		}

		private void HandleLogRequest(ReceivedRequestEventArgs request, IncomingPacket packet)
		{
			var infoPacket = JsonConvert.DeserializeObject<LogInfoPacket>(packet.Data);

			var logs = _manager.GetLogs(infoPacket.LevelFilter, infoPacket.MaxResults);
			request.Response = JsonConvert.SerializeObject(new ManagerLogsPacket
			{
				Logs = logs
			});

			request.Handled = true;
		}

		private void HandleRestartRequest(ReceivedRequestEventArgs request, IncomingPacket packet)
		{
			var restartPacket = JsonConvert.DeserializeObject<RestartPacket>(packet.Data);

			try
			{
				_manager.RestartMonitor(restartPacket.Name);
				request.Response = JsonConvert.SerializeObject(new OkPacket());
			}
			catch (Exception e)
			{
				request.Response = JsonConvert.SerializeObject(new ErrorPacket
				{
					Error = e.Message
				});
			}

			request.Handled = true;
		}

		private void HandleStopRequest(ReceivedRequestEventArgs request, IncomingPacket packet)
		{
			var stopPacket = JsonConvert.DeserializeObject<StopPacket>(packet.Data);

			try
			{
				_manager.StopMonitor(stopPacket.Name);
				request.Response = JsonConvert.SerializeObject(new OkPacket());
			}
			catch (Exception e)
			{
				request.Response = JsonConvert.SerializeObject(new ErrorPacket
				{
					Error = e.Message
				});
			}

			request.Handled = true;
		}

		private void HandleStartRequest(ReceivedRequestEventArgs request, IncomingPacket packet)
		{
			var startPacket = JsonConvert.DeserializeObject<StartPacket>(packet.Data);

			if (_manager.HasMonitorWithUserName(startPacket.UserName))
			{
				request.Response = JsonConvert.SerializeObject(new ErrorPacket
				{
					Error = $"A monitor with the user-name {startPacket.UserName} has already been started."
				});
			}

			try
			{
				StartMonitor(startPacket);
				request.Response = JsonConvert.SerializeObject(new OkPacket());
			}
			catch (Exception e)
			{
				request.Response = JsonConvert.SerializeObject(new ErrorPacket
				{
					Error = e.Message
				});
			}

			request.Handled = true;
		}

		private void StartMonitor(StartPacket startPacket)
		{
			_manager.AddMonitor(startPacket);
		}
	}
}