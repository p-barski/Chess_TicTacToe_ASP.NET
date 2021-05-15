namespace Server.Games.Chess
{
	public class ExpectedChess : IExpectedGame
	{
		public static bool operator ==(ExpectedChess left, ExpectedChess right)
		{
			return true;
		}
		public static bool operator !=(ExpectedChess left, ExpectedChess right)
		{
			return false;
		}
		public override bool Equals(object obj)
		{
			return obj is ExpectedChess;
		}
		public override int GetHashCode()
		{
			return true.GetHashCode();
		}
		public override string ToString()
		{
			return $"ExpectedChess";
		}
	}
}