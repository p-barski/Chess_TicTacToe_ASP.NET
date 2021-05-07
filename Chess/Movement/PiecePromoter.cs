using System.Linq;
using System.Collections.Generic;
using Chess.Pieces;

namespace Chess.Movement
{
	public class PiecePromoter : IPiecePromoter
	{
		private readonly IMovementHistory history;
		public PiecePromoter(IMovementHistory history)
		{
			this.history = history;
		}
		public bool PromoteIfPromotionMove(ChessMove chessMove,
			IEnumerable<IChessPiece> pieces)
		{
			if (chessMove.PawnPromotion == ChessPieceType.Pawn)
			{
				return false;
			}
			var pawnPosition = history.ChessMoves[^1].FinishedPosition;
			var pawn = pieces.First(p => p.Position == pawnPosition);
			pawn.Promote(chessMove.PawnPromotion);
			history.Add(chessMove);
			return true;
		}
		public ChessMove DepromoteIfPromotionMove(ChessMove chessMove,
			IEnumerable<IChessPiece> pieces)
		{
			if (chessMove.PawnPromotion == ChessPieceType.Pawn)
			{
				return chessMove;
			}
			chessMove = history.RemoveLastMove();
			var pawnPosition = chessMove.FinishedPosition;
			var pawn = pieces.First(p => p.Position == pawnPosition);
			pawn.Depromote();
			return chessMove;
		}
	}
}