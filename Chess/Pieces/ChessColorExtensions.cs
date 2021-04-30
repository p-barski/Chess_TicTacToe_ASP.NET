namespace Chess.Pieces
{
	public static class ChessColorExtensions
	{
		public static ChessColor Opposite(this ChessColor color)
		{
			switch (color)
			{
				case ChessColor.White:
					return ChessColor.Black;
				case ChessColor.Black:
					return ChessColor.White;
				default:
					return color;
			}
		}
	}
}