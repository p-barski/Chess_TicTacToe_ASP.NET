using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Chess;
using Chess.Game;
using Chess.Board;
using Chess.Pieces;
using Chess.Movement;

namespace ChessTests
{
	public class StalemateDetectorTests
	{
		[Test]
		public void WhenPlayerCanMoveReturnsSuccessfulMove()
		{
			//WK - white king
			//BK - black king
			//BP - black pawn
			//7             BK         
			//6                        
			//5                        
			//4                        
			//3                        
			//2                        
			//1                BP      
			//0             WK         
			//  0  1  2  3  4  5  6  7 
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var movementMock = new Mock<ILegalMovement>(MockBehavior.Strict);

			var whiteKingMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var blackKingMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var blackPawnMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);

			var whiteKingPosition = new Position(4, 0);
			var blackPawnPosition = new Position(5, 1);

			var possibleMove1 = new ChessMove(whiteKingPosition, new Position(3, 0), true);
			var possibleMove2 = new ChessMove(whiteKingPosition, new Position(3, 1), true);
			var possibleMove3 = new ChessMove(whiteKingPosition, new Position(4, 1), true);
			var possibleMove4 = new ChessMove(whiteKingPosition, blackPawnPosition, true);
			var possibleMove5 = new ChessMove(whiteKingPosition, new Position(5, 0), true);

			var pieces = new List<IReadOnlyChessPiece>(){
				whiteKingMock.Object, blackKingMock.Object,
				blackPawnMock.Object,
			};

			var possibleMoves = new List<ChessMove>(){
				possibleMove1, possibleMove2, possibleMove3,
				possibleMove4, possibleMove5
			};

			whiteKingMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.White);

			blackKingMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.Black);

			blackPawnMock
				.SetupGet(e => e.Color)
				.Returns(ChessColor.Black);

			boardMock
				.SetupGet(b => b.Pieces)
				.Returns(pieces);

			movementMock
				.Setup(m => m.GetAvailableLegalMoves(whiteKingMock.Object))
				.Returns(possibleMoves);

			var stalemateDetector = new StalemateDetector(boardMock.Object,
				movementMock.Object);

			var isStalemate = stalemateDetector.IsStalemate(ChessColor.White);

			Assert.AreEqual(ChessPlayResult.SuccessfulMove, isStalemate);
		}
		[Test]
		public void WhenPlayerCantMoveReturnsStalemate()
		{
			//WK - white king
			//BK - black king
			//BP - black pawn
			//7                      BK
			//6             WR         
			//5                        
			//4                        
			//3                        
			//2                        
			//1                        
			//0             WK    WR   
			//  0  1  2  3  4  5  6  7 
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var movementMock = new Mock<ILegalMovement>(MockBehavior.Strict);

			var whiteKingMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var blackKingMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var whiteRook1Mock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var whiteRook2Mock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);

			var pieces = new List<IReadOnlyChessPiece>(){
				whiteKingMock.Object, blackKingMock.Object,
				whiteRook1Mock.Object, whiteRook2Mock.Object
			};

			whiteKingMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.White);

			blackKingMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.Black);

			whiteRook1Mock
				.SetupGet(e => e.Color)
				.Returns(ChessColor.White);

			whiteRook2Mock
				.SetupGet(e => e.Color)
				.Returns(ChessColor.White);

			boardMock
				.SetupGet(b => b.Pieces)
				.Returns(pieces);

			movementMock
				.Setup(m => m.GetAvailableLegalMoves(blackKingMock.Object))
				.Returns(new List<ChessMove>());

			var stalemateDetector = new StalemateDetector(boardMock.Object,
				movementMock.Object);

			var isStalemate = stalemateDetector.IsStalemate(ChessColor.Black);

			Assert.AreEqual(ChessPlayResult.Stalemate, isStalemate);
		}
	}
}