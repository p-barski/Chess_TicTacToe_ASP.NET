namespace Server.Sockets.Messages
{
	public class FindGameMessage : IReceivedMessage
	{
		public int Size { get; set; }
	}
}