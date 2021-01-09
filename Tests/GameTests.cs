using System;
using NUnit.Framework;
using TicTacToe;

namespace Tests
{
	public class GameTests
	{
		[Test]
		public void TestWinningAsX()
		{
			var game = new Game(3);
			Assert.AreEqual(XO_Enum.Empty, game.Set(1, 1));
			Assert.AreEqual(XO_Enum.Empty, game.Set(1, 0));
			Assert.AreEqual(XO_Enum.Empty, game.Set(0, 0));
			Assert.AreEqual(XO_Enum.Empty, game.Set(2, 0));
			Assert.AreEqual(XO_Enum.X, game.Set(2, 2));
		}
		[Test]
		public void TestWinningAsO()
		{
			var game = new Game(3);
			Assert.AreEqual(XO_Enum.Empty, game.Set(1, 0));
			Assert.AreEqual(XO_Enum.Empty, game.Set(1, 1));
			Assert.AreEqual(XO_Enum.Empty, game.Set(2, 0));
			Assert.AreEqual(XO_Enum.Empty, game.Set(0, 0));
			Assert.AreEqual(XO_Enum.Empty, game.Set(0, 1));
			Assert.AreEqual(XO_Enum.O, game.Set(2, 2));
		}
		[Test]
		public void SettingAfterWinningShouldResultInInvalidOperationException()
		{
			var game = new Game(3);
			Assert.AreEqual(XO_Enum.Empty, game.Set(1, 0));
			Assert.AreEqual(XO_Enum.Empty, game.Set(1, 1));
			Assert.AreEqual(XO_Enum.Empty, game.Set(2, 0));
			Assert.AreEqual(XO_Enum.Empty, game.Set(0, 0));
			Assert.AreEqual(XO_Enum.Empty, game.Set(0, 1));
			Assert.AreEqual(XO_Enum.O, game.Set(2, 2));
			Assert.Throws<InvalidOperationException>(() => game.Set(0, 2));
		}
	}
}