using System;

namespace Chess
{
	public struct Position
	{
		public int X { get; init; }
		public int Y { get; init; }
		public Position(int x, int y)
		{
			X = x;
			Y = y;
		}
		public static bool operator ==(Position left, Position right)
		{
			return left.Equals(right);
		}
		public static bool operator !=(Position left, Position right)
		{
			return !left.Equals(right);
		}
		public override bool Equals(object obj)
		{
			if (!(obj is Position))
			{
				return false;
			}
			return Equals((Position)obj);
		}
		public bool Equals(Position other)
		{
			return X == other.X &&
				Y == other.Y;
		}
		public override int GetHashCode()
		{
			return HashCode.Combine(X, Y);
		}
	}
}