namespace Server.Sockets.Messages
{
	public class InvalidStateMessage : ISendMessage
	{
		public string Message { get; set; }
		public InvalidStateMessage() { }
		public InvalidStateMessage(string message)
		{
			Message = message;
		}
	}
}