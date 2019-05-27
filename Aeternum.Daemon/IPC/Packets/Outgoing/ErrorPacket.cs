namespace Aeternum.Daemon.IPC.Packets.Outgoing
{
	public struct ErrorPacket
	{
		public string Error { get; set; }
		public OutgoingOpCode OpCode => OutgoingOpCode.Error;
	}
}