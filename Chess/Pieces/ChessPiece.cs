namespace Chess.Pieces
{
	public class ChessPiece : IChessPiece
	{
		public ChessPieceType PieceType { get; }
		public ChessColor Color { get; }
		public Position Position { get; set; }
		public bool HasMoved { get => moveCounter > 0; }
		private int moveCounter = 0;
		public ChessPiece(ChessPieceType pieceType, ChessColor color, Position position)
		{
			PieceType = pieceType;
			Color = color;
			Position = position;
		}
		public void IncrementMoveCounter()
		{
			moveCounter++;
		}
		public void DecrementMoveCounter()
		{
			moveCounter--;
		}
	}
}