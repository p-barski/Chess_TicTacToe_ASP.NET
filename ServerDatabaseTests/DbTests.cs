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

			var databaseAccess = new DatabaseAccess("Data Source=tests.db;");
			await databaseAccess.SaveGameAsync(chessGame);
		}
		[Test]
		public async Task TestSavingPlayerData()
		{
			var databaseAccess = new DatabaseAccess("Data Source=tests.db;");
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
			var databaseAccess = new DatabaseAccess("Data Source=tests.db;");
			var games = databaseAccess.GetAllSavedGames();
			games
				.ToList()
				.ForEach(g => Assert.IsTrue(g.ChessMoves != null));
		}
	}
}