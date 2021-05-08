using System.Linq;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Chess;
using Chess.Pieces;
using Chess.Board;
using Chess.Movement;

namespace ChessTests
{
	public class EnPassantMovementTests
	{
		[Test]
		public void WhenLastMoveWasNotDoublePawnMove_EnPassantIsNotPossible()
		{
			//WP - white pawn
			//WR - white rook
			//BP - black pawn
			//7                        
			//6                        
			//5    WR                  
			//4                        
			//3          WP BP         
			//2                        
			//1                        
			//0                        
			//  0  1  2  3  4  5  6  7 
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var historyMock = new Mock<IReadOnlyMovementHistory>(MockBehavior.Strict);
			var whitePawnMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var blackPawnMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var whiteRookMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);

			var whitePawnPosition = new Position(3, 3);
			var blackPawnPositon = new Position(4, 3);
			var whiteRookPosition = new Position(1, 5);

			whitePawnMock
				.SetupGet(p => p.Color)
				.Returns(ChessColor.White);
			whitePawnMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Pawn);
			whitePawnMock
				.SetupGet(p => p.Position)
				.Returns(whitePawnPosition);
			whitePawnMock
				.SetupGet(p => p.HasMoved)
				.Returns(true);

			blackPawnMock
				.SetupGet(p => p.Color)
				.Returns(ChessColor.Black);
			blackPawnMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Pawn);
			blackPawnMock
				.SetupGet(p => p.Position)
				.Returns(blackPawnPositon);
			blackPawnMock
				.SetupGet(p => p.HasMoved)
				.Returns(true);

			whiteRookMock
				.SetupGet(p => p.Position)
				.Returns(whiteRookPosition);
			whiteRookMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Rook);

			var pieces = new List<IReadOnlyChessPiece>(){
				whitePawnMock.Object, blackPawnMock.Object,
				whiteRookMock.Object
			};

			var moves = new List<ChessMove>(){
				new ChessMove(new Position(1, 1), whiteRookPosition)
			};

			historyMock
				.SetupGet(h => h.ChessMoves)
				.Returns(moves);

			boardMock
				.SetupGet(b => b.Pieces)
				.Returns(pieces);
			boardMock
				.SetupGet(b => b.History)
				.Returns(historyMock.Object);

			var movement = new EnPassantMovement(boardMock.Object);
			var availableMoves = movement.GetAvailableMoves(whitePawnMock.Object);

			Assert.AreEqual(0, availableMoves.Count());
		}
		[Test]
		public void WhiteEnPassantTest()
		{
			//WP - white pawn
			//BP - black pawn
			//PM - possible move
			//7                        
			//6                        
			//5             PM         
			//4          WP BP         
			//3                        
			//2                        
			//1                        
			//0                        
			//  0  1  2  3  4  5  6  7 
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var historyMock = new Mock<IReadOnlyMovementHistory>(MockBehavior.Strict);
			var whitePawnMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var blackPawnMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);

			var whitePawnPosition = new Position(3, 4);
			var blackPawnPositon = new Position(4, 4);
			var blackPawnPreviousPosition = new Position(4, 6);
			var possibleCapturePosition = new Position(4, 5);

			whitePawnMock
				.SetupGet(p => p.Color)
				.Returns(ChessColor.White);
			whitePawnMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Pawn);
			whitePawnMock
				.SetupGet(p => p.Position)
				.Returns(whitePawnPosition);
			whitePawnMock
				.SetupGet(p => p.HasMoved)
				.Returns(true);

			blackPawnMock
				.SetupGet(p => p.Color)
				.Returns(ChessColor.Black);
			blackPawnMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Pawn);
			blackPawnMock
				.SetupGet(p => p.Position)
				.Returns(blackPawnPositon);
			blackPawnMock
				.SetupGet(p => p.HasMoved)
				.Returns(true);

			var pieces = new List<IReadOnlyChessPiece>(){
				whitePawnMock.Object, blackPawnMock.Object
			};

			var moves = new List<ChessMove>(){
				new ChessMove(blackPawnPreviousPosition, blackPawnPositon)
			};

			historyMock
				.SetupGet(h => h.ChessMoves)
				.Returns(moves);

			boardMock
				.SetupGet(b => b.Pieces)
				.Returns(pieces);
			boardMock
				.SetupGet(b => b.History)
				.Returns(historyMock.Object);

			var movement = new EnPassantMovement(boardMock.Object);
			var availableMoves = movement.GetAvailableMoves(whitePawnMock.Object);

			Assert.AreEqual(1, availableMoves.Count());
			var move = availableMoves.First();
			Assert.AreEqual(new ChessMove(whitePawnPosition, possibleCapturePosition), move);
		}
		[Test]
		public void BlackEnPassantTest()
		{
			//WP - white pawn
			//BP - black pawn
			//PM - possible move
			//7                        
			//6                        
			//5                        
			//4                        
			//3                   WP BP
			//2                   PM   
			//1                        
			//0                        
			//  0  1  2  3  4  5  6  7 
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var historyMock = new Mock<IReadOnlyMovementHistory>(MockBehavior.Strict);
			var whitePawnMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var blackPawnMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);

			var whitePawnPosition = new Position(6, 3);
			var blackPawnPositon = new Position(7, 3);
			var whitePawnPreviousPosition = new Position(6, 1);
			var possibleCapturePosition = new Position(6, 2);

			whitePawnMock
				.SetupGet(p => p.Color)
				.Returns(ChessColor.White);
			whitePawnMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Pawn);
			whitePawnMock
				.SetupGet(p => p.Position)
				.Returns(whitePawnPosition);
			whitePawnMock
				.SetupGet(p => p.HasMoved)
				.Returns(true);

			blackPawnMock
				.SetupGet(p => p.Color)
				.Returns(ChessColor.Black);
			blackPawnMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Pawn);
			blackPawnMock
				.SetupGet(p => p.Position)
				.Returns(blackPawnPositon);
			blackPawnMock
				.SetupGet(p => p.HasMoved)
				.Returns(true);

			var pieces = new List<IReadOnlyChessPiece>(){
				whitePawnMock.Object, blackPawnMock.Object
			};

			var moves = new List<ChessMove>(){
				new ChessMove(whitePawnPreviousPosition, whitePawnPosition)
			};

			historyMock
				.SetupGet(h => h.ChessMoves)
				.Returns(moves);

			boardMock
				.SetupGet(b => b.Pieces)
				.Returns(pieces);
			boardMock
				.SetupGet(b => b.History)
				.Returns(historyMock.Object);

			var movement = new EnPassantMovement(boardMock.Object);
			var availableMoves = movement.GetAvailableMoves(blackPawnMock.Object);

			Assert.AreEqual(1, availableMoves.Count());
			var move = availableMoves.First();
			Assert.AreEqual(new ChessMove(blackPawnPositon, possibleCapturePosition), move);
		}
	}
}