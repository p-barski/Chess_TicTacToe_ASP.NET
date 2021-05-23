using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Server.Database;
using Server.Database.Chess;

namespace ServerDatabaseTests
{
	public class DbTests
	{
		const string connectionString = "Data Source=tests.db;";
		[Test]
		public async Task TestSavingChessGame()
		{
			var chessMoves = new List<ChessMoveDb>(){
				new ChessMoveDb(0, 0, 1, 0, 3, ""),
				new ChessMoveDb(1, 0, 6, 0, 4, ""),
			};
			var whitePlayer = new PlayerData("Test1", "k$|31FsvXh]./a;1@#$Fg@#$$%&5423dfhFDdfvb");
			var blackPlayer = new PlayerData("Test2",
				new string("k$|31FsvXh]./a;1@#$Fg@#$$%&5423dfhFDdfvb".Reverse().ToArray()));
			var startDate = DateTime.Now;
			var finishDate = startDate.AddDays(1);
			var chessGame = new ChessGameDb(chessMoves, whitePlayer, blackPlayer,
				startDate, finishDate, "Stalemate");

			var databaseAccess = new DatabaseAccess(connectionString);
			await databaseAccess.SaveGameAsync(chessGame);
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
		public void TestReading()
		{
			var databaseAccess = new DatabaseAccess(connectionString);
			var games = databaseAccess.GetAllSavedGames();
			games
				.ToList()
				.ForEach(g => Assert.IsTrue(g.ChessMoves != null));
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