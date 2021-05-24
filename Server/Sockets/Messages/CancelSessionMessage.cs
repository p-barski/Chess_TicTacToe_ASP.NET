namespace Server.Sockets.Messages
{
	public class CancelSessionMessage : IReceivedMessage
	{
		public bool Cancel { get; set; }
	}
}