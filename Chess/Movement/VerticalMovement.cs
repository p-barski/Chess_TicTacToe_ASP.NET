using System.Linq;
using System.Collections.Generic;
using Chess.Pieces;
using Chess.Board;

namespace Chess.Movement
{
	///<summary>
	///Responsible for calculating vertical move of a rook and a queen.
	///</summary>
	public class VerticalMovement : IMovement
	{
		private const int boardSize = 8;
		private readonly IChessBoard chessBoard;
		public VerticalMovement(IChessBoard chessBoard)
		{
			this.chessBoard = chessBoard;
		}
		public IEnumerable<ChessMove> GetAvailableMoves(IReadOnlyChessPiece chessPiece)
		{
			var availableMoves = new List<ChessMove>();
			if (chessPiece.PieceType != ChessPieceType.Rook &&
				chessPiece.PieceType != ChessPieceType.Queen)
			{
				return availableMoves;
			}

			foreach (var position in GetLowerPositions(chessPiece)
				.Union(GetUpperPositions(chessPiece)))
			{
				var chessMove = new ChessMove(chessPiece.Position, position);
				availableMoves.Add(chessMove);
			}

			return availableMoves;
		}
		private List<Position> GetLowerPositions(IReadOnlyChessPiece chessPiece)
		{
			var positions = new List<Position>();
			for (int y = chessPiece.Position.Y - 1; y >= 0; y--)
			{
				var position = new Position(chessPiece.Position.X, y);
				if (AddPositionToList(chessPiece, position, positions))
				{
					break;
				}
			}
			return positions;
		}
		private List<Position> GetUpperPositions(IReadOnlyChessPiece chessPiece)
		{
			var positions = new List<Position>();
			for (int y = chessPiece.Position.Y + 1; y < boardSize; y++)
			{
				var position = new Position(chessPiece.Position.X, y);
				if (AddPositionToList(chessPiece, position, positions))
				{
					break;
				}
			}
			return positions;
		}
		private bool AddPositionToList(IReadOnlyChessPiece chessPiece,
			Position position, List<Position> positions)
		{
			var isEnemy = chessBoard
				.IsEnemyOnPosition(position, chessPiece.Color.Opposite());

			if (isEnemy)
			{
				positions.Add(position);
				return true;
			}
			if (chessBoard.IsPositionTaken(position))
			{
				return true;
			}
			positions.Add(position);
			return false;
		}
	}
}