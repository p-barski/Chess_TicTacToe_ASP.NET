namespace Server.Sockets.Messages
{
	public class PawnPromotionMessage : IReceivedMessage
	{
		public string PromotionPiece { get; set; }
	}
}