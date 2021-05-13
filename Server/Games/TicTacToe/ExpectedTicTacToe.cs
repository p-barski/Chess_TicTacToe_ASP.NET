namespace Server.Games.TicTacToe
{
	public class ExpectedTicTacToe : IExpectedGame
	{
		public int Size { get; }
		public ExpectedTicTacToe(int size)
		{
			Size = size;
		}
		public static bool operator ==(ExpectedTicTacToe left, ExpectedTicTacToe right)
		{
			return left.Equals(right);
		}
		public static bool operator !=(ExpectedTicTacToe left, ExpectedTicTacToe right)
		{
			return !left.Equals(right);
		}
		public override bool Equals(object obj)
		{
			if (!(obj is ExpectedTicTacToe))
			{
				return false;
			}
			return Equals((ExpectedTicTacToe)obj);
		}
		public bool Equals(ExpectedTicTacToe other)
		{
			return Size == other.Size;
		}
		public override int GetHashCode()
		{
			return Size.GetHashCode();
		}
		public override string ToString()
		{
			return $"ExpectedTicTacToe({Size})";
		}
	}
}