using System.Linq;
using System.Collections.Generic;
using Chess.Pieces;
using Chess.Board;

namespace Chess.Movement
{
	///<summary>
	///Responsible for calculating positive diagonal move of a bishop and a queen.
	///</summary>
	public class PositiveDiagonalMovement : IMovement
	{
		private const int boardSize = 8;
		private readonly IChessBoard chessBoard;
		public PositiveDiagonalMovement(IChessBoard chessBoard)
		{
			this.chessBoard = chessBoard;
		}
		public IEnumerable<ChessMove> GetAvailableMoves(IReadOnlyChessPiece chessPiece)
		{
			var availableMoves = new List<ChessMove>();
			if (chessPiece.PieceType != ChessPieceType.Bishop &&
				chessPiece.PieceType != ChessPieceType.Queen)
			{
				return availableMoves;
			}

			foreach (var position in GetLeftLowerPositions(chessPiece)
				.Union(GetRightUpperPositions(chessPiece)))
			{
				var chessMove = new ChessMove(chessPiece.Position, position);
				availableMoves.Add(chessMove);
			}

			return availableMoves;
		}
		private List<Position> GetLeftLowerPositions(IReadOnlyChessPiece chessPiece)
		{
			var positions = new List<Position>();
			int x = chessPiece.Position.X - 1;
			int y = chessPiece.Position.Y - 1;
			for (; x >= 0 && y >= 0; x--, y--)
			{
				var position = new Position(x, y);
				if (AddPositionToList(chessPiece, position, positions))
				{
					break;
				}
			}
			return positions;
		}
		private List<Position> GetRightUpperPositions(IReadOnlyChessPiece chessPiece)
		{
			var positions = new List<Position>();
			int x = chessPiece.Position.X + 1;
			int y = chessPiece.Position.Y + 1;
			for (; x < boardSize && y < boardSize; x++, y++)
			{
				var position = new Position(x, y);
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