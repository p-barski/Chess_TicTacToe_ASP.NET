using System.Threading.Tasks;
using Server.TicTacToe;

namespace Server.Sockets
{
	public interface ICollections
	{
		void AddPlayer(Player player);
		Task RemovePlayer(Player player);
		void AddSession(Player first, Player second, int size);
		void RemoveSession(GameSession session);
		Player FindPlayerSearchingForGame(Player excludedPlayer);
		GameSession FindSessionOfAPlayer(Player player);
	}
}