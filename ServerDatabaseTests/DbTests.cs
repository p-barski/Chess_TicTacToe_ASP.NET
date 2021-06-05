using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using NUnit.Framework;
using Server.Database;
using Server.Database.Chess;

namespace ServerDatabaseTests
{
	public class DbTests
	{
		const string connectionString = "Data Source=tests.db";
		[SetUp]
		public void SetUp()
		{
			var sqliteFile = connectionString.Substring(12);
			try
			{
				Console.WriteLine(Path.GetFullPath(sqliteFile));
				File.Delete(sqliteFile);
			}
			catch (IOException) { }
		}
		[Test]
		public async Task TestSavingChessGame()
		{
			var whitePlayer = new PlayerData("Test1", "k$|31FsvXh]./a;1@#$Fg@#$$%&5423dfhFDdfvb");
			var blackPlayer = new PlayerData("Test2", "345#^$254235#@%#gdFG%^&Tyjfgb%$^df%^&RGH");
			var startDate = DateTime.Now;
			var finishDate = startDate.AddDays(1);
			var chessGame = new ChessGameDb(whitePlayer, blackPlayer, startDate);

			var databaseAccess = new DatabaseAccess(connectionString);
			await databaseAccess.SaveGameAsync(chessGame);
		}
		[Test]
		public async Task TestUpdatingChessGame()
		{
			var chessMoves = new List<ChessMoveDb>(){
				new ChessMoveDb(0, 1, 0, 3, "", DateTime.UtcNow),
				new ChessMoveDb(0, 6, 0, 4, "", DateTime.UtcNow),
			};
			var whitePlayer = new PlayerData("Test1", "O|POfB#$5GHfJ345rWSvbcB45345etHFGHerTw");
			var blackPlayer = new PlayerData("Test2", "#$%dsgDhR^Y&DfG365Fgrt^4%^jTYIDdfShO67");
			var startDate = DateTime.Now;
			var finishDate = startDate.AddDays(1);
			var chessGame = new ChessGameDb(whitePlayer, blackPlayer, startDate);

			var databaseAccess = new DatabaseAccess(connectionString);
			await databaseAccess.SaveGameAsync(chessGame);

			chessGame.ChessMoves.AddRange(chessMoves);
			chessGame.Result = "BlackWin";
			chessGame.FinishDate = finishDate;

			await databaseAccess.UpdateGameAsync(chessGame);
		}
		[Test]
		public async Task TestSavingPlayerData()
		{
			var databaseAccess = new DatabaseAccess(connectionString);
			byte[] salt;
			new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
			var pbkdf2 = new Rfc2898DeriveBytes("hello", salt, 100000);
			byte[] hash = pbkdf2.GetBytes(20);
			byte[] hashBytes = new byte[36];
			Array.Copy(salt, 0, hashBytes, 0, 16);
			Array.Copy(hash, 0, hashBytes, 16, 20);
			string password = Convert.ToBase64String(hashBytes);
			var playerData = new PlayerData("userName", password);
			await databaseAccess.SavePlayerDataAsync(playerData);
		}
		[Test]
		public async Task TestReading()
		{
			var chessMoves = new List<ChessMoveDb>(){
				new ChessMoveDb(0, 1, 0, 3, "", DateTime.UtcNow),
				new ChessMoveDb(0, 6, 0, 4, "", DateTime.UtcNow),
			};
			var whitePlayer = new PlayerData("Test1", "O|POfB#$5GHfJ345rWSvbcB45345etHFGHerTw");
			var blackPlayer = new PlayerData("Test2", "#$%dsgDhR^Y&DfG365Fgrt^4%^jTYIDdfShO67");
			var startDate = DateTime.Now;
			var finishDate = startDate.AddDays(1);
			var chessGame = new ChessGameDb(whitePlayer, blackPlayer, startDate);

			var databaseAccess = new DatabaseAccess(connectionString);
			await databaseAccess.SaveGameAsync(chessGame);

			chessGame.ChessMoves.AddRange(chessMoves);
			chessGame.FinishDate = finishDate;

			await databaseAccess.UpdateGameAsync(chessGame);
			var gameDb = await databaseAccess.GetSavedGame(whitePlayer, blackPlayer);
			Assert.IsTrue(gameDb.ChessMoves != null);
		}
		[Test]
		public void TestGettingPlayerDataForNotLoggedInPlayers()
		{
			var databaseAccess = new DatabaseAccess(connectionString);
			var playerData = databaseAccess.PlayerDataForNotLoggedInPlayers;
			Assert.AreEqual("Anonymous", playerData.Name);
			Assert.AreEqual("", playerData.Password);
		}
		[Test]
		public void TestNpgsqlUrlParser()
		{
			var host = "localhost";
			var port = "5000";
			var username = "name";
			var password = "password1";
			var database = "testdb";
			var efCoreConnectionString =
				$"Host={host};Port={port};Username={username};" +
				$"Password={password};Database={database};" +
				$"SSL Mode=Require;Pooling=True;Trust Server Certificate=True";

			var databaseUrl = $"postgres://{username}:{password}@{host}:{port}/{database}";
			var connectionString = NpgsqlUrlParser.ParseToEFCoreConnectionString(databaseUrl);

			Assert.AreEqual(efCoreConnectionString, connectionString);
		}
	}
}