namespace Aeternum.Daemon.IPC.Packets.Incoming
{
	public struct IncomingPacket
	{
		public IncomingOpCode OpCode { get; set; }
		public string Data { get; set; }
	}
}