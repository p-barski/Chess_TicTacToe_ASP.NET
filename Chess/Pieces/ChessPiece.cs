namespace Chess.Pieces
{
	public class ChessPiece : IChessPiece
	{
		public ChessPieceType PieceType { get; private set; }
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
		public void Promote(ChessPieceType promotionPiece)
		{
			PieceType = promotionPiece;
		}
		public void Depromote()
		{
			PieceType = ChessPieceType.Pawn;
		}
	}
}