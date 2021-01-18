namespace Server.Sockets.Messages
{
	public class SessionClosedMessage : IMessage
	{
		public string Reason { get; set; }
		public SessionClosedMessage() { }
		public SessionClosedMessage(string reason)
		{
			Reason = reason;
		}
	}
}