using System.Collections.Generic;
using Chess.Pieces;
using Chess.Board;

namespace Chess.Movement
{
	///<summary>
	///Responsible for calculating available legal or illegal moves of a king.
	///</summary>
	public class KingMovement : IMovement
	{
		private const int boardSize = 8;
		private readonly IChessBoard chessBoard;
		public KingMovement(IChessBoard chessBoard)
		{
			this.chessBoard = chessBoard;
		}
		public IEnumerable<ChessMove> GetAvailableMoves(IReadOnlyChessPiece chessPiece)
		{
			var availableMoves = new List<ChessMove>();
			if (chessPiece.PieceType != ChessPieceType.King)
			{
				return availableMoves;
			}

			foreach (var position in GetNearestPositions(chessPiece.Position))
			{
				var isEnemyPresent = chessBoard.IsEnemyOnPosition(position, chessPiece.Color.Opposite());
				if (isEnemyPresent)
				{
					availableMoves.Add(new ChessMove(chessPiece.Position, position, true));
					continue;
				}
				var isPositionTaken = chessBoard.IsPositionTaken(position);
				if (!isPositionTaken)
				{
					availableMoves.Add(new ChessMove(chessPiece.Position, position));
				}
			}
			//TODO castling
			return availableMoves;
		}
		private List<Position> GetNearestPositions(Position position)
		{
			var positions = new List<Position>();
			for (int x = CalculateStartingX(position); x <= CalculateLastX(position); x++)
			{
				for (int y = CalculateStartingY(position); y <= CalculateLastY(position); y++)
				{
					if (x == position.X && y == position.Y)
					{
						continue;
					}
					positions.Add(new Position(x, y));
				}
			}
			return positions;
		}
		private int CalculateStartingX(Position position)
		{
			int value = position.X - 1;
			if (value < 0)
			{
				return 0;
			}
			return value;
		}
		private int CalculateLastX(Position position)
		{
			int value = position.X + 1;
			if (value >= boardSize)
			{
				return boardSize - 1;
			}
			return value;
		}
		private int CalculateStartingY(Position position)
		{
			int value = position.Y - 1;
			if (value < 0)
			{
				return 0;
			}
			return value;
		}
		private int CalculateLastY(Position position)
		{
			int value = position.Y + 1;
			if (value >= boardSize)
			{
				return boardSize - 1;
			}
			return value;
		}
	}
}