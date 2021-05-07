using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Chess;
using Chess.Pieces;
using Chess.Movement;

namespace ChessTests
{
	public class PiecePromoterTests
	{
		[Test]
		public void WhenMoveIsNotPromotionReturnFalse()
		{
			var movementHistoryMock = new Mock<IMovementHistory>(MockBehavior.Strict);
			var piecesMock = new Mock<IEnumerable<IChessPiece>>(MockBehavior.Strict);

			var notPromotionMove = new ChessMove(new Position(0, 1), new Position(0, 3));

			var piecePromoter = new PiecePromoter(movementHistoryMock.Object);

			var result = piecePromoter
				.PromoteIfPromotionMove(notPromotionMove, piecesMock.Object);

			Assert.IsFalse(result);
		}
		[Test]
		public void WhenMoveIsPromotionPromotePawnAndReturnTrue()
		{
			var movementHistoryMock = new Mock<IMovementHistory>(MockBehavior.Strict);
			var promotedPawn = new Mock<IChessPiece>(MockBehavior.Strict);

			var promoteType = ChessPieceType.Queen;

			var promotionMove = new ChessMove(new Position(0, 0), new Position(0, 0),
				pawnPromotion: promoteType);

			var pawnPosition = new Position(3, 0);

			var moves = new List<ChessMove>(){
				new ChessMove(new Position(0, 1), new Position(0, 3)),
				new ChessMove(new Position(3, 1), pawnPosition),
			};

			promotedPawn
				.SetupGet(p => p.Position)
				.Returns(pawnPosition);
			promotedPawn
				.Setup(p => p.Promote(promoteType));

			movementHistoryMock
				.SetupGet(h => h.ChessMoves)
				.Returns(moves);
			movementHistoryMock
				.Setup(h => h.Add(promotionMove));

			var piecePromoter = new PiecePromoter(movementHistoryMock.Object);

			var pieces = new List<IChessPiece>() { promotedPawn.Object };
			var result = piecePromoter
				.PromoteIfPromotionMove(promotionMove, pieces);

			Assert.IsTrue(result);
			promotedPawn
				.Verify(p => p.Promote(promoteType));
			movementHistoryMock
				.Verify(h => h.Add(promotionMove));
		}
		[Test]
		public void WhenLastMoveWasNotPromotionReturnTheSameChessMove()
		{
			var movementHistoryMock = new Mock<IMovementHistory>(MockBehavior.Strict);
			var piecesMock = new Mock<IEnumerable<IChessPiece>>(MockBehavior.Strict);

			var notPromotionMove = new ChessMove(new Position(0, 1), new Position(0, 3));

			var piecePromoter = new PiecePromoter(movementHistoryMock.Object);

			var result = piecePromoter
				.DepromoteIfPromotionMove(notPromotionMove, piecesMock.Object);

			Assert.AreEqual(notPromotionMove, result);
		}
		[Test]
		public void WhenLastMoveWasPromotionDepromotePieceAndReturnLastMove()
		{
			var movementHistoryMock = new Mock<IMovementHistory>(MockBehavior.Strict);
			var promotedPawn = new Mock<IChessPiece>(MockBehavior.Strict);

			var promoteType = ChessPieceType.Queen;

			var promotionMove = new ChessMove(new Position(0, 0), new Position(0, 0),
				pawnPromotion: promoteType);

			var pawnPosition = new Position(3, 0);

			var lastMove = new ChessMove(new Position(3, 1), pawnPosition);

			promotedPawn
				.SetupGet(p => p.Position)
				.Returns(pawnPosition);
			promotedPawn
				.Setup(p => p.Depromote());

			movementHistoryMock
				.Setup(h => h.RemoveLastMove())
				.Returns(lastMove);

			var piecePromoter = new PiecePromoter(movementHistoryMock.Object);

			var pieces = new List<IChessPiece>() { promotedPawn.Object };
			var result = piecePromoter
				.DepromoteIfPromotionMove(promotionMove, pieces);

			Assert.AreEqual(lastMove, result);
			promotedPawn
				.Verify(p => p.Depromote());
			movementHistoryMock
				.Verify(h => h.RemoveLastMove());
		}
	}
}