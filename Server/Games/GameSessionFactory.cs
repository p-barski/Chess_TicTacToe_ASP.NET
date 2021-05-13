using Server.Games.TicTacToe;

namespace Server.Games
{
	public class GameSessionFactory : IGameSessionFactory
	{
		public IGameSession Create(IPlayer playerOne, IPlayer playerTwo, int size)
		{
			return new TicTacToeGameSession(playerOne, playerTwo, size);
		}
	}
}