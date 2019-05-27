using System;
using System.Threading.Tasks;
using Aeternum.Daemon.IPC;
using Aeternum.Daemon.Monitoring;

namespace Aeternum.Daemon
{
	public static class Program
	{
		public static async Task Main(string[] args)
		{
			var manager = new MonitorManager(Int32.Parse(args[0]));
			var ipc = new IpcManager(manager);

			ipc.Start();
			await manager.StartAsync();
		}
	}
}