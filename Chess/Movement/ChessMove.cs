using Chess.Pieces;

namespace Chess.Movement
{
	public struct ChessMove
	{
		public Position StartingPosition { get; }
		public Position FinishedPosition { get; }
		public bool IsCapture { get; }
		///<summary>
		///Pawn when it's normal move, other piece type when this is promotion move.
		///</summary>
		public ChessPieceType PawnPromotion { get; }
		public ChessMove(
			Position startingPosition,
			Position finishedPosition,
			bool isCapture = false,
			ChessPieceType pawnPromotion = ChessPieceType.Pawn)
		{
			StartingPosition = startingPosition;
			FinishedPosition = finishedPosition;
			IsCapture = isCapture;
			PawnPromotion = pawnPromotion;
		}
		///<summary>
		///Creates new ChessMove with the same properties, except IsCapture is true.
		///</summary>
		public ChessMove ReturnWithCaptureAsTrue()
		{
			return new ChessMove(StartingPosition, FinishedPosition, true, PawnPromotion);
		}
		public override string ToString()
		{
			return $"{StartingPosition}->{FinishedPosition}, {IsCapture}, {PawnPromotion}";
		}
	}
}