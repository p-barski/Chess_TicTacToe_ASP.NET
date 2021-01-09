using System;

namespace TicTacToe
{
	public class Game
	{
		private GameBoard board;
		public XO_Enum Winner { get; private set; } = XO_Enum.Empty;
		public XO_Enum CurrentPlayer { get; private set; } = XO_Enum.X;
		public Game(int size)
		{
			board = new GameBoard(size);
		}
		public XO_Enum Set(int x, int y)
		{
			if (Winner != XO_Enum.Empty)
				throw new InvalidOperationException("The game finished.");
			board[x, y] = CurrentPlayer;
			CurrentPlayer = CurrentPlayer.OppositePlayer();
			Winner = board.Winner();
			return Winner;
		}
	}
}