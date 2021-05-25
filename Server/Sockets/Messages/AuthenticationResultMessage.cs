namespace Server.Sockets.Messages
{
	public class AuthenticationResultMessage : ISendMessage
	{
		public bool IsSuccess { get; set; }
		public string ErrorMessage { get; set; }
	}
}