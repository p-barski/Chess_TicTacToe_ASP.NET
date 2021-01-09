namespace TicTacToe
{
	public enum XO_Enum
	{
		Empty, X, O, Draw
	}
	static class XO_EnumExtensions
	{
		public static XO_Enum OppositePlayer(this XO_Enum enum_)
		{
			if (enum_ == XO_Enum.X)
				return XO_Enum.O;
			return XO_Enum.X;
		}
	}
}