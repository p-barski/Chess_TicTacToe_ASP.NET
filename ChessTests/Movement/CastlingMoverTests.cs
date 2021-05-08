using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Chess;
using Chess.Pieces;
using Chess.Movement;

namespace ChessTests
{
	public class CastlingMoverTests
	{
		[Test]
		public void PerformCastlingIfCastlingMove_PromotionMoveTest()
		{
			var movementHistory = new Mock<IMovementHistory>(MockBehavior.Strict);

			var pieces = new List<IChessPiece>();
			var chessMove = new ChessMove(new Position(0, 0), new Position(0, 0),
				pawnPromotion: ChessPieceType.Knight);

			var mover = new CastlingMover(movementHistory.Object);
			var result = mover.PerformCastlingIfCastlingMove(chessMove, pieces);

			Assert.AreEqual(false, result);
		}
		[Test]
		public void PerformCastlingIfCastlingMove_NormalKingMoveTest()
		{
			var movementHistory = new Mock<IMovementHistory>(MockBehavior.Strict);
			var kingMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var rookQueensideMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var rookKingsideMock = new Mock<IChessPiece>(MockBehavior.Strict);

			var kingPosition = new Position(4, 0);
			var rookQueensidePosition = new Position(0, 0);
			var rookKingsidePosition = new Position(7, 0);
			var kingColor = ChessColor.White;
			var chessMove = new ChessMove(kingPosition, new Position(4, 1));

			kingMock
				.SetupGet(p => p.Position)
				.Returns(kingPosition);
			kingMock
				.SetupGet(p => p.Color)
				.Returns(kingColor);
			kingMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.King);

			rookQueensideMock
				.SetupGet(p => p.Position)
				.Returns(rookQueensidePosition);
			rookQueensideMock
				.SetupGet(p => p.Color)
				.Returns(kingColor);
			rookQueensideMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Rook);

