namespace Server.TicTacToe
{
	public interface IGameSessionFactory
	{
		IGameSession Create(IPlayer playerOne, IPlayer playerTwo, int size);
	}
}