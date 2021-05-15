namespace Server.Sockets.Messages
{
	public class FindChessGameMessage : IReceivedMessage
	{
		public bool ChessGame { get; set; }
	}
}