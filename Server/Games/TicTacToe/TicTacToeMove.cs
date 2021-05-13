using System;

namespace Server.Games
{
	public class TicTacToeMove : IGameMove
	{
		public int X { get; }
		public int Y { get; }
		public TicTacToeMove(int x, int y)
		{
			X = x;
			Y = y;
		}
		public static bool operator ==(TicTacToeMove left, TicTacToeMove right)
		{
			return left.Equals(right);
		}
		public static bool operator !=(TicTacToeMove left, TicTacToeMove right)
		{
			return !left.Equals(right);
		}
		public override bool Equals(object obj)
		{
			if (!(obj is TicTacToeMove))
			{
				return false;
			}
			return Equals((TicTacToeMove)obj);
		}
		public bool Equals(TicTacToeMove other)
		{
			return X == other.X &&
				Y == other.Y;
		}
		public override int GetHashCode()
		{
			return HashCode.Combine(X, Y);
		}
		public override string ToString()
		{
			return $"TicTacToeMove({X}, {Y})";
		}
	}
}