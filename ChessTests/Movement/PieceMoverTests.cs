using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Chess;
using Chess.Pieces;
using Chess.Movement;

namespace ChessTests
{
	public class PieceMoverTests
	{
		[Test]
		public void WhenPromotionMoveIsPassed_Move_ReturnsNull()
		{
			var movementHistory = new Mock<IMovementHistory>(MockBehavior.Strict);
			var promoterMock = new Mock<IPiecePromoter>(MockBehavior.Strict);
			var castlingMoverMock = new Mock<ICastlingMover>(MockBehavior.Strict);
			var enPassantMoverMock = new Mock<IEnPassantMover>(MockBehavior.Strict);

			var pieces = new List<IChessPiece>();
			var chessMove = new ChessMove(new Position(0, 0), new Position(0, 0),
				pawnPromotion: ChessPieceType.Knight);

			promoterMock
				.Setup(p => p.PromoteIfPromotionMove(chessMove, pieces))
				.Returns(true);

			var pieceMover = new PieceMover(movementHistory.Object, promoterMock.Object,
				castlingMoverMock.Object, enPassantMoverMock.Object);
			var result = pieceMover.Move(chessMove, pieces);

			Assert.AreEqual(null, result);
		}
		[Test]
		public void WhenCastlingMoveIsPassed_Move_ReturnsNull()
		{
			var movementHistory = new Mock<IMovementHistory>(MockBehavior.Strict);
			var promoterMock = new Mock<IPiecePromoter>(MockBehavior.Strict);
			var castlingMoverMock = new Mock<ICastlingMover>(MockBehavior.Strict);
			var enPassantMoverMock = new Mock<IEnPassantMover>(MockBehavior.Strict);

			var pieces = new List<IChessPiece>();
			var chessMove = new ChessMove(new Position(4, 7), new Position(0, 7));

			promoterMock
				.Setup(p => p.PromoteIfPromotionMove(chessMove, pieces))
				.Returns(false);

			castlingMoverMock
				.Setup(c => c.PerformCastlingIfCastlingMove(chessMove, pieces))
				.Returns(true);

			var pieceMover = new PieceMover(movementHistory.Object, promoterMock.Object,
				castlingMoverMock.Object, enPassantMoverMock.Object);
			var result = pieceMover.Move(chessMove, pieces);

			Assert.AreEqual(null, result);
		}
		[Test]
		public void WhenEnPassantMoveIsPassed_Move_ReturnsRemovedPawn()
		{
			var movementHistory = new Mock<IMovementHistory>(MockBehavior.Strict);
			var promoterMock = new Mock<IPiecePromoter>(MockBehavior.Strict);
			var castlingMoverMock = new Mock<ICastlingMover>(MockBehavior.Strict);
			var enPassantMoverMock = new Mock<IEnPassantMover>(MockBehavior.Strict);
			var pawnToRemoveMock = new Mock<IChessPiece>(MockBehavior.Strict);

			var pieces = new List<IChessPiece>();
			var chessMove = new ChessMove(new Position(4, 7), new Position(0, 7));

			promoterMock
				.Setup(p => p.PromoteIfPromotionMove(chessMove, pieces))
				.Returns(false);

			castlingMoverMock
				.Setup(c => c.PerformCastlingIfCastlingMove(chessMove, pieces))
				.Returns(false);

			enPassantMoverMock
				.Setup(e => e.PerformEnPassantIfApplicable(chessMove, pieces))
				.Returns(pawnToRemoveMock.Object);

			var pieceMover = new PieceMover(movementHistory.Object, promoterMock.Object,
				castlingMoverMock.Object, enPassantMoverMock.Object);
			var result = pieceMover.Move(chessMove, pieces);

			Assert.AreEqual(pawnToRemoveMock.Object, result);
		}
		[Test]
		public void WhenThereIsNoCapture_Move_ReturnsNull_IncrementsMoveCounter_AndAddsMoveToMovmentHistory()
		{
			var movementHistory = new Mock<IMovementHistory>(MockBehavior.Strict);
			var promoterMock = new Mock<IPiecePromoter>(MockBehavior.Strict);
			var castlingMoverMock = new Mock<ICastlingMover>(MockBehavior.Strict);
			var enPassantMoverMock = new Mock<IEnPassantMover>(MockBehavior.Strict);
			var movedPieceMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var otherPieceMock = new Mock<IChessPiece>(MockBehavior.Strict);

			var piecePosition = new Position(3, 1);
			var destination = new Position(3, 3);
			var chessMove = new ChessMove(piecePosition, destination);

			movedPieceMock
				.SetupGet(p => p.Position)
				.Returns(piecePosition);
			movedPieceMock
				.SetupSet(p => p.Position = destination);
			movedPieceMock
				.Setup(p => p.IncrementMoveCounter());

			otherPieceMock
				.SetupGet(p => p.Position)
				.Returns(new Position(1, 1));

			var pieces = new List<IChessPiece>(){
				movedPieceMock.Object,
				otherPieceMock.Object
			};

			promoterMock
				.Setup(p => p.PromoteIfPromotionMove(chessMove, pieces))
				.Returns(false);

			castlingMoverMock
				.Setup(c => c.PerformCastlingIfCastlingMove(chessMove, pieces))
				.Returns(false);

			enPassantMoverMock
				.Setup(e => e.PerformEnPassantIfApplicable(chessMove, pieces))
				.Returns<IChessPiece>(null);

			movementHistory
				.Setup(h => h.Add(chessMove));

			var pieceMover = new PieceMover(movementHistory.Object, promoterMock.Object,
				castlingMoverMock.Object, enPassantMoverMock.Object);
			var result = pieceMover.Move(chessMove, pieces);

			Assert.AreEqual(null, result);
			movedPieceMock
				.VerifySet(p => p.Position = destination);
			movedPieceMock
				.Verify(p => p.IncrementMoveCounter());
			movementHistory
				.Verify(h => h.Add(chessMove));
		}
		[Test]
		public void WhenThereIsCapture_Move_ReturnsCapturedPiece_IncrementsMoveCounter_AndAddsMoveToMovmentHistory()
		{
			var movementHistory = new Mock<IMovementHistory>(MockBehavior.Strict);
			var promoterMock = new Mock<IPiecePromoter>(MockBehavior.Strict);
			var castlingMoverMock = new Mock<ICastlingMover>(MockBehavior.Strict);
			var enPassantMoverMock = new Mock<IEnPassantMover>(MockBehavior.Strict);
			var movedPieceMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var otherPieceMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var capturedPieceMock = new Mock<IChessPiece>(MockBehavior.Strict);

			var piecePosition = new Position(3, 1);
			var destination = new Position(3, 3);
			var chessMove = new ChessMove(piecePosition, destination);

			movedPieceMock
				.SetupGet(p => p.Position)
				.Returns(piecePosition);
			movedPieceMock
				.SetupSet(p => p.Position = destination);
			movedPieceMock
				.Setup(p => p.IncrementMoveCounter());

			otherPieceMock
				.SetupGet(p => p.Position)
				.Returns(new Position(1, 1));
			capturedPieceMock
				.SetupGet(p => p.Position)
				.Returns(destination);

			var pieces = new List<IChessPiece>(){
				movedPieceMock.Object,
				otherPieceMock.Object,
				capturedPieceMock.Object
			};

			promoterMock
				.Setup(p => p.PromoteIfPromotionMove(chessMove, pieces))
				.Returns(false);

			castlingMoverMock
				.Setup(c => c.PerformCastlingIfCastlingMove(chessMove, pieces))
				.Returns(false);

			enPassantMoverMock
				.Setup(e => e.PerformEnPassantIfApplicable(chessMove, pieces))
				.Returns<IChessPiece>(null);

			movementHistory
				.Setup(h => h.Add(chessMove.ReturnWithCaptureAsTrue()));

			var pieceMover = new PieceMover(movementHistory.Object, promoterMock.Object,
				castlingMoverMock.Object, enPassantMoverMock.Object);
			var result = pieceMover.Move(chessMove, pieces);

			Assert.AreEqual(capturedPieceMock.Object, result);
			movedPieceMock
				.VerifySet(p => p.Position = destination);
			movedPieceMock
				.Verify(p => p.IncrementMoveCounter());
			movementHistory
				.Verify(h => h.Add(chessMove.ReturnWithCaptureAsTrue()));
		}
		[Test]
		public void WhenPromotionMoveWasLast_ReverseLastMove_ReversesPawnMove_AndReturnsWhetherItWasCaptureMove()
		{
			var movementHistory = new Mock<IMovementHistory>(MockBehavior.Strict);
			var promoterMock = new Mock<IPiecePromoter>(MockBehavior.Strict);
			var castlingMoverMock = new Mock<ICastlingMover>(MockBehavior.Strict);
			var enPassantMoverMock = new Mock<IEnPassantMover>(MockBehavior.Strict);
			var pawnMock = new Mock<IChessPiece>(MockBehavior.Strict);

			var pawnPreviousPosition = new Position(3, 6);
			var pawnCurrentPosition = new Position(3, 7);
			var promotionMove = new ChessMove(new Position(0, 0),
				new Position(0, 0), pawnPromotion: ChessPieceType.Knight);
			var pawnMoveBeforePromotion = new ChessMove(pawnPreviousPosition,
				pawnCurrentPosition);

			pawnMock
				.SetupGet(p => p.Position)
				.Returns(pawnCurrentPosition);
			pawnMock
				.SetupSet(p => p.Position = pawnPreviousPosition);
			pawnMock
				.Setup(p => p.DecrementMoveCounter());

			var pieces = new List<IChessPiece>(){
				pawnMock.Object
			};

			promoterMock
				.Setup(p => p.DepromoteIfPromotionMove(promotionMove, pieces))
				.Returns(pawnMoveBeforePromotion);

			castlingMoverMock
				.Setup(c => c.UndoCastlingIfCastlingMove(promotionMove, pieces))
				.Returns(false);

			movementHistory
				.Setup(h => h.RemoveLastMove())
				.Returns(promotionMove);

			var pieceMover = new PieceMover(movementHistory.Object, promoterMock.Object,
				castlingMoverMock.Object, enPassantMoverMock.Object);
			var result = pieceMover.ReverseLastMove(pieces);

			Assert.AreEqual(pawnMoveBeforePromotion.IsCapture, result);
			pawnMock
				.VerifySet(p => p.Position = pawnPreviousPosition);
			pawnMock
				.Verify(p => p.DecrementMoveCounter());
			movementHistory
				.Verify(h => h.RemoveLastMove());
		}
		[Test]
		public void WhenCastlingMoveWasLast_ReverseLastMove_ReturnsFalse()
		{
			var movementHistory = new Mock<IMovementHistory>(MockBehavior.Strict);
			var promoterMock = new Mock<IPiecePromoter>(MockBehavior.Strict);
			var castlingMoverMock = new Mock<ICastlingMover>(MockBehavior.Strict);
			var enPassantMoverMock = new Mock<IEnPassantMover>(MockBehavior.Strict);

			var pieces = new List<IChessPiece>();
			var castlingMove = new ChessMove(new Position(4, 7), new Position(0, 7));

			castlingMoverMock
				.Setup(c => c.UndoCastlingIfCastlingMove(castlingMove, pieces))
				.Returns(true);

			movementHistory
				.Setup(h => h.RemoveLastMove())
				.Returns(castlingMove);

			var pieceMover = new PieceMover(movementHistory.Object, promoterMock.Object,
				castlingMoverMock.Object, enPassantMoverMock.Object);
			var result = pieceMover.ReverseLastMove(pieces);

			Assert.AreEqual(false, result);
		}
		[Test]
		public void WhenThereWasNoCapture_ReverseLastMove_ReturnsFalse_DecrementsMoveCounter_AndRemovesMoveFromMovmentHistory()
		{
			var movementHistory = new Mock<IMovementHistory>(MockBehavior.Strict);
			var promoterMock = new Mock<IPiecePromoter>(MockBehavior.Strict);
			var castlingMoverMock = new Mock<ICastlingMover>(MockBehavior.Strict);
			var enPassantMoverMock = new Mock<IEnPassantMover>(MockBehavior.Strict);
			var movedPieceMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var otherPieceMock = new Mock<IChessPiece>(MockBehavior.Strict);

			var piecePreviousPosition = new Position(3, 1);
			var pieceCurrentPosition = new Position(3, 3);
			var lastChessMove = new ChessMove(piecePreviousPosition,
				pieceCurrentPosition);

			movedPieceMock
				.SetupGet(p => p.Position)
				.Returns(pieceCurrentPosition);
			movedPieceMock
				.SetupSet(p => p.Position = piecePreviousPosition);
			movedPieceMock
				.Setup(p => p.DecrementMoveCounter());

			otherPieceMock
				.SetupGet(p => p.Position)
				.Returns(new Position(1, 1));

			var pieces = new List<IChessPiece>(){
				movedPieceMock.Object,
				otherPieceMock.Object
			};

			promoterMock
				.Setup(p => p.DepromoteIfPromotionMove(lastChessMove, pieces))
				.Returns(lastChessMove);

			castlingMoverMock
				.Setup(c => c.UndoCastlingIfCastlingMove(lastChessMove, pieces))
				.Returns(false);

			movementHistory
				.Setup(h => h.RemoveLastMove())
				.Returns(lastChessMove);

			var pieceMover = new PieceMover(movementHistory.Object, promoterMock.Object,
				castlingMoverMock.Object, enPassantMoverMock.Object);
			var result = pieceMover.ReverseLastMove(pieces);

			Assert.AreEqual(false, result);
			movedPieceMock
				.VerifySet(p => p.Position = piecePreviousPosition);
			movedPieceMock
				.Verify(p => p.DecrementMoveCounter());
			movementHistory
				.Verify(h => h.RemoveLastMove());
		}
		[Test]
		public void WhenThereWasCapture_ReverseLastMove_ReturnsTrue_DecrementsMoveCounter_AndRemovesMoveFromMovmentHistory()
		{
			var movementHistory = new Mock<IMovementHistory>(MockBehavior.Strict);
			var promoterMock = new Mock<IPiecePromoter>(MockBehavior.Strict);
			var castlingMoverMock = new Mock<ICastlingMover>(MockBehavior.Strict);
			var enPassantMoverMock = new Mock<IEnPassantMover>(MockBehavior.Strict);
			var movedPieceMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var otherPieceMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var capturedPieceMock = new Mock<IChessPiece>(MockBehavior.Strict);

			var piecePreviousPosition = new Position(3, 1);
			var pieceCurrentPosition = new Position(3, 3);
			var lastChessMove = new ChessMove(piecePreviousPosition,
				pieceCurrentPosition, true);

			movedPieceMock
				.SetupGet(p => p.Position)
				.Returns(pieceCurrentPosition);
			movedPieceMock
				.SetupSet(p => p.Position = piecePreviousPosition);
			movedPieceMock
				.Setup(p => p.DecrementMoveCounter());

			otherPieceMock
				.SetupGet(p => p.Position)
				.Returns(new Position(1, 1));

			var pieces = new List<IChessPiece>(){
				movedPieceMock.Object,
				otherPieceMock.Object
			};

			promoterMock
				.Setup(p => p.DepromoteIfPromotionMove(lastChessMove, pieces))
				.Returns(lastChessMove);

			castlingMoverMock
				.Setup(c => c.UndoCastlingIfCastlingMove(lastChessMove, pieces))
				.Returns(false);

			movementHistory
				.Setup(h => h.RemoveLastMove())
				.Returns(lastChessMove);

			var pieceMover = new PieceMover(movementHistory.Object, promoterMock.Object,
				castlingMoverMock.Object, enPassantMoverMock.Object);
			var result = pieceMover.ReverseLastMove(pieces);

			Assert.AreEqual(true, result);
			movedPieceMock
				.VerifySet(p => p.Position = piecePreviousPosition);
			movedPieceMock
				.Verify(p => p.DecrementMoveCounter());
			movementHistory
				.Verify(h => h.RemoveLastMove());
		}
	}
}