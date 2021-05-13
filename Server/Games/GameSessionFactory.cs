using System;
using Server.Games.TicTacToe;

namespace Server.Games
{
	public class GameSessionFactory : IGameSessionFactory
	{
		public IGameSession Create(IPlayer playerOne, IPlayer playerTwo, IExpectedGame expectedGame)
		{
			if (expectedGame is ExpectedTicTacToe)
			{
				int size = ((ExpectedTicTacToe)expectedGame).Size;
				return new TicTacToeGameSession(playerOne, playerTwo, size);
			}
			throw new NotImplementedException();
		}
	}
}