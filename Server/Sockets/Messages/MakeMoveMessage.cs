namespace Server.Sockets.Messages
{
	public class MakeMoveMessage : IReceivedMessage
	{
		public int X { get; set; }
		public int Y { get; set; }
	}
}