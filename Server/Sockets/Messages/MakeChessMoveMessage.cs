namespace Server.Sockets.Messages
{
	public class MakeChessMoveMessage : IReceivedMessage
	{
		public int X_StartPosition { get; set; }
		public int Y_StartPosition { get; set; }
		public int X_FinishedPosition { get; set; }
		public int Y_FinishedPosition { get; set; }
	}
}