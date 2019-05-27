using System;
using System.Linq;
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

                case IncomingOpCode.Add:
                {
                    HandleAddRequest(request, packet);
                    break;
                }

                case IncomingOpCode.Edit:
                {
                    HandleEditRequest(request, packet);
                    break;
                }

                case IncomingOpCode.List:
                {
                    HandleListRequest(request, packet);
                    break;
                }

                default: throw new ArgumentException();
            }
        }

        private void HandleListRequest(ReceivedRequestEventArgs request, IncomingPacket packet)
        {
            try
            {
                var monitors = _manager.ListMonitors();

                request.Response = JsonConvert.SerializeObject(new ListPacket
                {
                    Monitors = monitors
                });
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

        private void HandleEditRequest(ReceivedRequestEventArgs request, IncomingPacket packet)
        {
            var editPacket = JsonConvert.DeserializeObject<EditRequestPacket>(packet.Data);

            try
            {
                _manager.EditMonitorSettings(editPacket.Name, editPacket.Save, editPacket.Command, editPacket.FileName,
                    editPacket.Flags, editPacket.ShouldSave,
                    editPacket.CaptureStandardIn, editPacket.CaptureStandardOut, editPacket.MaxLogs,
                    editPacket.MaxErrorLogs, editPacket.MaxRetries, editPacket.MinimumStableUptime);
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

        private void HandleAddRequest(ReceivedRequestEventArgs request, IncomingPacket packet)
        {
            var addPacket = JsonConvert.DeserializeObject<AddRequestPacket>(packet.Data);

            try
            {
                _manager.AddMonitor(addPacket.Name, addPacket.Command, addPacket.FileName, addPacket.Flags,
                    addPacket.ShouldSave, addPacket.CaptureStandardIn, addPacket.CaptureStandardOut, addPacket.MaxLogs,
                    addPacket.MaxErrorLogs, addPacket.MaxRetries, addPacket.MinimumStableUptime);
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

        private void HandleInfoRequest(ReceivedRequestEventArgs request, IncomingPacket packet)
        {
            var infoPacket = JsonConvert.DeserializeObject<MonitorLogsRequestPacket>(packet.Data);

            try
            {
                var logs = _manager.GetMonitorLogs(infoPacket.Name);
                request.Response = JsonConvert.SerializeObject(new MonitorLogsPacket
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
            var infoPacket = JsonConvert.DeserializeObject<LogInfoRequestPacket>(packet.Data);

            var logs = _manager.GetLogs(infoPacket.LevelFilter, infoPacket.MaxResults);
            request.Response = JsonConvert.SerializeObject(new ManagerLogsPacket
            {
                Logs = logs
            });

            request.Handled = true;
        }

        private void HandleRestartRequest(ReceivedRequestEventArgs request, IncomingPacket packet)
        {
            var restartPacket = JsonConvert.DeserializeObject<RestartRequestPacket>(packet.Data);

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
            var stopPacket = JsonConvert.DeserializeObject<StopRequestPacket>(packet.Data);

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
            var startPacket = JsonConvert.DeserializeObject<StartRequestPacket>(packet.Data);

            try
            {
                _manager.StartMonitor(startPacket.Name);
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

    }
}