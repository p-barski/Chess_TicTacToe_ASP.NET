namespace Server.TicTacToe
{
	public class GameSessionFactory : IGameSessionFactory
	{
		public IGameSession Create(IPlayer playerOne, IPlayer playerTwo, int size)
		{
			return new GameSession(playerOne, playerTwo, size);
		}
	}
}