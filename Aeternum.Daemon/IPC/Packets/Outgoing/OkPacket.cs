namespace Aeternum.Daemon.IPC.Packets.Outgoing
{
	public struct OkPacket
	{
		public OutgoingOpCode OpCode => OutgoingOpCode.Ok;
	}
}