using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Chess;
using Chess.Pieces;
using Chess.Movement;

namespace ChessTests
{
	public class EnPassantMoverTests
	{
		[Test]
		public void PerformEnPassantIfApplicable_PromotionMoveTest()
		{
			var movementHistory = new Mock<IMovementHistory>(MockBehavior.Strict);

			var pieces = new List<IChessPiece>();
			var chessMove = new ChessMove(new Position(0, 0), new Position(0, 0),
				pawnPromotion: ChessPieceType.Knight);

			var mover = new EnPassantMover(movementHistory.Object);
			var result = mover.PerformEnPassantIfApplicable(chessMove, pieces);

			Assert.AreEqual(null, result);
		}
		[Test]
		public void PerformEnPassantIfApplicable_NormalPawnMoveTest()
		{
			var movementHistory = new Mock<IMovementHistory>(MockBehavior.Strict);
			var whitePawnMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var blackPawnMock = new Mock<IChessPiece>(MockBehavior.Strict);

			var whitePawnPosition = new Position(4, 1);
			var blackPawnPosition = new Position(1, 6);
			var destination = new Position(4, 3);
			var chessMove = new ChessMove(whitePawnPosition, destination);

			whitePawnMock
				.SetupGet(p => p.Position)
				.Returns(whitePawnPosition);
			whitePawnMock
				.SetupGet(p => p.Color)
				.Returns(ChessColor.White);
			whitePawnMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Pawn);

			blackPawnMock
				.SetupGet(p => p.Position)
				.Returns(blackPawnPosition);
			blackPawnMock
				.SetupGet(p => p.Color)
				.Returns(ChessColor.Black);
			blackPawnMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Pawn);

			var pieces = new List<IChessPiece>(){
				whitePawnMock.Object, blackPawnMock.Object
			};

			var mover = new EnPassantMover(movementHistory.Object);
			var result = mover.PerformEnPassantIfApplicable(chessMove, pieces);

			Assert.AreEqual(null, result);
		}
		[Test]
		public void PerformEnPassantIfApplicable_NormalPawnCaptureTest()
		{
			var movementHistory = new Mock<IMovementHistory>(MockBehavior.Strict);
			var whitePawnMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var blackPawnMock = new Mock<IChessPiece>(MockBehavior.Strict);

			var whitePawnPosition = new Position(4, 3);
			var blackPawnPosition = new Position(5, 4);
			var chessMove = new ChessMove(whitePawnPosition, blackPawnPosition);

			whitePawnMock
				.SetupGet(p => p.Position)
				.Returns(whitePawnPosition);
			whitePawnMock
				.SetupGet(p => p.Color)
				.Returns(ChessColor.White);
			whitePawnMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Pawn);

			blackPawnMock
				.SetupGet(p => p.Position)
				.Returns(blackPawnPosition);
			blackPawnMock
				.SetupGet(p => p.Color)
				.Returns(ChessColor.Black);
			blackPawnMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Pawn);

			var pieces = new List<IChessPiece>(){
				whitePawnMock.Object, blackPawnMock.Object
			};

			var mover = new EnPassantMover(movementHistory.Object);
			var result = mover.PerformEnPassantIfApplicable(chessMove, pieces);

			Assert.AreEqual(null, result);
		}
		[Test]
		public void PerformEnPassantIfApplicable_WhiteEnPassantTest()
		{
			var movementHistory = new Mock<IMovementHistory>(MockBehavior.Strict);
			var whitePawnMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var blackPawnMock = new Mock<IChessPiece>(MockBehavior.Strict);

			var whitePawnPosition = new Position(4, 4);
			var blackPawnPosition = new Position(5, 4);
			var blackPawnPreviousPosition = new Position(5, 6);
			var whitePawnCapturePosition = new Position(5, 5);
			var chessMove = new ChessMove(whitePawnPosition, whitePawnCapturePosition);

			whitePawnMock
				.SetupGet(p => p.Position)
				.Returns(whitePawnPosition);
			whitePawnMock
				.SetupGet(p => p.Color)
				.Returns(ChessColor.White);
			whitePawnMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Pawn);
			whitePawnMock
				.SetupSet(p => p.Position = whitePawnCapturePosition);
			whitePawnMock
				.Setup(p => p.IncrementMoveCounter());

			blackPawnMock
				.SetupGet(p => p.Position)
				.Returns(blackPawnPosition);
			blackPawnMock
				.SetupGet(p => p.Color)
				.Returns(ChessColor.Black);
			blackPawnMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Pawn);

			var previousMoves = new List<ChessMove>(){
				new ChessMove(blackPawnPreviousPosition, blackPawnPosition)
			};

			movementHistory
				.Setup(h => h.Add(chessMove.ReturnWithCaptureAsTrue()));

			var pieces = new List<IChessPiece>(){
				whitePawnMock.Object, blackPawnMock.Object
			};

			var mover = new EnPassantMover(movementHistory.Object);
			var result = mover.PerformEnPassantIfApplicable(chessMove, pieces);

			Assert.AreEqual(blackPawnMock.Object, result);
			whitePawnMock
				.VerifySet(p => p.Position = whitePawnCapturePosition);
			whitePawnMock
				.Verify(p => p.IncrementMoveCounter());
			movementHistory
				.Verify(h => h.Add(chessMove.ReturnWithCaptureAsTrue()));
		}
		[Test]
		public void PerformEnPassantIfApplicable_BlackEnPassantTest()
		{
			var movementHistory = new Mock<IMovementHistory>(MockBehavior.Strict);
			var whitePawnMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var blackPawnMock = new Mock<IChessPiece>(MockBehavior.Strict);

			var whitePawnPosition = new Position(1, 3);
			var blackPawnPosition = new Position(0, 3);
			var whitePawnPreviousPosition = new Position(1, 1);
			var blackPawnCapturePosition = new Position(1, 2);
			var chessMove = new ChessMove(blackPawnPosition, blackPawnCapturePosition);

			whitePawnMock
				.SetupGet(p => p.Position)
				.Returns(whitePawnPosition);
			whitePawnMock
				.SetupGet(p => p.Color)
				.Returns(ChessColor.White);
			whitePawnMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Pawn);

			blackPawnMock
				.SetupGet(p => p.Position)
				.Returns(blackPawnPosition);
			blackPawnMock
				.SetupGet(p => p.Color)
				.Returns(ChessColor.Black);
			blackPawnMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Pawn);
			blackPawnMock
				.SetupSet(p => p.Position = blackPawnCapturePosition);
			blackPawnMock
				.Setup(p => p.IncrementMoveCounter());

			movementHistory
				.Setup(h => h.Add(chessMove.ReturnWithCaptureAsTrue()));

			var pieces = new List<IChessPiece>(){
				whitePawnMock.Object, blackPawnMock.Object
			};

			var mover = new EnPassantMover(movementHistory.Object);
			var result = mover.PerformEnPassantIfApplicable(chessMove, pieces);

			Assert.AreEqual(whitePawnMock.Object, result);
			blackPawnMock
				.VerifySet(p => p.Position = blackPawnCapturePosition);
			blackPawnMock
				.Verify(p => p.IncrementMoveCounter());
			movementHistory
				.Verify(h => h.Add(chessMove.ReturnWithCaptureAsTrue()));
		}
	}
}