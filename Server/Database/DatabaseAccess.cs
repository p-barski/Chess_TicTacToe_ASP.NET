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
		public async Task<ChessGameDb> GetSavedGame(PlayerData whitePlayer, PlayerData blackPlayer)
		{
			using var context = new GamesDbContext(connectionString, usePostgres);
			context.PlayerDatas.Attach(whitePlayer);
			context.PlayerDatas.Attach(blackPlayer);

			var chessGame = context.ChessGames
				.FirstOrDefault(
					g => g.Result == "" &&
					g.WhitePlayer.Id == whitePlayer.Id &&
					g.BlackPlayer.Id == blackPlayer.Id);

			//Loading ChessMoves list, otherwise it will be null
			await context.Entry(chessGame)
					.Collection(g => g.ChessMoves)
					.LoadAsync();
			return chessGame;
		}
		public async Task SaveGameAsync(ChessGameDb chessGame)
		{
			using var context = new GamesDbContext(connectionString, usePostgres);
			context.PlayerDatas.Attach(chessGame.WhitePlayer);
			context.PlayerDatas.Attach(chessGame.BlackPlayer);
			await context.AddAsync(chessGame);
			await context.SaveChangesAsync();
		}
		public async Task UpdateGameAsync(ChessGameDb chessGame)
		{
			using var context = new GamesDbContext(connectionString, usePostgres);
			context.PlayerDatas.Attach(chessGame.WhitePlayer);
			context.PlayerDatas.Attach(chessGame.BlackPlayer);
			context.ChessMoves.AttachRange(chessGame.ChessMoves);
			context.Update(chessGame);
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