using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using Chess;
using Chess.Game;
using Chess.Pieces;
using Chess.Movement;

namespace ChessTests
{
	public class FullTests
	{
		[Test]
		public void ThereShouldBe20StartingMoves()
		{
			var factory = new ChessGameFactory();
			var game = factory.Create();
			var moves = game.GetAvailableLegalMoves(ChessColor.White).ToList();
			Assert.AreEqual(20, moves.Count);
		}
		[Test]
		public void ThereShouldBe20FirstBlackMoves()
		{
			var factory = new ChessGameFactory();
			var game = factory.Create();
			game.Play(new ChessMove(new Position(0, 1), new Position(0, 2)), ChessColor.White);
			var moves = game.GetAvailableLegalMoves(ChessColor.Black).ToList();
			Assert.AreEqual(20, moves.Count);
		}
		[Test]
		public void QuickestWhiteWinTest()
		{
			var moves = new List<ChessMove>(){
				//king's white pawn two up
				new ChessMove(new Position(4, 1), new Position(4, 3)),
				//right bishop's black pawn two down
				new ChessMove(new Position(5, 6), new Position(5, 4)),
				//leftmost white pawn two up
				new ChessMove(new Position(0, 1), new Position(0, 3)),
				//right knight's black pawn two down
				new ChessMove(new Position(6, 6), new Position(6, 4)),
				//white queen checkmate
				new ChessMove(new Position(3, 0), new Position(7, 4)),
			};

			var factory = new ChessGameFactory();
			var game = factory.Create();

			ChessPlayResult result = ChessPlayResult.SuccessfulMove;
			foreach (var move in moves)
			{
				Assert.AreEqual(ChessPlayResult.SuccessfulMove, result);
				result = game.Play(move, game.CurrentPlayer);
			}

			Assert.AreEqual(ChessPlayResult.WhiteWin, result);
		}
		[Test]
		public void QuickestBlackWinTest()
		{
			var moves = new List<ChessMove>(){
				//right bishop's white pawn two up
				new ChessMove(new Position(5, 1), new Position(5, 3)),
				//king's black pawn two down
				new ChessMove(new Position(4, 6), new Position(4, 4)),
				//right knight's white pawn two up
				new ChessMove(new Position(6, 1), new Position(6, 3)),
				//black queen checkmate
				new ChessMove(new Position(3, 7), new Position(7, 3)),
			};

			var factory = new ChessGameFactory();
			var game = factory.Create();

			ChessPlayResult result = ChessPlayResult.SuccessfulMove;
			foreach (var move in moves)
			{
				Assert.AreEqual(ChessPlayResult.SuccessfulMove, result);
				result = game.Play(move, game.CurrentPlayer);
			}

			Assert.AreEqual(ChessPlayResult.BlackWin, result);
		}
		[Test]
		public void QuickestBlackKingsideCastling()
		{
			var moves = new List<ChessMove>(){
				//leftmost white pawn one up
				new ChessMove(new Position(0, 1), new Position(0, 2)),
				//king's black pawn two down
				new ChessMove(new Position(4, 6), new Position(4, 4)),
				//left white rook 1 up
				new ChessMove(new Position(0, 0), new Position(0, 1)),
				//right black bishop capturing leftmost white pawn
				new ChessMove(new Position(5, 7), new Position(0, 2)),
				//second from left white pawn capturing black bishop
				new ChessMove(new Position(1, 1), new Position(0, 2)),
				//right black knight move
				new ChessMove(new Position(6, 7), new Position(5, 5)),
				//left white bishop move
				new ChessMove(new Position(2, 0), new Position(1, 1)),
				//black kingside castling
				new ChessMove(new Position(4, 7), new Position(7, 7)),
			};

			var factory = new ChessGameFactory();
			var game = factory.Create();

			ChessPlayResult result = ChessPlayResult.SuccessfulMove;
			foreach (var move in moves)
			{
				Assert.AreEqual(ChessPlayResult.SuccessfulMove, result);
				System.Console.WriteLine(move);
				result = game.Play(move, game.CurrentPlayer);
			}

			Assert.AreEqual(ChessPlayResult.SuccessfulMove, result);
			var isBlackKingOnCorrectPosition = game.Pieces
				.Any(p =>
					p.Color == ChessColor.Black &&
					p.PieceType == ChessPieceType.King &&
					p.Position == new Position(6, 7));
			Assert.IsTrue(isBlackKingOnCorrectPosition);
		}
		[Test]
		public void QuickestWhitePromotionTest()
		{
			var moves = new List<ChessMove>(){
				//leftmost white pawn two up
				new ChessMove(new Position(0, 1), new Position(0, 3)),
				//left knight's black pawn two down
				new ChessMove(new Position(1, 6), new Position(1, 4)),
				//leftmost white pawn capture of black pawn
				new ChessMove(new Position(0, 3), new Position(1, 4)),
				//left black knight move
				new ChessMove(new Position(1, 7), new Position(2, 5)),
				//leftmost white pawn one up
				new ChessMove(new Position(1, 4), new Position(1, 5)),
				//king's black pawn two down
				new ChessMove(new Position(4, 6), new Position(4, 4)),
				//leftmost white pawn one up
				new ChessMove(new Position(1, 5), new Position(1, 6)),
				//black king move one down
				new ChessMove(new Position(4, 7), new Position(4, 6)),
				//leftmost white pawn reaching promotion area
				new ChessMove(new Position(1, 6), new Position(1, 7)),
			};

			var factory = new ChessGameFactory();
			var game = factory.Create();

			ChessPlayResult result = ChessPlayResult.SuccessfulMove;
			foreach (var move in moves)
			{
				Assert.AreEqual(ChessPlayResult.SuccessfulMove, result);
				result = game.Play(move, game.CurrentPlayer);
			}

			Assert.AreEqual(ChessPlayResult.PromotionRequired, result);
			var promotionMove = new ChessMove(new Position(0, 0), new Position(0, 0), pawnPromotion: ChessPieceType.Queen);

			result = game.Play(promotionMove, game.CurrentPlayer);
			Assert.AreEqual(ChessPlayResult.SuccessfulMove, result);

			var whiteQueens = game.Pieces
				.Where(p => p.Color == ChessColor.White &&
					p.PieceType == ChessPieceType.Queen)
				.ToList();

			Assert.AreEqual(2, whiteQueens.Count);
		}
		[Test]
		public void QuickestBlackPromotionTest()
		{
			var moves = new List<ChessMove>(){
				//leftmost white pawn two up
				new ChessMove(new Position(0, 1), new Position(0, 3)),
				//left knight's black pawn two down
				new ChessMove(new Position(1, 6), new Position(1, 4)),
				//left white rook two up
				new ChessMove(new Position(0, 0), new Position(0, 2)),
				//left knight's black pawn capture of white pawn
				new ChessMove(new Position(1, 4), new Position(0, 3)),
				//left white rook four right
				new ChessMove(new Position(0, 2), new Position(4, 2)),
				//left knight's black pawn one down
				new ChessMove(new Position(0, 3), new Position(0, 2)),
				//right white knight move
				new ChessMove(new Position(6, 0), new Position(7, 2)),
				//left knight's black pawn one down
				new ChessMove(new Position(0, 2), new Position(0, 1)),
				//right white knight move
				new ChessMove(new Position(7, 2), new Position(6, 4)),
				//left knight's black pawn reaching promotion area
				new ChessMove(new Position(0, 1), new Position(0, 0)),
			};

			var factory = new ChessGameFactory();
			var game = factory.Create();

			ChessPlayResult result = ChessPlayResult.SuccessfulMove;
			foreach (var move in moves)
			{
				Assert.AreEqual(ChessPlayResult.SuccessfulMove, result);
				result = game.Play(move, game.CurrentPlayer);
			}

			Assert.AreEqual(ChessPlayResult.PromotionRequired, result);
			var promotionMove = new ChessMove(new Position(0, 0), new Position(0, 0), pawnPromotion: ChessPieceType.Queen);

			result = game.Play(promotionMove, game.CurrentPlayer);
			Assert.AreEqual(ChessPlayResult.SuccessfulMove, result);

			var blackQueens = game.Pieces
				.Where(p => p.Color == ChessColor.Black &&
					p.PieceType == ChessPieceType.Queen)
				.ToList();

			Assert.AreEqual(2, blackQueens.Count);
		}
		[Test]
		public void TryingToMoveEmptySpaceReturnsInvalidMove()
		{
			var invalidMove = new ChessMove(new Position(2, 2), new Position(2, 3));
			var factory = new ChessGameFactory();
			var game = factory.Create();

			var result = game.Play(invalidMove, ChessColor.White);

			Assert.AreEqual(ChessPlayResult.InvalidMove, result);
		}
	}
}