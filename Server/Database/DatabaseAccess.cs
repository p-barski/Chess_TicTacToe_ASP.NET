using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Server.Database.Chess;

namespace Server.Database
{
	public class DatabaseAccess : IChessDatabase, IPlayerDataDatabase
	{
		private readonly string connectionString;
		private readonly bool usePostgres;
		public DatabaseAccess(string connectionString, bool usePostgres = false)
		{
			this.connectionString = connectionString;
			this.usePostgres = usePostgres;
			using var context = new GamesDbContext(connectionString, usePostgres);
			context.Database.EnsureCreated();
		}
		public IEnumerable<ChessGameDb> GetAllSavedGames()
		{
			using var context = new GamesDbContext(connectionString, usePostgres);
			var chessGames = context.ChessGames.ToList();
			//Loading ChessMoves list, otherwise it will be null
			chessGames
				.ForEach(async g => await context
					.Entry(g)
					.Collection(g => g.ChessMoves)
					.LoadAsync());
			return chessGames;
		}
		public async Task SaveGameAsync(ChessGameDb chessGame)
		{
			using var context = new GamesDbContext(connectionString, usePostgres);
			await context.AddAsync(chessGame);
			await context.SaveChangesAsync();
		}
		public async Task<PlayerData> GetPlayerDataForNotLoggedInPlayers()
		{
			string name = "Anonymous";
			string password = "";

			using var context = new GamesDbContext(connectionString, usePostgres);
			var playerData = context.PlayerDatas
				.First(u => u.Name == name && u.Password == password);

			if (playerData != null)
			{
				return playerData;
			}

			playerData = new PlayerData(name, password);
			await context.AddAsync(playerData);
			await context.SaveChangesAsync();
			return playerData;
		}
		public PlayerData GetPlayerData(string name, string password)
		{
			using var context = new GamesDbContext(connectionString, usePostgres);
			return context.PlayerDatas
				.FirstOrDefault(u => u.Name == name && u.Password == password);
		}
		public async Task SavePlayerDataAsync(PlayerData playerData)
		{
			using var context = new GamesDbContext(connectionString, usePostgres);
			await context.AddAsync(playerData);
			await context.SaveChangesAsync();
		}
	}
}