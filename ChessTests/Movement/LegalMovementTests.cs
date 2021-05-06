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
	public class LegalMovementTests
	{
		[Test]
		public void CorrectlyFiltersIllegalMoves()
		{
			//WK - white king
			//BR - black rook
			//PM - possible move
			//7                        
			//6                        
			//5                        
			//4                        
			//3                        
			//2                        
			//1                      BR
			//0          PM WK PM      
			//  0  1  2  3  4  5  6  7 
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var checkDetectorMock = new Mock<ICheckDetector>(MockBehavior.Strict);
			var movementMock = new Mock<IMovement>(MockBehavior.Strict);
			var kingMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);

			var kingPosition = new Position(4, 0);

			var legalKingMoves = new List<ChessMove>(){
				new ChessMove(kingPosition, new Position(3, 0)),
				new ChessMove(kingPosition, new Position(5, 0))
			};

			var illegalKingMoves = new List<ChessMove>(){
				new ChessMove(kingPosition, new Position(3, 1)),
				new ChessMove(kingPosition, new Position(4, 1)),
				new ChessMove(kingPosition, new Position(5, 1))
			};

			bool isChecked = false;

			kingMock
				.SetupGet(p => p.Color)
				.Returns(ChessColor.White);

			boardMock
				.Setup(b => b.Move(It.IsIn<ChessMove>(legalKingMoves)));
			boardMock
				.Setup(b => b.Move(It.IsIn<ChessMove>(illegalKingMoves)))
				.Callback(() => isChecked = true);
			boardMock
				.Setup(b => b.ReverseLastMove())
				.Callback(() => isChecked = false);

			checkDetectorMock
				.Setup(c => c.IsChecked(ChessColor.White))
				.Returns(() => isChecked);

			movementMock
				.Setup(m => m.GetAvailableMoves(kingMock.Object))
				.Returns(legalKingMoves.Union(illegalKingMoves));

			var movement = new LegalMovement(boardMock.Object, movementMock.Object,
				checkDetectorMock.Object);
			var legalMoves = movement.GetAvailableLegalMoves(kingMock.Object);

			CollectionAssert.AreEquivalent(legalKingMoves, legalMoves);
		}
		[Test]
		public void WhenThereAreLegalMovesAvailableReturnsTrue()
		{
			//WK - white king
			//BR - black rook
			//PM - possible move
			//7                        
			//6                        
			//5                        
			//4                        
			//3                        
			//2                        
			//1                      BR
			//0          PM WK PM      
			//  0  1  2  3  4  5  6  7 
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var checkDetectorMock = new Mock<ICheckDetector>(MockBehavior.Strict);
			var movementMock = new Mock<IMovement>(MockBehavior.Strict);
			var kingMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);

			var kingPosition = new Position(4, 0);

			var legalKingMoves = new List<ChessMove>(){
				new ChessMove(kingPosition, new Position(3, 0)),
				new ChessMove(kingPosition, new Position(5, 0))
			};

			var illegalKingMoves = new List<ChessMove>(){
				new ChessMove(kingPosition, new Position(3, 1)),
				new ChessMove(kingPosition, new Position(4, 1)),
				new ChessMove(kingPosition, new Position(5, 1))
			};

			bool isChecked = false;

			kingMock
				.SetupGet(p => p.Color)
				.Returns(ChessColor.White);

			boardMock
				.Setup(b => b.Move(It.IsIn<ChessMove>(legalKingMoves)));
			boardMock
				.Setup(b => b.Move(It.IsIn<ChessMove>(illegalKingMoves)))
				.Callback(() => isChecked = true);
			boardMock
				.Setup(b => b.ReverseLastMove())
				.Callback(() => isChecked = false);
			boardMock
				.SetupGet(b => b.Pieces)
				.Returns(new List<IReadOnlyChessPiece>() { kingMock.Object });

			checkDetectorMock
				.Setup(c => c.IsChecked(ChessColor.White))
				.Returns(() => isChecked);

			movementMock
				.Setup(m => m.GetAvailableMoves(kingMock.Object))
				.Returns(legalKingMoves.Union(illegalKingMoves));

			var movement = new LegalMovement(boardMock.Object, movementMock.Object,
				checkDetectorMock.Object);
			var canMove = movement.HasAnyLegalMoves(kingMock.Object.Color);

			Assert.IsTrue(canMove);
		}
		[Test]
		public void WhenThereIsNoLegalMoveAvailableReturnsFalse()
		{
			//WK - white king
			//BR - black rook
			//PM - possible move
			//7                        
			//6                        
			//5                        
			//4                        
			//3                        
			//2                        
			//1                      BR
			//0             WK       BR
			//  0  1  2  3  4  5  6  7 
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var checkDetectorMock = new Mock<ICheckDetector>(MockBehavior.Strict);
			var movementMock = new Mock<IMovement>(MockBehavior.Strict);
			var kingMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);

			var kingPosition = new Position(4, 0);

			var illegalKingMoves = new List<ChessMove>(){
				new ChessMove(kingPosition, new Position(3, 0)),
				new ChessMove(kingPosition, new Position(3, 1)),
				new ChessMove(kingPosition, new Position(4, 1)),
				new ChessMove(kingPosition, new Position(5, 1)),
				new ChessMove(kingPosition, new Position(5, 0))
			};

			bool isChecked = false;

			kingMock
				.SetupGet(p => p.Color)
				.Returns(ChessColor.White);

			boardMock
				.Setup(b => b.Move(It.IsIn<ChessMove>(illegalKingMoves)))
				.Callback(() => isChecked = true);
			boardMock
				.Setup(b => b.ReverseLastMove())
				.Callback(() => isChecked = false);
			boardMock
				.SetupGet(b => b.Pieces)
				.Returns(new List<IReadOnlyChessPiece>() { kingMock.Object });

			checkDetectorMock
				.Setup(c => c.IsChecked(ChessColor.White))
				.Returns(() => isChecked);

			movementMock
				.Setup(m => m.GetAvailableMoves(kingMock.Object))
				.Returns(illegalKingMoves);

			var movement = new LegalMovement(boardMock.Object, movementMock.Object,
				checkDetectorMock.Object);
			var canMove = movement.HasAnyLegalMoves(kingMock.Object.Color);

			Assert.IsFalse(canMove);
		}
	}
}