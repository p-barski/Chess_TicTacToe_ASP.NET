using System;
using TicTacToe;

namespace Server.Games.TicTacToe
{
	public class TicTacToeGameSession : IGameSession
	{
		public Guid GUID { get; } = Guid.NewGuid();
		public IPlayer PlayerOne { get; }
		public IPlayer PlayerTwo { get; }
		public DateTime StartDate { get; } = DateTime.UtcNow;
		private Game game;
		public TicTacToeGameSession(IPlayer playerOne, IPlayer playerTwo, int size)
		{
			lock (playerOne)
			{
				lock (playerTwo)
				{
					if (playerOne.GameSessionGUID != Guid.Empty || playerTwo.GameSessionGUID != Guid.Empty)
						throw new InvalidOperationException("Player is already assigned to a game session.");

					PlayerOne = playerOne;
					PlayerTwo = playerTwo;
					PlayerOne.AddToGame(GUID, new PlayerType<XO_Enum>(XO_Enum.X));
					PlayerTwo.AddToGame(GUID, new PlayerType<XO_Enum>(XO_Enum.O));
					game = new Game(size);
				}
			}
		}
		public PlayResult Play(IPlayer from, IGameMove gameMove)
		{
			if (from.PlayerType.StringRepresentation
				.ToEnum<XO_Enum>() != game.CurrentPlayer)
			{
				return PlayResult.NotYourTurn;
			}
			TicTacToeMove tttMove;
			try
			{
				tttMove = (TicTacToeMove)gameMove;
			}
			catch (InvalidCastException)
			{
				return PlayResult.Error;
			}

			XO_Enum result;
			try
			{
				result = game.Set(tttMove.X, tttMove.Y);
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
			PlayerOne.RemoveFromGame();
			PlayerTwo.RemoveFromGame();
		}
	}
}