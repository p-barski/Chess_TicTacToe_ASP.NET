using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using Chess.Pieces;
using Chess.Board;

namespace Chess.Movement
{
	///<summary>
	///Responsible for calculating move of a knight.
	///</summary>
	public class KnightMovement : IMovement
	{
		private const int boardSize = 8;
		private readonly IChessBoard chessBoard;
		private readonly ImmutableArray<Tuple<int, int>> possibleCombinations;
		public KnightMovement(IChessBoard chessBoard)
		{
			this.chessBoard = chessBoard;
			var combinations = new List<Tuple<int, int>>(){
				new Tuple<int, int>(-2, -1),
				new Tuple<int, int>(-2, +1),
				new Tuple<int, int>(-1, +2),
				new Tuple<int, int>(+1, +2)
			};
			possibleCombinations = ImmutableArray.ToImmutableArray(combinations);
		}
		public IEnumerable<ChessMove> GetAvailableMoves(IReadOnlyChessPiece chessPiece)
		{
			var availableMoves = new List<ChessMove>();
			if (chessPiece.PieceType != ChessPieceType.Knight)
			{
				return availableMoves;
			}

			foreach (var position in GetPositions(chessPiece))
			{
				var chessMove = new ChessMove(chessPiece.Position, position);
				availableMoves.Add(chessMove);
			}

			return availableMoves;
		}
		private List<Position> GetPositions(IReadOnlyChessPiece chessPiece)
		{
			var positions = new List<Position>(8);
			int x = chessPiece.Position.X;
			int y = chessPiece.Position.Y;

			foreach ((var first, var second) in possibleCombinations)
			{
				positions.Add(new Position(x + first, y + second));
				positions.Add(new Position(x + second, y + first));
			}

			return FilterTakenPositionsAndEnemies(
				FilterInvalidPositions(positions), chessPiece.Color);
		}
		private List<Position> FilterInvalidPositions(List<Position> positions)
		{
			return positions
				.Where(p =>
					p.X >= 0 &&
					p.X < boardSize &&
					p.Y >= 0 &&
					p.Y < boardSize)
				.ToList();
		}
		private List<Position> FilterTakenPositionsAndEnemies(List<Position> positions,
			ChessColor chessColor)
		{
			var filtered = new List<Position>();
			foreach (var position in positions)
			{
				var isEnemy = chessBoard
					.IsEnemyOnPosition(position, chessColor.Opposite());

				if (isEnemy)
				{
					filtered.Add(position);
					continue;
				}
				if (!chessBoard.IsPositionTaken(position))
				{
					filtered.Add(position);
				}
			}
			return filtered;
		}
	}
}