namespace Server.Sockets.Messages
{
	public class IncorrectMessage : IMessage
	{
		public string Json { get; set; }
	}
}