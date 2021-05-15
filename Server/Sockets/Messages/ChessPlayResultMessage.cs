namespace Server.Sockets.Messages
{
	public class ChessPlayResultMessage : ISendMessage
	{
		public string Message { get; set; }
		public bool IsClientTurn { get; set; }
		public bool IsPromotionRequired { get; set; }
	}
}