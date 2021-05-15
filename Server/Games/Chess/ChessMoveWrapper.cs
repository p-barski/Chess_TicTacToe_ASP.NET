using Chess.Movement;

namespace Server.Games.Chess
{
	public class ChessMoveWrapper
	{
		public ChessMove ChessMove { get; }
		public ChessMoveWrapper(ChessMove chessMove)
		{
			ChessMove = chessMove;
		}
		public override bool Equals(object obj)
		{
			if (!(obj is ChessMoveWrapper))
			{
				return false;
			}
			return Equals((ChessMoveWrapper)obj);
		}
		public bool Equals(ChessMoveWrapper other)
		{
			return ChessMove.Equals(other.ChessMove);
		}
		public override int GetHashCode()
		{
			return ChessMove.GetHashCode();
		}
		public override string ToString()
		{
			return ChessMove.ToString();
		}
	}
}