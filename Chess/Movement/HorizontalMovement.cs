using System.Linq;
using System.Collections.Generic;
using Chess.Pieces;
using Chess.Board;

namespace Chess.Movement
{
	///<summary>
	///Responsible for calculating horizontal move of a rook and a queen.
	///</summary>
	public class HorizontalMovement : IMovement
	{
		private const int boardSize = 8;
		private readonly IChessBoard chessBoard;
		public HorizontalMovement(IChessBoard chessBoard)
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

			foreach (var position in GetLeftPositions(chessPiece)
				.Union(GetRightPositions(chessPiece)))
			{
				var chessMove = new ChessMove(chessPiece.Position, position);
				availableMoves.Add(chessMove);
			}

			return availableMoves;
		}
		private List<Position> GetLeftPositions(IReadOnlyChessPiece chessPiece)
		{
			var positions = new List<Position>();
			for (int x = chessPiece.Position.X - 1; x >= 0; x--)
			{
				var position = new Position(x, chessPiece.Position.Y);
				if (AddPositionToList(chessPiece, position, positions))
				{
					break;
				}
			}
			return positions;
		}
		private List<Position> GetRightPositions(IReadOnlyChessPiece chessPiece)
		{
			var positions = new List<Position>();
			for (int x = chessPiece.Position.X + 1; x < boardSize; x++)
			{
				var position = new Position(x, chessPiece.Position.Y);
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