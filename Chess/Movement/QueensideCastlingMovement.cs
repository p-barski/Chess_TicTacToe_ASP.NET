using System.Linq;
using System.Collections.Generic;
using Chess.Pieces;
using Chess.Board;

namespace Chess.Movement
{
	///<summary>
	///Responsible for calculating a possibility of queenside castling for a king.
	///</summary>
	public class QueensideCastlingMovement : IMovement
	{
		private const int boardSize = 8;
		private const int queensideRookXPosition = 0;
		private readonly IChessBoard chessBoard;
		private readonly IMovement movement;
		public QueensideCastlingMovement(IChessBoard chessBoard, IMovement movement)
		{
			this.chessBoard = chessBoard;
			this.movement = movement;
		}
		public IEnumerable<ChessMove> GetAvailableMoves(IReadOnlyChessPiece chessPiece)
		{
			var availableMoves = new List<ChessMove>();
			if (chessPiece.PieceType != ChessPieceType.King)
			{
				return availableMoves;
			}

			if (chessPiece.HasMoved)
			{
				return availableMoves;
			}

			var rook = GetQueensideRook(chessPiece);
			if (rook == null)
			{
				return availableMoves;
			}

			if (!ArePositionsEmpty(chessPiece))
			{
				return availableMoves;
			}

			if (ArePositionsBeingAttacked(chessPiece))
			{
				return availableMoves;
			}

			var castlingMove = new ChessMove(chessPiece.Position, rook.Position);
			availableMoves.Add(castlingMove);

			return availableMoves;
		}
		private IReadOnlyChessPiece GetQueensideRook(IReadOnlyChessPiece king)
		{
			return chessBoard.Pieces
				.FirstOrDefault(p =>
					p.Color == king.Color &&
					p.PieceType == ChessPieceType.Rook &&
					p.HasMoved == false &&
					p.Position == new Position(queensideRookXPosition, king.Position.Y));
		}
		private bool ArePositionsEmpty(IReadOnlyChessPiece king)
		{
			int y = king.Position.Y;
			for (int x = king.Position.X - 1; x > 0; x--)
			{
				if (chessBoard.IsPositionTaken(new Position(x, y)))
				{
					return false;
				}
			}
			return true;
		}
		private bool ArePositionsBeingAttacked(IReadOnlyChessPiece king)
		{
			int y = king.Position.Y;
			int lastX = king.Position.X - 2;
			var enemyPieces = chessBoard.Pieces
				.Where(p => p.Color == king.Color.Opposite())
				.ToList();

			for (int x = king.Position.X; x >= lastX; x--)
			{
				var position = new Position(x, y);
				foreach (var piece in enemyPieces)
				{
					var isAttacked = movement
						.GetAvailableMoves(piece)
						.Any(m => m.FinishedPosition == position);
					if (isAttacked)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}