			rookKingsideMock
				.SetupGet(p => p.Position)
				.Returns(rookKingsidePosition);
			rookKingsideMock
				.SetupGet(p => p.Color)
				.Returns(kingColor);
			rookKingsideMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Rook);

			var pieces = new List<IChessPiece>(){
				kingMock.Object, rookKingsideMock.Object,
				rookQueensideMock.Object
			};

			var mover = new CastlingMover(movementHistory.Object);
			var result = mover.PerformCastlingIfCastlingMove(chessMove, pieces);

			Assert.AreEqual(false, result);
		}
		[Test]
		public void PerformCastlingIfCastlingMove_KingsideCastleTest()
		{
			var movementHistory = new Mock<IMovementHistory>(MockBehavior.Strict);
			var kingMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var rookQueensideMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var rookKingsideMock = new Mock<IChessPiece>(MockBehavior.Strict);

			var kingPosition = new Position(4, 0);
			var rookQueensidePosition = new Position(0, 0);
			var rookKingsidePosition = new Position(7, 0);
			var rookFinalPosition = new Position(5, 0);
			var kingFinalPosition = new Position(6, 0);
			var kingColor = ChessColor.White;
			var chessMove = new ChessMove(kingPosition, rookKingsidePosition);

			kingMock
				.SetupGet(p => p.Position)
				.Returns(kingPosition);
			kingMock
				.SetupGet(p => p.Color)
				.Returns(kingColor);
			kingMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.King);
			kingMock
				.Setup(p => p.IncrementMoveCounter());
			kingMock
				.SetupSet(p => p.Position = kingFinalPosition);

			rookQueensideMock
				.SetupGet(p => p.Position)
				.Returns(rookQueensidePosition);
			rookQueensideMock
				.SetupGet(p => p.Color)
				.Returns(kingColor);
			rookQueensideMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Rook);

			rookKingsideMock
				.SetupGet(p => p.Position)
				.Returns(rookKingsidePosition);
			rookKingsideMock
				.SetupGet(p => p.Color)
				.Returns(kingColor);
			rookKingsideMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Rook);
			rookKingsideMock
				.Setup(p => p.IncrementMoveCounter());
			rookKingsideMock
				.SetupSet(p => p.Position = rookFinalPosition);

			movementHistory
				.Setup(h => h.Add(chessMove));

			var pieces = new List<IChessPiece>(){
				kingMock.Object, rookKingsideMock.Object,
				rookQueensideMock.Object
			};

			var mover = new CastlingMover(movementHistory.Object);
			var result = mover.PerformCastlingIfCastlingMove(chessMove, pieces);

			Assert.AreEqual(true, result);
			kingMock
				.VerifySet(p => p.Position = kingFinalPosition);
			kingMock
				.Verify(p => p.IncrementMoveCounter());
			rookKingsideMock
				.VerifySet(p => p.Position = rookFinalPosition);
			rookKingsideMock
				.Verify(p => p.IncrementMoveCounter());
			movementHistory
				.Verify(h => h.Add(chessMove));
		}
		[Test]
		public void PerformCastlingIfCastlingMove_QueensideCastleTest()
		{
			var movementHistory = new Mock<IMovementHistory>(MockBehavior.Strict);
			var kingMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var rookQueensideMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var rookKingsideMock = new Mock<IChessPiece>(MockBehavior.Strict);

			var kingPosition = new Position(4, 7);
			var rookQueensidePosition = new Position(0, 7);
			var rookKingsidePosition = new Position(7, 7);
			var rookFinalPosition = new Position(3, 7);
			var kingFinalPosition = new Position(2, 7);
			var kingColor = ChessColor.Black;
			var chessMove = new ChessMove(kingPosition, rookQueensidePosition);

			kingMock
				.SetupGet(p => p.Position)
				.Returns(kingPosition);
			kingMock
				.SetupGet(p => p.Color)
				.Returns(kingColor);
			kingMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.King);
			kingMock
				.Setup(p => p.IncrementMoveCounter());
			kingMock
				.SetupSet(p => p.Position = kingFinalPosition);

			rookQueensideMock
				.SetupGet(p => p.Position)
				.Returns(rookQueensidePosition);
			rookQueensideMock
				.SetupGet(p => p.Color)
				.Returns(kingColor);
			rookQueensideMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Rook);
			rookQueensideMock
				.Setup(p => p.IncrementMoveCounter());
			rookQueensideMock
				.SetupSet(p => p.Position = rookFinalPosition);


			rookKingsideMock
				.SetupGet(p => p.Position)
				.Returns(rookKingsidePosition);
			rookKingsideMock
				.SetupGet(p => p.Color)
				.Returns(kingColor);
			rookKingsideMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Rook);

			movementHistory
				.Setup(h => h.Add(chessMove));

			var pieces = new List<IChessPiece>(){
				kingMock.Object, rookKingsideMock.Object,
				rookQueensideMock.Object
			};

			var mover = new CastlingMover(movementHistory.Object);
			var result = mover.PerformCastlingIfCastlingMove(chessMove, pieces);

			Assert.AreEqual(true, result);
			kingMock
				.VerifySet(p => p.Position = kingFinalPosition);
			kingMock
				.Verify(p => p.IncrementMoveCounter());
			rookQueensideMock
				.VerifySet(p => p.Position = rookFinalPosition);
			rookQueensideMock
				.Verify(p => p.IncrementMoveCounter());
			movementHistory
				.Verify(h => h.Add(chessMove));
		}













		[Test]
		public void UndoCastlingIfCastlingMove_PromotionMoveTest()
		{
			var movementHistory = new Mock<IMovementHistory>(MockBehavior.Strict);

			var pieces = new List<IChessPiece>();
			var chessMove = new ChessMove(new Position(0, 0), new Position(0, 0),
				pawnPromotion: ChessPieceType.Knight);

			var mover = new CastlingMover(movementHistory.Object);
			var result = mover.UndoCastlingIfCastlingMove(chessMove, pieces);

			Assert.AreEqual(false, result);
		}
		[Test]
		public void UndoCastlingIfCastlingMove_NormalKingMoveTest()
		{
			var movementHistory = new Mock<IMovementHistory>(MockBehavior.Strict);
			var kingMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var rookQueensideMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var rookKingsideMock = new Mock<IChessPiece>(MockBehavior.Strict);

			var kingPreviousPosition = new Position(4, 0);
			var rookQueensidePosition = new Position(0, 0);
			var rookKingsidePosition = new Position(7, 0);
			var kingDestination = new Position(5, 1);
			var kingColor = ChessColor.White;
			var chessMove = new ChessMove(kingPreviousPosition, kingDestination);

			kingMock
				.SetupGet(p => p.Position)
				.Returns(kingDestination);
			kingMock
				.SetupGet(p => p.Color)
				.Returns(kingColor);
			kingMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.King);

			rookQueensideMock
				.SetupGet(p => p.Position)
				.Returns(rookQueensidePosition);
			rookQueensideMock
				.SetupGet(p => p.Color)
				.Returns(kingColor);
			rookQueensideMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Rook);

			rookKingsideMock
				.SetupGet(p => p.Position)
				.Returns(rookKingsidePosition);
			rookKingsideMock
				.SetupGet(p => p.Color)
				.Returns(kingColor);
			rookKingsideMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Rook);

			var pieces = new List<IChessPiece>(){
				kingMock.Object, rookKingsideMock.Object,
				rookQueensideMock.Object
			};

			var mover = new CastlingMover(movementHistory.Object);
			var result = mover.UndoCastlingIfCastlingMove(chessMove, pieces);

			Assert.AreEqual(false, result);
		}
		[Test]
		public void UndoCastlingIfCastlingMove_KingsideCastleTest()
		{
			var movementHistory = new Mock<IMovementHistory>(MockBehavior.Strict);
			var kingMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var rookQueensideMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var rookKingsideMock = new Mock<IChessPiece>(MockBehavior.Strict);

			var kingPreviousPosition = new Position(4, 0);
			var rookQueensidePosition = new Position(0, 0);
			var rookKingsidePreviousPosition = new Position(7, 0);
			var rookFinalPosition = new Position(5, 0);
			var kingFinalPosition = new Position(6, 0);
			var kingColor = ChessColor.White;
			var chessMove = new ChessMove(kingPreviousPosition, rookKingsidePreviousPosition);

			kingMock
				.SetupGet(p => p.Position)
				.Returns(kingFinalPosition);
			kingMock
				.SetupGet(p => p.Color)
				.Returns(kingColor);
			kingMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.King);
			kingMock
				.Setup(p => p.DecrementMoveCounter());
			kingMock
				.SetupSet(p => p.Position = kingPreviousPosition);

			rookQueensideMock
				.SetupGet(p => p.Position)
				.Returns(rookQueensidePosition);
			rookQueensideMock
				.SetupGet(p => p.Color)
				.Returns(kingColor);
			rookQueensideMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Rook);

			rookKingsideMock
				.SetupGet(p => p.Position)
				.Returns(rookFinalPosition);
			rookKingsideMock
				.SetupGet(p => p.Color)
				.Returns(kingColor);
			rookKingsideMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Rook);
			rookKingsideMock
				.Setup(p => p.DecrementMoveCounter());
			rookKingsideMock
				.SetupSet(p => p.Position = rookKingsidePreviousPosition);

			var pieces = new List<IChessPiece>(){
				kingMock.Object, rookKingsideMock.Object,
				rookQueensideMock.Object
			};

			var mover = new CastlingMover(movementHistory.Object);
			var result = mover.UndoCastlingIfCastlingMove(chessMove, pieces);

			Assert.AreEqual(true, result);
			kingMock
				.VerifySet(p => p.Position = kingPreviousPosition);
			kingMock
				.Verify(p => p.DecrementMoveCounter());
			rookKingsideMock
				.VerifySet(p => p.Position = rookKingsidePreviousPosition);
			rookKingsideMock
				.Verify(p => p.DecrementMoveCounter());
		}
		[Test]
		public void UndoCastlingIfCastlingMove_QueensideCastleTest()
		{
			var movementHistory = new Mock<IMovementHistory>(MockBehavior.Strict);
			var kingMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var rookQueensideMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var rookKingsideMock = new Mock<IChessPiece>(MockBehavior.Strict);

			var kingPreviousPosition = new Position(4, 7);
			var rookQueensidePreviousPosition = new Position(0, 7);
			var rookKingsidePosition = new Position(7, 7);
			var rookFinalPosition = new Position(3, 7);
			var kingFinalPosition = new Position(2, 7);
			var kingColor = ChessColor.Black;
			var chessMove = new ChessMove(kingPreviousPosition, rookQueensidePreviousPosition);

			kingMock
				.SetupGet(p => p.Position)
				.Returns(kingFinalPosition);
			kingMock
				.SetupGet(p => p.Color)
				.Returns(kingColor);
			kingMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.King);
			kingMock
				.Setup(p => p.DecrementMoveCounter());
			kingMock
				.SetupSet(p => p.Position = kingPreviousPosition);

			rookQueensideMock
				.SetupGet(p => p.Position)
				.Returns(rookFinalPosition);
			rookQueensideMock
				.SetupGet(p => p.Color)
				.Returns(kingColor);
			rookQueensideMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Rook);
			rookQueensideMock
				.Setup(p => p.DecrementMoveCounter());
			rookQueensideMock
				.SetupSet(p => p.Position = rookQueensidePreviousPosition);


			rookKingsideMock
				.SetupGet(p => p.Position)
				.Returns(rookKingsidePosition);
			rookKingsideMock
				.SetupGet(p => p.Color)
				.Returns(kingColor);
			rookKingsideMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Rook);

			var pieces = new List<IChessPiece>(){
				kingMock.Object, rookKingsideMock.Object,
				rookQueensideMock.Object
			};

			var mover = new CastlingMover(movementHistory.Object);
			var result = mover.UndoCastlingIfCastlingMove(chessMove, pieces);

			Assert.AreEqual(true, result);
			kingMock
				.VerifySet(p => p.Position = kingPreviousPosition);
			kingMock
				.Verify(p => p.DecrementMoveCounter());
			rookQueensideMock
				.VerifySet(p => p.Position = rookQueensidePreviousPosition);
			rookQueensideMock
				.Verify(p => p.DecrementMoveCounter());
		}
	}
}