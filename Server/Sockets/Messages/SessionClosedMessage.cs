namespace Server.Sockets.Messages
{
	public class SessionClosedMessage : ISendMessage
	{
		public string Reason { get; set; }
		public SessionClosedMessage() { }
		public SessionClosedMessage(string reason)
		{
			Reason = reason;
		}
	}
}