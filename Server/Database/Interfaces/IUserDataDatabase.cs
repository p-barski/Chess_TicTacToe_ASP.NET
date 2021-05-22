using System.Threading.Tasks;

namespace Server.Database
{
	public interface IPlayerDataDatabase
	{
		Task SavePlayerDataAsync(PlayerData playerData);
		PlayerData GetPlayerData(string name, string password);
		Task<PlayerData> GetPlayerDataForNotLoggedInPlayers();
	}
}