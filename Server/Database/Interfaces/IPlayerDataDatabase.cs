using System.Threading.Tasks;

namespace Server.Database
{
	public interface IPlayerDataDatabase
	{
		PlayerData PlayerDataForNotLoggedInPlayers { get; }
		Task<bool> SavePlayerDataAsync(PlayerData playerData);
		PlayerData GetPlayerData(string name);
	}
}