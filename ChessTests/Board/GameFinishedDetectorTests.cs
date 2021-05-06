using Moq;
using NUnit.Framework;
using Chess.Game;
using Chess.Board;
using Chess.Pieces;
using Chess.Movement;

namespace ChessTests
{
	public class GameFinishedDetectorTests
	{
		[Test]
		public void IfWhitePlayerIsNotCheckedAndHasLegalMovesReturnsSuccessfulMove()
		{
			var checkDetectorMock = new Mock<ICheckDetector>(MockBehavior.Strict);
			var legalMovementMock = new Mock<ILegalMovement>(MockBehavior.Strict);

			var colorToCheck = ChessColor.White;

			checkDetectorMock
				.Setup(d => d.IsChecked(colorToCheck))
				.Returns(false);

			legalMovementMock
				.Setup(m => m.HasAnyLegalMoves(colorToCheck))
				.Returns(true);

			var detector = new GameFinishedDetector(checkDetectorMock.Object,
				legalMovementMock.Object);

			var result = detector.IsGameFinished(colorToCheck);

			Assert.AreEqual(ChessPlayResult.SuccessfulMove, result);
		}
		[Test]
		public void IfWhitePlayerIsCheckedAndHasLegalMovesReturnsWhiteChecked()
		{
			var checkDetectorMock = new Mock<ICheckDetector>(MockBehavior.Strict);
			var legalMovementMock = new Mock<ILegalMovement>(MockBehavior.Strict);

			var colorToCheck = ChessColor.White;

			checkDetectorMock
				.Setup(d => d.IsChecked(colorToCheck))
				.Returns(true);

			legalMovementMock
				.Setup(m => m.HasAnyLegalMoves(colorToCheck))
				.Returns(true);

			var detector = new GameFinishedDetector(checkDetectorMock.Object,
				legalMovementMock.Object);

			var result = detector.IsGameFinished(colorToCheck);

			Assert.AreEqual(ChessPlayResult.WhiteChecked, result);
		}
		[Test]
		public void IfWhitePlayerIsCheckedAndDoesNotHaveLegalMovesReturnsBlackWin()
		{
			var checkDetectorMock = new Mock<ICheckDetector>(MockBehavior.Strict);
			var legalMovementMock = new Mock<ILegalMovement>(MockBehavior.Strict);

			var colorToCheck = ChessColor.White;

			checkDetectorMock
				.Setup(d => d.IsChecked(colorToCheck))
				.Returns(true);

			legalMovementMock
				.Setup(m => m.HasAnyLegalMoves(colorToCheck))
				.Returns(false);

			var detector = new GameFinishedDetector(checkDetectorMock.Object,
				legalMovementMock.Object);

			var result = detector.IsGameFinished(colorToCheck);

			Assert.AreEqual(ChessPlayResult.BlackWin, result);
		}
		[Test]
		public void IfWhitePlayerIsNotCheckedAndDoesNotHaveLegalMovesReturnsStalemate()
		{
			var checkDetectorMock = new Mock<ICheckDetector>(MockBehavior.Strict);
			var legalMovementMock = new Mock<ILegalMovement>(MockBehavior.Strict);

			var colorToCheck = ChessColor.White;

			checkDetectorMock
				.Setup(d => d.IsChecked(colorToCheck))
				.Returns(false);

			legalMovementMock
				.Setup(m => m.HasAnyLegalMoves(colorToCheck))
				.Returns(false);

			var detector = new GameFinishedDetector(checkDetectorMock.Object,
				legalMovementMock.Object);

			var result = detector.IsGameFinished(colorToCheck);

			Assert.AreEqual(ChessPlayResult.Stalemate, result);
		}
		[Test]
		public void IfBlackPlayerIsNotCheckedAndHasLegalMovesReturnsSuccessfulMove()
		{
			var checkDetectorMock = new Mock<ICheckDetector>(MockBehavior.Strict);
			var legalMovementMock = new Mock<ILegalMovement>(MockBehavior.Strict);

			var colorToCheck = ChessColor.Black;

			checkDetectorMock
				.Setup(d => d.IsChecked(colorToCheck))
				.Returns(false);

			legalMovementMock
				.Setup(m => m.HasAnyLegalMoves(colorToCheck))
				.Returns(true);

			var detector = new GameFinishedDetector(checkDetectorMock.Object,
				legalMovementMock.Object);

			var result = detector.IsGameFinished(colorToCheck);

			Assert.AreEqual(ChessPlayResult.SuccessfulMove, result);
		}
		[Test]
		public void IfBlackPlayerIsCheckedAndHasLegalMovesReturnsWhiteChecked()
		{
			var checkDetectorMock = new Mock<ICheckDetector>(MockBehavior.Strict);
			var legalMovementMock = new Mock<ILegalMovement>(MockBehavior.Strict);

			var colorToCheck = ChessColor.Black;

			checkDetectorMock
				.Setup(d => d.IsChecked(colorToCheck))
				.Returns(true);

			legalMovementMock
				.Setup(m => m.HasAnyLegalMoves(colorToCheck))
				.Returns(true);

			var detector = new GameFinishedDetector(checkDetectorMock.Object,
				legalMovementMock.Object);

			var result = detector.IsGameFinished(colorToCheck);

			Assert.AreEqual(ChessPlayResult.BlackChecked, result);
		}
		[Test]
		public void IfBlackPlayerIsCheckedAndDoesNotHaveLegalMovesReturnsBlackWin()
		{
			var checkDetectorMock = new Mock<ICheckDetector>(MockBehavior.Strict);
			var legalMovementMock = new Mock<ILegalMovement>(MockBehavior.Strict);

			var colorToCheck = ChessColor.Black;

			checkDetectorMock
				.Setup(d => d.IsChecked(colorToCheck))
				.Returns(true);

			legalMovementMock
				.Setup(m => m.HasAnyLegalMoves(colorToCheck))
				.Returns(false);

			var detector = new GameFinishedDetector(checkDetectorMock.Object,
				legalMovementMock.Object);

			var result = detector.IsGameFinished(colorToCheck);

			Assert.AreEqual(ChessPlayResult.WhiteWin, result);
		}
		[Test]
		public void IfBlackPlayerIsNotCheckedAndDoesNotHaveLegalMovesReturnsStalemate()
		{
			var checkDetectorMock = new Mock<ICheckDetector>(MockBehavior.Strict);
			var legalMovementMock = new Mock<ILegalMovement>(MockBehavior.Strict);

			var colorToCheck = ChessColor.Black;

			checkDetectorMock
				.Setup(d => d.IsChecked(colorToCheck))
				.Returns(false);

			legalMovementMock
				.Setup(m => m.HasAnyLegalMoves(colorToCheck))
				.Returns(false);

			var detector = new GameFinishedDetector(checkDetectorMock.Object,
				legalMovementMock.Object);

			var result = detector.IsGameFinished(colorToCheck);

			Assert.AreEqual(ChessPlayResult.Stalemate, result);
		}
	}
}