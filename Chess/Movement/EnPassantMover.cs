using System;
using System.Linq;
using System.Collections.Generic;
using Chess.Pieces;

namespace Chess.Movement
{
	public class EnPassantMover : IEnPassantMover
	{
		private readonly IMovementHistory history;
		public EnPassantMover(IMovementHistory history)
		{
			this.history = history;
		}
		public IChessPiece PerformEnPassantIfApplicable(ChessMove chessMove,
			IEnumerable<IChessPiece> pieces)
		{
			if (chessMove.PawnPromotion != ChessPieceType.Pawn)
			{
				return null;
			}
			var pieceToMove = pieces
				.First(p => p.Position == chessMove.StartingPosition);
			if (pieceToMove.PieceType != ChessPieceType.Pawn)
			{
				return null;
			}
			if (!IsDiagonalMove(chessMove))
			{
				return null;
			}
			if (IsFinishedPositionTaken(chessMove, pieces))
			{
				return null;
			}

			pieceToMove.IncrementMoveCounter();
			pieceToMove.Position = chessMove.FinishedPosition;
			history.Add(chessMove.ReturnWithCaptureAsTrue());

			return FindPawnToRemove(chessMove, pieces);
		}
		private bool IsDiagonalMove(ChessMove chessMove)
		{
			int xdiff = chessMove.FinishedPosition.X - chessMove.StartingPosition.X;
			int ydiff = chessMove.FinishedPosition.Y - chessMove.StartingPosition.Y;
			return Math.Abs(xdiff) == Math.Abs(ydiff);
		}
		private bool IsFinishedPositionTaken(ChessMove chessMove,
			IEnumerable<IChessPiece> pieces)
		{
			return pieces
				.Any(p => p.Position == chessMove.FinishedPosition);
		}
		private IChessPiece FindPawnToRemove(ChessMove chessMove,
			IEnumerable<IChessPiece> pieces)
		{
			int x = chessMove.FinishedPosition.X;
			int y = chessMove.StartingPosition.Y;
			var position = new Position(x, y);

			return pieces
				.First(p => p.Position == position);
		}
	}
}