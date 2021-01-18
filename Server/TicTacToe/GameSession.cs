using System;
using TicTacToe;

namespace Server.TicTacToe
{
	public class GameSession
	{
		public Guid GUID { get; } = Guid.NewGuid();
		public Player PlayerX { get; }
		public Player PlayerO { get; }
		private Game game;
		public GameSession(Player first, Player second, int size)
		{
			lock (first)
			{
				lock (second)
				{
					if (first.GameSessionGUID != Guid.Empty || second.GameSessionGUID != Guid.Empty)
						throw new InvalidOperationException("Player is already assigned to a game session.");

					PlayerX = first;
					PlayerO = second;
					PlayerX.AddToGame(GUID, XO_Enum.X);
					PlayerO.AddToGame(GUID, XO_Enum.O);
					game = new Game(size);
				}
			}
		}
		public PlayResult Play(Player from, int x, int y)
		{
			if (from.Sign != game.CurrentPlayer)
				return PlayResult.NotYourTurn;
			XO_Enum result;
			try
			{
				result = game.Set(x, y);
			}
			catch (InvalidOperationException)
			{
				return PlayResult.PositionTaken;
			}
			catch (IndexOutOfRangeException)
			{
				return PlayResult.Error;
			}
			switch (result)
			{
				case XO_Enum.Empty:
					return PlayResult.Success;
				case XO_Enum.Draw:
					return PlayResult.Draw;
				default:
					return PlayResult.YouWin;
			}
		}
		public void Close()
		{
			PlayerX.RemoveFromGame();
			PlayerO.RemoveFromGame();
		}
	}
}