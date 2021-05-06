using System.Collections.Generic;

namespace Chess.Movement
{
	public class MovementHistory : IMovementHistory
	{
		public IReadOnlyList<ChessMove> ChessMoves { get => chessMoves; }
		private readonly List<ChessMove> chessMoves = new();
		public void Add(ChessMove chessMove)
		{
			chessMoves.Add(chessMove);
		}
		public ChessMove RemoveLastMove()
		{
			var move = chessMoves[^1];
			chessMoves.RemoveAt(chessMoves.Count - 1);
			return move;
		}
	}
}