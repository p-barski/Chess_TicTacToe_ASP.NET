namespace Server.Sockets.Messages
{
	public class IncorrectMessage : IReceivedMessage
	{
		public string Json { get; set; }
	}
}