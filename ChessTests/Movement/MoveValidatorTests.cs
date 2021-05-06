using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Chess;
using Chess.Pieces;
using Chess.Board;
using Chess.Movement;

namespace ChessTests
{
	public class MoveValidatorTests
	{
		[Test]
		public void WhenMoveIsLegalThenMoveAndReturnTrue()
		{
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var promotionDetectorMock = new Mock<IPromotionDetector>(MockBehavior.Strict);
			var movementMock = new Mock<ILegalMovement>(MockBehavior.Strict);
			var movedPieceMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);

			var currentPlayer = ChessColor.White;
			var piecePosition = new Position(0, 1);
			var pieceDestination = new Position(0, 3);

			var chessMove = new ChessMove(piecePosition, pieceDestination);

			movedPieceMock
				.SetupGet(p => p.Color)
				.Returns(currentPlayer);
			movedPieceMock
				.SetupGet(p => p.Position)
				.Returns(piecePosition);

			boardMock
				.Setup(b => b.Move(chessMove));
			boardMock
				.SetupGet(b => b.Pieces)
				.Returns(new List<IReadOnlyChessPiece>() { movedPieceMock.Object });

			promotionDetectorMock
				.Setup(c => c.IsPromotionRequired())
				.Returns(false);

			movementMock
				.Setup(m => m.GetAvailableLegalMoves(movedPieceMock.Object))
				.Returns(new List<ChessMove>() { chessMove });

			var moveValidator = new MoveValidator(boardMock.Object, movementMock.Object,
				promotionDetectorMock.Object);

			var result = moveValidator.ValidateAndMove(chessMove, currentPlayer);

			Assert.AreEqual(true, result);
			boardMock
				.Verify(b => b.Move(chessMove));
		}
		[Test]
		public void WhenMoveIsIllegalReturnFalse()
		{
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var promotionDetectorMock = new Mock<IPromotionDetector>(MockBehavior.Strict);
			var movementMock = new Mock<ILegalMovement>(MockBehavior.Strict);
			var movedPieceMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);

			var currentPlayer = ChessColor.Black;
			var piecePosition = new Position(0, 1);
			var pieceDestination = new Position(0, 3);

			var illegalChessMove = new ChessMove(piecePosition, pieceDestination);
			var legalChessMove = new ChessMove(new Position(0, 3), new Position(0, 1));

			movedPieceMock
				.SetupGet(p => p.Color)
				.Returns(currentPlayer);
			movedPieceMock
				.SetupGet(p => p.Position)
				.Returns(piecePosition);

			boardMock
				.SetupGet(b => b.Pieces)
				.Returns(new List<IReadOnlyChessPiece>() { movedPieceMock.Object });

			promotionDetectorMock
				.Setup(c => c.IsPromotionRequired())
				.Returns(false);

			movementMock
				.Setup(m => m.GetAvailableLegalMoves(movedPieceMock.Object))
				.Returns(new List<ChessMove>() { legalChessMove });

			var moveValidator = new MoveValidator(boardMock.Object, movementMock.Object,
				promotionDetectorMock.Object);

			var result = moveValidator.ValidateAndMove(illegalChessMove, currentPlayer);

			Assert.AreEqual(false, result);
		}
		[Test]
		public void WhenPromotionIsRequiredAndPromotionMoveWasNotGivenReturnFalse()
		{
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var promotionDetectorMock = new Mock<IPromotionDetector>(MockBehavior.Strict);
			var movementMock = new Mock<ILegalMovement>(MockBehavior.Strict);

			var currentPlayer = ChessColor.White;
			var piecePosition = new Position(0, 1);
			var pieceDestination = new Position(0, 3);

			var chessMove = new ChessMove(piecePosition, pieceDestination);

			promotionDetectorMock
				.Setup(c => c.IsPromotionRequired())
				.Returns(true);

			var moveValidator = new MoveValidator(boardMock.Object, movementMock.Object,
				promotionDetectorMock.Object);

			var result = moveValidator.ValidateAndMove(chessMove, currentPlayer);

			Assert.AreEqual(false, result);
		}
		[Test]
		public void WhenPromotionIsRequiredAndPromotionMoveWasGivenPromoteAndReturnTrue()
		{
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var promotionDetectorMock = new Mock<IPromotionDetector>(MockBehavior.Strict);
			var movementMock = new Mock<ILegalMovement>(MockBehavior.Strict);

			var currentPlayer = ChessColor.White;
			var piecePosition = new Position(0, 1);
			var pieceDestination = new Position(0, 3);

			var chessMove = new ChessMove(piecePosition, pieceDestination,
				pawnPromotion: ChessPieceType.Queen);

			boardMock
				.Setup(b => b.Move(chessMove));

			promotionDetectorMock
				.Setup(c => c.IsPromotionRequired())
				.Returns(true);

			var moveValidator = new MoveValidator(boardMock.Object, movementMock.Object,
				promotionDetectorMock.Object);

			var result = moveValidator.ValidateAndMove(chessMove, currentPlayer);

			Assert.AreEqual(true, result);
			boardMock
				.Verify(b => b.Move(chessMove));
		}
		[Test]
		public void WhenTryingToMoveEmptySpaceReturnFalse()
		{
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var promotionDetectorMock = new Mock<IPromotionDetector>(MockBehavior.Strict);
			var movementMock = new Mock<ILegalMovement>(MockBehavior.Strict);
			var examplePieceMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);

			var currentPlayer = ChessColor.Black;
			var piecePosition = new Position(0, 1);
			var pieceDestination = new Position(0, 3);

			var chessMove = new ChessMove(new Position(1, 6), pieceDestination);

			examplePieceMock
				.SetupGet(p => p.Position)
				.Returns(piecePosition);

			boardMock
				.SetupGet(b => b.Pieces)
				.Returns(new List<IReadOnlyChessPiece>() { examplePieceMock.Object });

			promotionDetectorMock
				.Setup(c => c.IsPromotionRequired())
				.Returns(false);

			var moveValidator = new MoveValidator(boardMock.Object, movementMock.Object,
				promotionDetectorMock.Object);

			var result = moveValidator.ValidateAndMove(chessMove, currentPlayer);

			Assert.AreEqual(false, result);
		}
		[Test]
		public void WhenTryingToMoveEnemyPieceReturnFalse()
		{
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var promotionDetectorMock = new Mock<IPromotionDetector>(MockBehavior.Strict);
			var movementMock = new Mock<ILegalMovement>(MockBehavior.Strict);
			var movedPieceMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);

			var currentPlayer = ChessColor.White;
			var piecePosition = new Position(0, 1);
			var pieceDestination = new Position(0, 3);

			var chessMove = new ChessMove(piecePosition, pieceDestination);

			movedPieceMock
				.SetupGet(p => p.Color)
				.Returns(currentPlayer.Opposite());
			movedPieceMock
				.SetupGet(p => p.Position)
				.Returns(piecePosition);

			boardMock
				.SetupGet(b => b.Pieces)
				.Returns(new List<IReadOnlyChessPiece>() { movedPieceMock.Object });

			promotionDetectorMock
				.Setup(c => c.IsPromotionRequired())
				.Returns(false);

			var moveValidator = new MoveValidator(boardMock.Object, movementMock.Object,
				promotionDetectorMock.Object);

			var result = moveValidator.ValidateAndMove(chessMove, currentPlayer);

			Assert.AreEqual(false, result);
		}
	}
}