using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using Chess;
using Chess.Movement;
using Server.Database;
using Server.Database.Chess;

namespace ServerDatabaseTests
{
	public class ChessMovementHistoryConverterTests
	{
		class MovementHistory : IReadOnlyMovementHistory
		{
			public IReadOnlyList<ChessMove> ChessMoves { get; }
			public MovementHistory(List<ChessMove> moves)
			{
				ChessMoves = moves;
			}
		}
		[Test]
		public void TestConvertingToDb()
		{
			var chessMoves = new List<ChessMove>(){
				new ChessMove(new Position(0, 1), new Position(0, 3)),
				new ChessMove(new Position(0, 6), new Position(0, 4)),
				new ChessMove(new Position(2, 1), new Position(2, 3)),
			};
			var history = new MovementHistory(chessMoves);
			var whitePlayer = new PlayerData("Test1", "k$|31FsvXh]./a;1@#$Fg@#$$%&5423dfhFDdfvb");
			var blackPlayer = new PlayerData("Test2",
				new string("k$|31FsvXh]./a;1@#$Fg@#$$%&5423dfhFDdfvb".Reverse().ToArray()));
			var startDate = DateTime.Now;
			var finishDate = startDate.AddDays(1);

			var converter = new ChessMovementHistoryConverter();

			var converted = converter.ConvertToDb((IReadOnlyMovementHistory)history, whitePlayer,
				blackPlayer, startDate, finishDate, "WhiteWin");

			Assert.AreEqual(whitePlayer, converted.WhitePlayer);
			Assert.AreEqual(blackPlayer, converted.BlackPlayer);
			Assert.AreEqual(startDate, converted.StartDate);
			Assert.AreEqual(finishDate, converted.FinishDate);
			Assert.AreEqual(chessMoves.Count, converted.ChessMoves.Count);
			for (int i = 0; i < converted.ChessMoves.Count; i++)
			{
				Assert.AreEqual(i, converted.ChessMoves[i].Index);
			}
		}
	}
}