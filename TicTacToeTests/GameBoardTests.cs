using System;
using NUnit.Framework;
using TicTacToe;

namespace Tests
{
	public class GameBoardTests
	{
		[Test]
		public void WinnerShouldReturnXO_EnumEmptyWhenThereIsNoWinner()
		{
			var board = new GameBoard(3);
			Assert.AreEqual(XO_Enum.Empty, board.Winner());
		}
		[Test]
		public void VerticalWin_WinnerShouldBeX()
		{
			var board = new GameBoard(3);
			board[0, 0] = XO_Enum.X;
			board[0, 1] = XO_Enum.X;
			board[0, 2] = XO_Enum.X;
			Assert.AreEqual(XO_Enum.X, board.Winner());
		}
		[Test]
		public void VerticalWin_WinnerShouldBeO()
		{
			var board = new GameBoard(5);
			board[3, 0] = XO_Enum.O;
			board[3, 1] = XO_Enum.O;
			board[3, 2] = XO_Enum.O;
			board[3, 3] = XO_Enum.O;
			board[3, 4] = XO_Enum.O;
			Assert.AreEqual(XO_Enum.O, board.Winner());
		}
		[Test]
		public void HorizontalWin_WinnerShouldBeO()
		{
			var board = new GameBoard(4);
			board[0, 1] = XO_Enum.O;
			board[1, 1] = XO_Enum.O;
			board[2, 1] = XO_Enum.O;
			board[3, 1] = XO_Enum.O;
			Assert.AreEqual(XO_Enum.O, board.Winner());
		}
		[Test]
		public void HorizontalWin_WinnerShouldBeX()
		{
			var board = new GameBoard(6);
			board[0, 4] = XO_Enum.X;
			board[1, 4] = XO_Enum.X;
			board[2, 4] = XO_Enum.X;
			board[3, 4] = XO_Enum.X;
			board[4, 4] = XO_Enum.X;
			board[5, 4] = XO_Enum.X;
			Assert.AreEqual(XO_Enum.X, board.Winner());
		}
		[Test]
		public void Diagonal1Win_WinnerShouldBeX()
		{
			var board = new GameBoard(5);
			board[0, 0] = XO_Enum.X;
			board[1, 1] = XO_Enum.X;
			board[2, 2] = XO_Enum.X;
			board[3, 3] = XO_Enum.X;
			board[4, 4] = XO_Enum.X;
			Assert.AreEqual(XO_Enum.X, board.Winner());
		}
		[Test]
		public void Diagonal1Win_WinnerShouldBeO()
		{
			var board = new GameBoard(3);
			board[0, 0] = XO_Enum.O;
			board[1, 1] = XO_Enum.O;
			board[2, 2] = XO_Enum.O;
			Assert.AreEqual(XO_Enum.O, board.Winner());
		}
		[Test]
		public void Diagonal2Win_WinnerShouldBeX()
		{
			var board = new GameBoard(7);
			board[0, 6] = XO_Enum.X;
			board[1, 5] = XO_Enum.X;
			board[2, 4] = XO_Enum.X;
			board[3, 3] = XO_Enum.X;
			board[4, 2] = XO_Enum.X;
			board[5, 1] = XO_Enum.X;
			board[6, 0] = XO_Enum.X;
			Assert.AreEqual(XO_Enum.X, board.Winner());
		}
		[Test]
		public void Diagonal2Win_WinnerShouldBeO()
		{
			var board = new GameBoard(4);
			board[0, 3] = XO_Enum.O;
			board[1, 2] = XO_Enum.O;
			board[2, 1] = XO_Enum.O;
			board[3, 0] = XO_Enum.O;
			Assert.AreEqual(XO_Enum.O, board.Winner());
		}
		[Test]
		public void TestDraw1()
		{
			var board = new GameBoard(3);
			board[0, 2] = XO_Enum.O;
			board[0, 1] = XO_Enum.X;
			board[1, 0] = XO_Enum.O;
			board[1, 1] = XO_Enum.X;
			board[2, 2] = XO_Enum.O;
			board[1, 2] = XO_Enum.X;
			board[2, 1] = XO_Enum.O;
			board[2, 0] = XO_Enum.X;
			Assert.AreEqual(XO_Enum.Draw, board.Winner());
		}
		[Test]
		public void TestDraw2()
		{
			var board = new GameBoard(4);
			board[0, 0] = XO_Enum.O;
			board[0, 1] = XO_Enum.X;
			board[1, 2] = XO_Enum.O;
			board[1, 3] = XO_Enum.X;
			board[2, 1] = XO_Enum.O;
			board[2, 2] = XO_Enum.X;
			board[3, 3] = XO_Enum.O;
			board[3, 0] = XO_Enum.X;
			Assert.AreEqual(XO_Enum.Draw, board.Winner());
		}
		[Test]
		public void SettingTheSamePlaceMoreThanOnceResultsInInvalidOperationException()
		{
			var board = new GameBoard(4);
			board[0, 0] = XO_Enum.O;
			Assert.Throws<InvalidOperationException>(
				() => board[0, 0] = XO_Enum.X);
		}
	}
}