using System.Collections.Generic;
using Chess.Pieces;
using Chess.Movement;

namespace Chess.Board
{
	public interface IChessBoard
	{
		IEnumerable<IReadOnlyChessPiece> Pieces { get; }
		IReadOnlyMovementHistory History { get; }
		IReadOnlyChessPiece GetKing(ChessColor kingColor);
		bool IsPositionTaken(Position position);
		bool IsEnemyOnPosition(Position position, ChessColor enemyColor);
		void Move(ChessMove chessMove);
		void ReverseLastMove();
	}
}