namespace Server.Games.Chess
{
	public class ChessGameMove : IGameMove
	{
		public int X_StartPosition { get; }
		public int Y_StartPosition { get; }
		public int X_FinishedPosition { get; }
		public int Y_FinishedPosition { get; }
		public ChessGameMove(int xStartPosition, int yStartPosition,
			int xFinishedPosition, int yFinishedPosition)
		{
			X_StartPosition = xStartPosition;
			Y_StartPosition = yStartPosition;
			X_FinishedPosition = xFinishedPosition;
			Y_FinishedPosition = yFinishedPosition;
		}
	}
}