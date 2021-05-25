using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Server.Database.Chess;

namespace Server.Database
{
	public class DatabaseAccess : IChessDatabase, IPlayerDataDatabase
	{
		public PlayerData PlayerDataForNotLoggedInPlayers { get; }
		private const string defaultName = "Anonymous";
		private const string defaultPassword = "";
		private readonly string connectionString;
		private readonly bool usePostgres;
		public DatabaseAccess(string connectionString, bool usePostgres = false)
		{
			this.connectionString = connectionString;
			this.usePostgres = usePostgres;
			using var context = new GamesDbContext(connectionString, usePostgres);
			context.Database.EnsureCreated();
			PlayerDataForNotLoggedInPlayers = GetOrCreatePlayerDataForNotLoggedInPlayers(context);
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
			context.PlayerDatas.Attach(chessGame.WhitePlayer);
			context.PlayerDatas.Attach(chessGame.BlackPlayer);
			await context.AddAsync(chessGame);
			await context.SaveChangesAsync();
		}
		public PlayerData GetPlayerData(string name)
		{
			using var context = new GamesDbContext(connectionString, usePostgres);
			return context.PlayerDatas
				.FirstOrDefault(u => u.Name == name);
		}
		public async Task<bool> SavePlayerDataAsync(PlayerData playerData)
		{
			using var context = new GamesDbContext(connectionString, usePostgres);
			try
			{
				await context.AddAsync(playerData);
				await context.SaveChangesAsync();
			}
			catch (DbUpdateException)
			{
				return false;
			}
			return true;
		}
		private PlayerData GetOrCreatePlayerDataForNotLoggedInPlayers(GamesDbContext context)
		{
			var playerData = context.PlayerDatas
				.FirstOrDefault(u => u.Name == defaultName && u.Password == defaultPassword);
			if (playerData != null)
			{
				return playerData;
			}

			playerData = new PlayerData(defaultName, defaultPassword);
			context.Add(playerData);
			context.SaveChanges();
			return playerData;
		}
	}
}