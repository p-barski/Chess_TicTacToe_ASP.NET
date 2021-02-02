using System.Threading.Tasks;
using Server.TicTacToe;

namespace Server.Sockets
{
	public interface ICollections
	{
		void AddPlayer(IPlayer player);
		Task RemovePlayer(IPlayer player);
		void AddSession(IPlayer first, IPlayer second, int size);
		void RemoveSession(IGameSession session);
		IPlayer FindPlayerSearchingForGame(IPlayer excludedPlayer);
		IGameSession FindSessionOfAPlayer(IPlayer player);
	}
}