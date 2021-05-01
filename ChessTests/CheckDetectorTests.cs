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
	public class CheckDetectorTests
	{
		[Test]
		public void WhenWhiteKingIsCheckedReturnsWhiteKingChecked()
		{
			//WK - white king
			//BP - black pawn
			//7                        
			//6                        
			//5                        
			//4                        
			//3                        
			//2                        
			//1                BP      
			//0             WK         
			//  0  1  2  3  4  5  6  7 
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var movementMock = new Mock<IMovement>(MockBehavior.Strict);

			var kingColor = ChessColor.White;
			var kingMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var enemyMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var kingPosition = new Position(4, 0);
			var enemyPosition = new Position(5, 1);
			var enemyChessMove1 = new ChessMove(enemyPosition, kingPosition, true);
			var enemyChessMove2 = new ChessMove(enemyPosition, new Position(5, 0), false);

			kingMock
				.SetupGet(k => k.Position)
				.Returns(kingPosition);
			kingMock
				.SetupGet(k => k.Color)
				.Returns(kingColor);

			enemyMock
				.SetupGet(e => e.Position)
				.Returns(enemyPosition);
			enemyMock
				.SetupGet(e => e.Color)
				.Returns(kingColor.Opposite());

			boardMock
				.Setup(b => b.GetKing(kingColor))
				.Returns(kingMock.Object);
			boardMock
				.SetupGet(b => b.Pieces)
				.Returns(new List<IReadOnlyChessPiece>()
					{ kingMock.Object, enemyMock.Object });

			movementMock
				.Setup(m => m.GetAvailableMoves(enemyMock.Object))
				.Returns(new List<ChessMove>()
					{ enemyChessMove1, enemyChessMove2 });

			var checkDetector = new CheckDetector(boardMock.Object,
				movementMock.Object);

			var isChecked = checkDetector.IsChecked(kingColor);

			Assert.AreEqual(ChessPlayResult.WhiteChecked, isChecked);
		}
		[Test]
		public void WhenBlackKingIsCheckedReturnsBlackKingChecked()
		{
			//BK - black king
			//WP - white pawn
			//7             BK         
			//6          WP            
			//5                        
			//4                        
			//3                        
			//2                        
			//1                        
			//0                        
			//  0  1  2  3  4  5  6  7 
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var movementMock = new Mock<IMovement>(MockBehavior.Strict);

			var kingColor = ChessColor.Black;
			var kingMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var enemyMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var kingPosition = new Position(4, 7);
			var enemyPosition = new Position(3, 6);
			var enemyChessMove1 = new ChessMove(enemyPosition, kingPosition, true);
			var enemyChessMove2 = new ChessMove(enemyPosition, new Position(3, 7), false);

			kingMock
				.SetupGet(k => k.Position)
				.Returns(kingPosition);
			kingMock
				.SetupGet(k => k.Color)
				.Returns(kingColor);

			enemyMock
				.SetupGet(e => e.Position)
				.Returns(enemyPosition);
			enemyMock
				.SetupGet(e => e.Color)
				.Returns(kingColor.Opposite());

			boardMock
				.Setup(b => b.GetKing(kingColor))
				.Returns(kingMock.Object);
			boardMock
				.SetupGet(b => b.Pieces)
				.Returns(new List<IReadOnlyChessPiece>()
					{ kingMock.Object, enemyMock.Object });

			movementMock
				.Setup(m => m.GetAvailableMoves(enemyMock.Object))
				.Returns(new List<ChessMove>()
					{ enemyChessMove1, enemyChessMove2 });

			var checkDetector = new CheckDetector(boardMock.Object,
				movementMock.Object);

			var isChecked = checkDetector.IsChecked(kingColor);

			Assert.AreEqual(ChessPlayResult.BlackChecked, isChecked);
		}
		[Test]
		public void WhenKingIsNotCheckedReturnSuccessfulMove()
		{
			//BK - black king
			//WP - white pawn
			//7             BK         
			//6                        
			//5                        
			//4                        
			//3                        
			//2          WP            
			//1                        
			//0                        
			//  0  1  2  3  4  5  6  7 
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var movementMock = new Mock<IMovement>(MockBehavior.Strict);

			var kingColor = ChessColor.Black;
			var kingMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var enemyMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var kingPosition = new Position(4, 7);
			var enemyPosition = new Position(3, 2);
			var enemyChessMove = new ChessMove(enemyPosition, new Position(3, 3), false);

			kingMock
				.SetupGet(k => k.Position)
				.Returns(kingPosition);
			kingMock
				.SetupGet(k => k.Color)
				.Returns(kingColor);

			enemyMock
				.SetupGet(e => e.Position)
				.Returns(enemyPosition);
			enemyMock
				.SetupGet(e => e.Color)
				.Returns(kingColor.Opposite());

			boardMock
				.Setup(b => b.GetKing(kingColor))
				.Returns(kingMock.Object);
			boardMock
				.SetupGet(b => b.Pieces)
				.Returns(new List<IReadOnlyChessPiece>()
					{ kingMock.Object, enemyMock.Object });

			movementMock
				.Setup(m => m.GetAvailableMoves(enemyMock.Object))
				.Returns(new List<ChessMove>()
					{ enemyChessMove });

			var checkDetector = new CheckDetector(boardMock.Object,
				movementMock.Object);

			var isChecked = checkDetector.IsChecked(kingColor);

			Assert.AreEqual(ChessPlayResult.SuccessfulMove, isChecked);
		}
	}
}