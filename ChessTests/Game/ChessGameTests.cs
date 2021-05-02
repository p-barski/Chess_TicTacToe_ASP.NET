using Moq;
using NUnit.Framework;
using Chess;
using Chess.Game;
using Chess.Board;
using Chess.Pieces;
using Chess.Movement;

namespace ChessTests
{
	public class ChessGameTests
	{
		[Test]
		public void WhitePlayerStartsTheGame()
		{
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var validatorMock = new Mock<IMoveValidator>(MockBehavior.Strict);
			var promotionDetectorMock =
				new Mock<IPromotionDetector>(MockBehavior.Strict);
			var gameFinishedDetectorMock =
				new Mock<IGameFinishedDetector>(MockBehavior.Strict);
			var legalMovementMock = new Mock<ILegalMovement>(MockBehavior.Strict);
			var chessMove =
				new ChessMove(new Position(0, 1), new Position(0, 3), false);

			var game = new ChessGame(boardMock.Object, validatorMock.Object,
				promotionDetectorMock.Object, gameFinishedDetectorMock.Object,
				legalMovementMock.Object);
			var result = game.Play(chessMove, ChessColor.Black);

			Assert.AreEqual(ChessPlayResult.WrongPlayer, result);
			Assert.AreEqual(ChessColor.White, game.CurrentPlayer);
		}
		[Test]
		public void GivingCorrectChessMoveAndCorrectPlayerColorShouldReturnSuccessfulMove()
		{
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var validatorMock = new Mock<IMoveValidator>(MockBehavior.Strict);
			var promotionDetectorMock =
				new Mock<IPromotionDetector>(MockBehavior.Strict);
			var gameFinishedDetectorMock =
				new Mock<IGameFinishedDetector>(MockBehavior.Strict);
			var legalMovementMock = new Mock<ILegalMovement>(MockBehavior.Strict);

			var startingPosition = new Position(0, 1);
			var finishedPosition = new Position(0, 2);
			var chessMove = new ChessMove(startingPosition, finishedPosition, false);

			boardMock
				.Setup(m => m.Move(chessMove));

			validatorMock
				.Setup(v => v.ValidateAndMove(chessMove, ChessColor.White))
				.Returns(true);

			promotionDetectorMock
				.Setup(d => d.IsPromotionRequired())
				.Returns(false);

			gameFinishedDetectorMock
				.Setup(d => d.IsGameFinished(ChessColor.Black))
				.Returns(ChessPlayResult.SuccessfulMove);

			var game = new ChessGame(boardMock.Object, validatorMock.Object,
				promotionDetectorMock.Object, gameFinishedDetectorMock.Object,
				legalMovementMock.Object);
			var result = game.Play(chessMove, ChessColor.White);

			Assert.AreEqual(ChessPlayResult.SuccessfulMove, result);
		}
		[Test]
		public void WhenCheckForBlackIsDetectedReturnBlackChecked()
		{
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var validatorMock = new Mock<IMoveValidator>(MockBehavior.Strict);
			var promotionDetectorMock =
				new Mock<IPromotionDetector>(MockBehavior.Strict);
			var gameFinishedDetectorMock =
				new Mock<IGameFinishedDetector>(MockBehavior.Strict);
			var legalMovementMock = new Mock<ILegalMovement>(MockBehavior.Strict);

			var startingPosition = new Position(0, 1);
			var finishedPosition = new Position(0, 2);
			var chessMove = new ChessMove(startingPosition, finishedPosition, false);

			boardMock
				.Setup(m => m.Move(chessMove));

			validatorMock
				.Setup(v => v.ValidateAndMove(chessMove, ChessColor.White))
				.Returns(true);

			promotionDetectorMock
				.Setup(d => d.IsPromotionRequired())
				.Returns(false);

			gameFinishedDetectorMock
				.Setup(d => d.IsGameFinished(ChessColor.Black))
				.Returns(ChessPlayResult.BlackChecked);

			var game = new ChessGame(boardMock.Object, validatorMock.Object,
				promotionDetectorMock.Object, gameFinishedDetectorMock.Object,
				legalMovementMock.Object);
			var result = game.Play(chessMove, ChessColor.White);

			Assert.AreEqual(ChessPlayResult.BlackChecked, result);
		}
	}
}