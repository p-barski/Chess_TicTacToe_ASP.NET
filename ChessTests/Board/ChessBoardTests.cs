using System;
using System.Linq;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Chess;
using Chess.Board;
using Chess.Pieces;
using Chess.Movement;

namespace ChessTests
{
	public class ChessBoardTests
	{
		[Test]
		public void WhenThereAreNoKingsCreatedThrowsException()
		{
			var pieceMoverMock = new Mock<IPieceMover>(MockBehavior.Strict);
			var piecesFactoryMock = new Mock<IPiecesFactory>(MockBehavior.Strict);

			piecesFactoryMock
				.Setup(f => f.Create())
				.Returns(new List<IChessPiece>());

			Assert.Throws<InvalidOperationException>(
				() => new ChessBoard(
					piecesFactoryMock.Object,
					pieceMoverMock.Object));
		}
		[Test]
		public void ReturnsHistoryFromIPieceMover()
		{
			var whiteKingMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var blackKingMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var pawnMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var pieceMoverMock = new Mock<IPieceMover>(MockBehavior.Strict);
			var piecesFactoryMock = new Mock<IPiecesFactory>(MockBehavior.Strict);
			var historyMock = new Mock<IReadOnlyMovementHistory>(MockBehavior.Strict);

			whiteKingMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.White);
			whiteKingMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.King);

			blackKingMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.Black);
			blackKingMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.King);

			pawnMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.Black);
			pawnMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.Pawn);

			var pieces = new HashSet<IChessPiece>();

			pieces.Add(whiteKingMock.Object);
			pieces.Add(blackKingMock.Object);
			pieces.Add(pawnMock.Object);

			piecesFactoryMock
				.Setup(f => f.Create())
				.Returns(pieces);

			pieceMoverMock
				.SetupGet(p => p.History)
				.Returns(historyMock.Object);

			var chessBoard = new ChessBoard(piecesFactoryMock.Object, pieceMoverMock.Object);

			Assert.AreEqual(historyMock.Object, chessBoard.History);
		}
		[Test]
		public void ReturnsCorrectKings()
		{
			var whiteKingMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var blackKingMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var pawnMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var pieceMoverMock = new Mock<IPieceMover>(MockBehavior.Strict);
			var piecesFactoryMock = new Mock<IPiecesFactory>(MockBehavior.Strict);

			whiteKingMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.White);
			whiteKingMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.King);

			blackKingMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.Black);
			blackKingMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.King);

			pawnMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.Black);
			pawnMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.Pawn);

			var pieces = new HashSet<IChessPiece>();

			pieces.Add(whiteKingMock.Object);
			pieces.Add(blackKingMock.Object);
			pieces.Add(pawnMock.Object);

			piecesFactoryMock
				.Setup(f => f.Create())
				.Returns(pieces);

			var chessBoard = new ChessBoard(piecesFactoryMock.Object, pieceMoverMock.Object);

			Assert.AreEqual(blackKingMock.Object, chessBoard.GetKing(ChessColor.Black));
			Assert.AreEqual(whiteKingMock.Object, chessBoard.GetKing(ChessColor.White));
		}
		[Test]
		public void WhenPositionIsTaken_IsPositionTaken_ReturnsTrue()
		{
			var whiteKingMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var blackKingMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var pawnMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var pieceMoverMock = new Mock<IPieceMover>(MockBehavior.Strict);
			var piecesFactoryMock = new Mock<IPiecesFactory>(MockBehavior.Strict);

			var whiteKingPosition = new Position(4, 0);
			var blackKingPosition = new Position(4, 7);
			var pawnPosition = new Position(0, 6);

			whiteKingMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.White);
			whiteKingMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.King);
			whiteKingMock
				.SetupGet(k => k.Position)
				.Returns(whiteKingPosition);

			blackKingMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.Black);
			blackKingMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.King);
			blackKingMock
				.SetupGet(k => k.Position)
				.Returns(blackKingPosition);

			pawnMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.Black);
			pawnMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.Pawn);
			pawnMock
				.SetupGet(k => k.Position)
				.Returns(pawnPosition);

			var pieces = new HashSet<IChessPiece>();

			pieces.Add(whiteKingMock.Object);
			pieces.Add(blackKingMock.Object);
			pieces.Add(pawnMock.Object);

			piecesFactoryMock
				.Setup(f => f.Create())
				.Returns(pieces);

			var chessBoard = new ChessBoard(piecesFactoryMock.Object, pieceMoverMock.Object);

			Assert.AreEqual(true, chessBoard.IsPositionTaken(whiteKingPosition));
			Assert.AreEqual(true, chessBoard.IsPositionTaken(blackKingPosition));
			Assert.AreEqual(true, chessBoard.IsPositionTaken(pawnPosition));
		}
		[Test]
		public void WhenPositionIsNotTaken_IsPositionTaken_ReturnsFalse()
		{
			var whiteKingMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var blackKingMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var pawnMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var pieceMoverMock = new Mock<IPieceMover>(MockBehavior.Strict);
			var piecesFactoryMock = new Mock<IPiecesFactory>(MockBehavior.Strict);

			var whiteKingPosition = new Position(4, 0);
			var blackKingPosition = new Position(4, 7);
			var pawnPosition = new Position(0, 6);

			var emptyPosition1 = new Position(1, 1);
			var emptyPosition2 = new Position(5, 3);
			var emptyPosition3 = new Position(6, 5);

			whiteKingMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.White);
			whiteKingMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.King);
			whiteKingMock
				.SetupGet(k => k.Position)
				.Returns(whiteKingPosition);

			blackKingMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.Black);
			blackKingMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.King);
			blackKingMock
				.SetupGet(k => k.Position)
				.Returns(blackKingPosition);

			pawnMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.Black);
			pawnMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.Pawn);
			pawnMock
				.SetupGet(k => k.Position)
				.Returns(pawnPosition);

			var pieces = new HashSet<IChessPiece>();

			pieces.Add(whiteKingMock.Object);
			pieces.Add(blackKingMock.Object);
			pieces.Add(pawnMock.Object);

			piecesFactoryMock
				.Setup(f => f.Create())
				.Returns(pieces);

			var chessBoard = new ChessBoard(piecesFactoryMock.Object, pieceMoverMock.Object);

			Assert.AreEqual(false, chessBoard.IsPositionTaken(emptyPosition1));
			Assert.AreEqual(false, chessBoard.IsPositionTaken(emptyPosition2));
			Assert.AreEqual(false, chessBoard.IsPositionTaken(emptyPosition3));
		}
		[Test]
		public void WhenThereIsEnemyOnGivenPosition_IsEnemyOnPosition_ReturnsTrue()
		{
			var whiteKingMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var blackKingMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var pawnMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var pieceMoverMock = new Mock<IPieceMover>(MockBehavior.Strict);
			var piecesFactoryMock = new Mock<IPiecesFactory>(MockBehavior.Strict);

			var whiteKingPosition = new Position(4, 0);
			var blackKingPosition = new Position(4, 7);
			var pawnPosition = new Position(0, 6);

			whiteKingMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.White);
			whiteKingMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.King);
			whiteKingMock
				.SetupGet(k => k.Position)
				.Returns(whiteKingPosition);

			blackKingMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.Black);
			blackKingMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.King);
			blackKingMock
				.SetupGet(k => k.Position)
				.Returns(blackKingPosition);

			pawnMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.Black);
			pawnMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.Pawn);
			pawnMock
				.SetupGet(k => k.Position)
				.Returns(pawnPosition);

			var pieces = new HashSet<IChessPiece>();

			pieces.Add(whiteKingMock.Object);
			pieces.Add(blackKingMock.Object);
			pieces.Add(pawnMock.Object);

			piecesFactoryMock
				.Setup(f => f.Create())
				.Returns(pieces);

			var chessBoard = new ChessBoard(piecesFactoryMock.Object, pieceMoverMock.Object);

			Assert.AreEqual(true, chessBoard.IsEnemyOnPosition(whiteKingPosition,
				ChessColor.White));
			Assert.AreEqual(true, chessBoard.IsEnemyOnPosition(blackKingPosition,
				ChessColor.Black));
			Assert.AreEqual(true, chessBoard.IsEnemyOnPosition(pawnPosition,
				ChessColor.Black));
		}
		[Test]
		public void WhenThereIsFriendlyPieceOnGivenPosition_IsEnemyOnPosition_ReturnsFalse()
		{
			var whiteKingMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var blackKingMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var pawnMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var pieceMoverMock = new Mock<IPieceMover>(MockBehavior.Strict);
			var piecesFactoryMock = new Mock<IPiecesFactory>(MockBehavior.Strict);

			var whiteKingPosition = new Position(4, 0);
			var blackKingPosition = new Position(4, 7);
			var pawnPosition = new Position(0, 6);

			whiteKingMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.White);
			whiteKingMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.King);
			whiteKingMock
				.SetupGet(k => k.Position)
				.Returns(whiteKingPosition);

			blackKingMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.Black);
			blackKingMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.King);
			blackKingMock
				.SetupGet(k => k.Position)
				.Returns(blackKingPosition);

			pawnMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.Black);
			pawnMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.Pawn);
			pawnMock
				.SetupGet(k => k.Position)
				.Returns(pawnPosition);

			var pieces = new HashSet<IChessPiece>();

			pieces.Add(whiteKingMock.Object);
			pieces.Add(blackKingMock.Object);
			pieces.Add(pawnMock.Object);

			piecesFactoryMock
				.Setup(f => f.Create())
				.Returns(pieces);

			var chessBoard = new ChessBoard(piecesFactoryMock.Object, pieceMoverMock.Object);

			Assert.AreEqual(false, chessBoard.IsEnemyOnPosition(whiteKingPosition,
				ChessColor.Black));
			Assert.AreEqual(false, chessBoard.IsEnemyOnPosition(blackKingPosition,
				ChessColor.White));
			Assert.AreEqual(false, chessBoard.IsEnemyOnPosition(pawnPosition,
				ChessColor.White));
		}
		[Test]
		public void WhenThereIsNoChessPieceOnPosition_IsEnemyOnPosition_ReturnsFalse()
		{
			var whiteKingMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var blackKingMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var pawnMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var pieceMoverMock = new Mock<IPieceMover>(MockBehavior.Strict);
			var piecesFactoryMock = new Mock<IPiecesFactory>(MockBehavior.Strict);

			var whiteKingPosition = new Position(4, 0);
			var blackKingPosition = new Position(4, 7);
			var pawnPosition = new Position(0, 6);

			var emptyPosition1 = new Position(1, 1);
			var emptyPosition2 = new Position(5, 3);
			var emptyPosition3 = new Position(6, 5);

			whiteKingMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.White);
			whiteKingMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.King);
			whiteKingMock
				.SetupGet(k => k.Position)
				.Returns(whiteKingPosition);

			blackKingMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.Black);
			blackKingMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.King);
			blackKingMock
				.SetupGet(k => k.Position)
				.Returns(blackKingPosition);

			pawnMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.Black);
			pawnMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.Pawn);
			pawnMock
				.SetupGet(k => k.Position)
				.Returns(pawnPosition);

			var pieces = new HashSet<IChessPiece>();

			pieces.Add(whiteKingMock.Object);
			pieces.Add(blackKingMock.Object);
			pieces.Add(pawnMock.Object);

			piecesFactoryMock
				.Setup(f => f.Create())
				.Returns(pieces);

			var chessBoard = new ChessBoard(piecesFactoryMock.Object, pieceMoverMock.Object);

			Assert.AreEqual(false, chessBoard.IsEnemyOnPosition(emptyPosition1,
				ChessColor.White));
			Assert.AreEqual(false, chessBoard.IsEnemyOnPosition(emptyPosition2,
				ChessColor.Black));
			Assert.AreEqual(false, chessBoard.IsEnemyOnPosition(emptyPosition3,
				ChessColor.Black));
		}
		[Test]
		public void WhenThereIsNoCapture_MovingPieceWontRemoveAnyPieceFromAvailablePieces()
		{
			var whiteKingMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var blackKingMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var pawnMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var pieceMoverMock = new Mock<IPieceMover>(MockBehavior.Strict);
			var piecesFactoryMock = new Mock<IPiecesFactory>(MockBehavior.Strict);

			var whiteKingPosition = new Position(4, 0);
			var blackKingPosition = new Position(4, 7);
			var pawnPosition = new Position(0, 6);

			var chessMove = new ChessMove(whiteKingPosition, new Position(4, 1));

			whiteKingMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.White);
			whiteKingMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.King);
			whiteKingMock
				.SetupGet(k => k.Position)
				.Returns(whiteKingPosition);

			blackKingMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.Black);
			blackKingMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.King);
			blackKingMock
				.SetupGet(k => k.Position)
				.Returns(blackKingPosition);

			pawnMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.Black);
			pawnMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.Pawn);
			pawnMock
				.SetupGet(k => k.Position)
				.Returns(pawnPosition);

			pieceMoverMock
				.Setup(p => p.Move(chessMove, It.IsAny<IEnumerable<IChessPiece>>()))
				.Returns<IChessPiece>(null);

			var pieces = new HashSet<IChessPiece>();

			pieces.Add(whiteKingMock.Object);
			pieces.Add(blackKingMock.Object);
			pieces.Add(pawnMock.Object);

			piecesFactoryMock
				.Setup(f => f.Create())
				.Returns(pieces);

			var chessBoard = new ChessBoard(piecesFactoryMock.Object, pieceMoverMock.Object);
			chessBoard.Move(chessMove);

			Assert.AreEqual(3, chessBoard.Pieces.Count());
			Assert.Contains(whiteKingMock.Object, chessBoard.Pieces.ToList());
			Assert.Contains(blackKingMock.Object, chessBoard.Pieces.ToList());
			Assert.Contains(pawnMock.Object, chessBoard.Pieces.ToList());
		}
		[Test]
		public void WhenThereIsCapture_MovingPieceWillRemovePieceFromAvailablePieces()
		{
			var whiteKingMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var blackKingMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var pawnMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var pieceMoverMock = new Mock<IPieceMover>(MockBehavior.Strict);
			var piecesFactoryMock = new Mock<IPiecesFactory>(MockBehavior.Strict);

			var whiteKingPosition = new Position(4, 0);
			var blackKingPosition = new Position(4, 7);
			var pawnPosition = new Position(4, 1);

			var chessMove = new ChessMove(whiteKingPosition, new Position(4, 1), true);

			whiteKingMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.White);
			whiteKingMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.King);
			whiteKingMock
				.SetupGet(k => k.Position)
				.Returns(whiteKingPosition);

			blackKingMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.Black);
			blackKingMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.King);
			blackKingMock
				.SetupGet(k => k.Position)
				.Returns(blackKingPosition);

			pawnMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.Black);
			pawnMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.Pawn);
			pawnMock
				.SetupGet(k => k.Position)
				.Returns(pawnPosition);

			pieceMoverMock
				.Setup(p => p.Move(chessMove, It.IsAny<IEnumerable<IChessPiece>>()))
				.Returns(pawnMock.Object);

			var pieces = new HashSet<IChessPiece>();

			pieces.Add(whiteKingMock.Object);
			pieces.Add(blackKingMock.Object);
			pieces.Add(pawnMock.Object);

			piecesFactoryMock
				.Setup(f => f.Create())
				.Returns(pieces);

			var chessBoard = new ChessBoard(piecesFactoryMock.Object, pieceMoverMock.Object);
			chessBoard.Move(chessMove);

			Assert.AreEqual(2, chessBoard.Pieces.Count());
			Assert.Contains(whiteKingMock.Object, chessBoard.Pieces.ToList());
			Assert.Contains(blackKingMock.Object, chessBoard.Pieces.ToList());
		}
		[Test]
		public void WhenThereIsNoCapture_MovingPieceAndReversingTheMoveWontRemoveAnyPieceFromAvailablePiecesAndWontAddAnyDuplicates()
		{
			var whiteKingMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var blackKingMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var pawnMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var pieceMoverMock = new Mock<IPieceMover>(MockBehavior.Strict);
			var piecesFactoryMock = new Mock<IPiecesFactory>(MockBehavior.Strict);

			var whiteKingPosition = new Position(4, 0);
			var blackKingPosition = new Position(4, 7);
			var pawnPosition = new Position(0, 6);

			var chessMove = new ChessMove(whiteKingPosition, new Position(4, 1));

			whiteKingMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.White);
			whiteKingMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.King);
			whiteKingMock
				.SetupGet(k => k.Position)
				.Returns(whiteKingPosition);

			blackKingMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.Black);
			blackKingMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.King);
			blackKingMock
				.SetupGet(k => k.Position)
				.Returns(blackKingPosition);

			pawnMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.Black);
			pawnMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.Pawn);
			pawnMock
				.SetupGet(k => k.Position)
				.Returns(pawnPosition);

			pieceMoverMock
				.Setup(p => p.Move(chessMove, It.IsAny<IEnumerable<IChessPiece>>()))
				.Returns<IChessPiece>(null);
			pieceMoverMock
			.Setup(p => p.ReverseLastMove(It.IsAny<IEnumerable<IChessPiece>>()))
			.Returns(chessMove.IsCapture);

			var pieces = new HashSet<IChessPiece>();

			pieces.Add(whiteKingMock.Object);
			pieces.Add(blackKingMock.Object);
			pieces.Add(pawnMock.Object);

			piecesFactoryMock
				.Setup(f => f.Create())
				.Returns(pieces);

			var chessBoard = new ChessBoard(piecesFactoryMock.Object, pieceMoverMock.Object);
			chessBoard.Move(chessMove);
			chessBoard.ReverseLastMove();

			Assert.AreEqual(3, chessBoard.Pieces.Count());
			Assert.Contains(whiteKingMock.Object, chessBoard.Pieces.ToList());
			Assert.Contains(blackKingMock.Object, chessBoard.Pieces.ToList());
			Assert.Contains(pawnMock.Object, chessBoard.Pieces.ToList());
		}
		[Test]
		public void WhenThereIsCapture_MovingPieceAndReversingTheMoveWillReaddRemovedPieceToAvailablePieces()
		{
			var whiteKingMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var blackKingMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var pawnMock = new Mock<IChessPiece>(MockBehavior.Strict);
			var pieceMoverMock = new Mock<IPieceMover>(MockBehavior.Strict);
			var piecesFactoryMock = new Mock<IPiecesFactory>(MockBehavior.Strict);

			var whiteKingPosition = new Position(4, 0);
			var blackKingPosition = new Position(4, 7);
			var pawnPosition = new Position(4, 1);

			var chessMove = new ChessMove(whiteKingPosition, new Position(4, 1), true);

			whiteKingMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.White);
			whiteKingMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.King);
			whiteKingMock
				.SetupGet(k => k.Position)
				.Returns(whiteKingPosition);

			blackKingMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.Black);
			blackKingMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.King);
			blackKingMock
				.SetupGet(k => k.Position)
				.Returns(blackKingPosition);

			pawnMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.Black);
			pawnMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.Pawn);
			pawnMock
				.SetupGet(k => k.Position)
				.Returns(pawnPosition);

			pieceMoverMock
				.Setup(p => p.Move(chessMove, It.IsAny<IEnumerable<IChessPiece>>()))
				.Returns(pawnMock.Object);
			pieceMoverMock
				.Setup(p => p.ReverseLastMove(It.IsAny<IEnumerable<IChessPiece>>()))
				.Returns(chessMove.IsCapture);

			var pieces = new HashSet<IChessPiece>();

			pieces.Add(whiteKingMock.Object);
			pieces.Add(blackKingMock.Object);
			pieces.Add(pawnMock.Object);

			piecesFactoryMock
				.Setup(f => f.Create())
				.Returns(pieces);

			var chessBoard = new ChessBoard(piecesFactoryMock.Object, pieceMoverMock.Object);
			chessBoard.Move(chessMove);
			chessBoard.ReverseLastMove();

			Assert.AreEqual(3, chessBoard.Pieces.Count());
			Assert.Contains(whiteKingMock.Object, chessBoard.Pieces.ToList());
			Assert.Contains(blackKingMock.Object, chessBoard.Pieces.ToList());
			Assert.Contains(pawnMock.Object, chessBoard.Pieces.ToList());
		}
	}
}