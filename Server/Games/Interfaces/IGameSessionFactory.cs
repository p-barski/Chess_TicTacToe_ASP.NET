namespace Server.Games
{
	public interface IGameSessionFactory
	{
		IGameSession Create(IPlayer playerOne, IPlayer playerTwo, IExpectedGame expectedGame);
	}
}