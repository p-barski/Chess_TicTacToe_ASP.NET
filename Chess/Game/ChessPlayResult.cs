namespace Chess.Game
{
	public enum ChessPlayResult
	{
		SuccessfulMove,
		WrongPlayer,
		InvalidMove,
		PromotionRequired,
		WhiteChecked,
		BlackChecked,
		WhiteWin,
		BlackWin,
		Stalemate,
	}
}