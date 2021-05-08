using System;
using System.Linq;
using System.Collections.Generic;
using Chess.Pieces;

namespace Chess.Movement
{
	public class CastlingMover : ICastlingMover
	{
		private readonly IMovementHistory history;
		public CastlingMover(IMovementHistory history)
		{
			this.history = history;
		}
		public bool PerformCastlingIfCastlingMove(ChessMove chessMove,
			IEnumerable<IChessPiece> pieces)
		{
			if (chessMove.PawnPromotion != ChessPieceType.Pawn)
			{
				return false;
			}

			var movedPiece = pieces
				.First(p => p.Position == chessMove.StartingPosition);

			if (movedPiece.PieceType != ChessPieceType.King)
			{
				return false;
			}
			if (CalculateDifference(chessMove.StartingPosition, chessMove.FinishedPosition) == 1)
			{
				return false;
			}

			var rook = pieces
				.First(p => p.Position == chessMove.FinishedPosition);

			rook.Position = CalculateRookPositionAfterCastling(rook.Position,
				movedPiece.Position);
			movedPiece.Position = CalculateKingPositionAfterCastling(rook.Position,
				movedPiece.Position);

			rook.IncrementMoveCounter();
			movedPiece.IncrementMoveCounter();
			history.Add(chessMove);
			return true;
		}
		public bool UndoCastlingIfCastlingMove(ChessMove chessMove,
			IEnumerable<IChessPiece> pieces)
		{
			if (chessMove.PawnPromotion != ChessPieceType.Pawn)
			{
				return false;
			}

			var movedPiece = pieces
				.FirstOrDefault(p => p.Position == chessMove.FinishedPosition);

			if (movedPiece != null)
			{
				return false;
			}

			var rookCurrentPosition = CalculateRookPositionAfterCastling(
				chessMove.FinishedPosition, chessMove.StartingPosition);
			var kingCurrentPosition = CalculateKingPositionAfterCastling(
				chessMove.FinishedPosition, chessMove.StartingPosition);

			var rook = pieces
				.First(p => p.Position == rookCurrentPosition);
			var king = pieces
				.First(p => p.Position == kingCurrentPosition);

			rook.Position = chessMove.FinishedPosition;
			king.Position = chessMove.StartingPosition;

			rook.DecrementMoveCounter();
			king.DecrementMoveCounter();

			return true;
		}
		private int CalculateDifference(Position a, Position b)
		{
			int xdiff = Math.Abs(a.X - b.X);
			int ydiff = Math.Abs(a.Y - b.Y);
			if (xdiff > ydiff)
			{
				return xdiff;
			}
			return ydiff;
		}
		private Position CalculateKingPositionAfterCastling(Position rookPosition,
			Position kingPosition)
		{
			if (rookPosition.X < kingPosition.X)
			{
				//queenside
				return new Position(kingPosition.X - 2, kingPosition.Y);
			}
			//kingside
			return new Position(kingPosition.X + 2, kingPosition.Y);
		}
		private Position CalculateRookPositionAfterCastling(Position rookPosition,
			Position kingPosition)
		{
			if (rookPosition.X < kingPosition.X)
			{
				//queenside
				return new Position(rookPosition.X + 3, rookPosition.Y);
			}
			//kingside
			return new Position(rookPosition.X - 2, rookPosition.Y);
		}
	}
}