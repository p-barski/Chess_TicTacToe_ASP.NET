using System.Threading.Tasks;

namespace Server.Database
{
	public interface IPlayerDataDatabase
	{
		PlayerData PlayerDataForNotLoggedInPlayers { get; }
		Task SavePlayerDataAsync(PlayerData playerData);
		PlayerData GetPlayerData(string name, string password);
	}
}