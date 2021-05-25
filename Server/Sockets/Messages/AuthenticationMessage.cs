namespace Server.Sockets.Messages
{
	public class AuthenticationMessage : IReceivedMessage
	{
		public bool Registration { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
	}
}