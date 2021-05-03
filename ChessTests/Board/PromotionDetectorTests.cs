using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Chess;
using Chess.Board;
using Chess.Pieces;

namespace ChessTests
{
	public class PromotionDetectorTests
	{
		[Test]
		public void WhenThereIsNoPawnEligibleForPromotionReturnsFalse()
		{
			//WK - white king
			//WP - white pawn
			//BK - black king
			//BP - black pawn
			//7             BK         
			//6             BP         
			//5                        
			//4                        
			//3                        
			//2                        
			//1             WP         
			//0             WK         
			//  0  1  2  3  4  5  6  7 
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var whiteKingMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var whitePawnMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var blackKingMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var blackPawnMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);

			var whiteKingPosition = new Position(4, 0);
			var whitePawnPosition = new Position(4, 1);
			var blackKingPosition = new Position(4, 7);
			var blackPawnPosition = new Position(4, 6);

			var pieces = new List<IReadOnlyChessPiece>(){
				whiteKingMock.Object, whitePawnMock.Object,
				blackKingMock.Object, blackPawnMock.Object
			};

			whiteKingMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.White);
			whiteKingMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.King);
			whiteKingMock
				.SetupGet(k => k.Position)
				.Returns(whiteKingPosition);

			whitePawnMock
				.SetupGet(e => e.Color)
				.Returns(ChessColor.White);
			whitePawnMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.Pawn);
			whitePawnMock
				.SetupGet(k => k.Position)
				.Returns(whitePawnPosition);

			blackKingMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.Black);
			blackKingMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.King);
			blackKingMock
				.SetupGet(k => k.Position)
				.Returns(blackKingPosition);

			blackPawnMock
				.SetupGet(e => e.Color)
				.Returns(ChessColor.Black);
			blackPawnMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.Pawn);
			blackPawnMock
				.SetupGet(k => k.Position)
				.Returns(blackPawnPosition);

			boardMock
				.SetupGet(b => b.Pieces)
				.Returns(pieces);

			var promotionDetector = new PromotionDetector(boardMock.Object);
			var isPromotionRequired = promotionDetector.IsPromotionRequired();

			Assert.AreEqual(false, isPromotionRequired);
		}
		[Test]
		public void WhenThereIsBlackPawnEligibleForPromotionReturnsTrue()
		{
			//WK - white king
			//WP - white pawn
			//BK - black king
			//BP - black pawn
			//7             BK         
			//6                        
			//5                        
			//4                        
			//3                        
			//2                        
			//1             WP         
			//0             WK       BP
			//  0  1  2  3  4  5  6  7 
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var whiteKingMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var whitePawnMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var blackKingMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var blackPawnMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);

			var whiteKingPosition = new Position(4, 0);
			var whitePawnPosition = new Position(4, 1);
			var blackKingPosition = new Position(4, 7);
			var blackPawnPosition = new Position(7, 0);

			var pieces = new List<IReadOnlyChessPiece>(){
				whiteKingMock.Object, whitePawnMock.Object,
				blackKingMock.Object, blackPawnMock.Object
			};

			whiteKingMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.White);
			whiteKingMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.King);
			whiteKingMock
				.SetupGet(k => k.Position)
				.Returns(whiteKingPosition);

			whitePawnMock
				.SetupGet(e => e.Color)
				.Returns(ChessColor.White);
			whitePawnMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.Pawn);
			whitePawnMock
				.SetupGet(k => k.Position)
				.Returns(whitePawnPosition);

			blackKingMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.Black);
			blackKingMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.King);
			blackKingMock
				.SetupGet(k => k.Position)
				.Returns(blackKingPosition);

			blackPawnMock
				.SetupGet(e => e.Color)
				.Returns(ChessColor.Black);
			blackPawnMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.Pawn);
			blackPawnMock
				.SetupGet(k => k.Position)
				.Returns(blackPawnPosition);

			boardMock
				.SetupGet(b => b.Pieces)
				.Returns(pieces);

			var promotionDetector = new PromotionDetector(boardMock.Object);
			var isPromotionRequired = promotionDetector.IsPromotionRequired();

			Assert.AreEqual(true, isPromotionRequired);
		}
		[Test]
		public void WhenThereIsWhitePawnEligibleForPromotionReturnsTrue()
		{
			//WK - white king
			//WP - white pawn
			//BK - black king
			//BP - black pawn
			//7          WP BK         
			//6             BP         
			//5                        
			//4                        
			//3                        
			//2                        
			//1                      
			//0             WK         
			//  0  1  2  3  4  5  6  7 
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var whiteKingMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var whitePawnMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var blackKingMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var blackPawnMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);

			var whiteKingPosition = new Position(4, 0);
			var whitePawnPosition = new Position(3, 7);
			var blackKingPosition = new Position(4, 7);
			var blackPawnPosition = new Position(4, 6);

			var pieces = new List<IReadOnlyChessPiece>(){
				whiteKingMock.Object, whitePawnMock.Object,
				blackKingMock.Object, blackPawnMock.Object
			};

			whiteKingMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.White);
			whiteKingMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.King);
			whiteKingMock
				.SetupGet(k => k.Position)
				.Returns(whiteKingPosition);

			whitePawnMock
				.SetupGet(e => e.Color)
				.Returns(ChessColor.White);
			whitePawnMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.Pawn);
			whitePawnMock
				.SetupGet(k => k.Position)
				.Returns(whitePawnPosition);

			blackKingMock
				.SetupGet(k => k.Color)
				.Returns(ChessColor.Black);
			blackKingMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.King);
			blackKingMock
				.SetupGet(k => k.Position)
				.Returns(blackKingPosition);

			blackPawnMock
				.SetupGet(e => e.Color)
				.Returns(ChessColor.Black);
			blackPawnMock
				.SetupGet(k => k.PieceType)
				.Returns(ChessPieceType.Pawn);
			blackPawnMock
				.SetupGet(k => k.Position)
				.Returns(blackPawnPosition);

			boardMock
				.SetupGet(b => b.Pieces)
				.Returns(pieces);

			var promotionDetector = new PromotionDetector(boardMock.Object);
			var isPromotionRequired = promotionDetector.IsPromotionRequired();

			Assert.AreEqual(true, isPromotionRequired);
		}
	}
}