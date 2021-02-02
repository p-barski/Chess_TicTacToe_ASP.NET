namespace Server.Sockets.Messages
{
	public class MoveResultMessage : ISendMessage
	{
		public int X { get; set; }
		public int Y { get; set; }
		public string Message { get; set; }
		public MoveResultMessage() { }
		public MoveResultMessage(string message, int x = 0, int y = 0)
		{
			X = x;
			Y = y;
			Message = message;
		}
	}
}