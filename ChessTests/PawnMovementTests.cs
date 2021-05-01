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
	public class PawnMovementTests
	{
		[Test]
		public void WhitePawnOnStartingPositionShouldHaveTwoPossibleMoves()
		{
			//WP - white pawn
			//PM - possible move
			//7                        
			//6                        
			//5                        
			//4                        
			//3          PM            
			//2          PM            
			//1          WP            
			//0                        
			//  0  1  2  3  4  5  6  7 
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var targetPiece = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var piecePosition = new Position(3, 1);

			var availableFinishPosition1 = new Position(3, 2);
			var availableFinishPosition2 = new Position(3, 3);

			targetPiece
				.SetupGet(p => p.Color)
				.Returns(ChessColor.White);
			targetPiece
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Pawn);
			targetPiece
				.SetupGet(p => p.Position)
				.Returns(piecePosition);
			targetPiece
				.SetupGet(p => p.HasMoved)
				.Returns(false);

			boardMock
				.Setup(b => b.IsPositionTaken(availableFinishPosition1))
				.Returns(false);
			boardMock
				.Setup(b => b.IsPositionTaken(availableFinishPosition2))
				.Returns(false);
			boardMock
				.Setup(b => b.IsEnemyOnPosition(It.IsAny<Position>(), It.IsAny<ChessColor>()))
				.Returns(false);

			var movement = new PawnMovement(boardMock.Object);
			var movements = movement.GetAvailableMoves(targetPiece.Object);

			Assert.AreEqual(2, movements.Count());
			Assert.IsTrue(movements
				.Any(m =>
					m.StartingPosition == piecePosition &&
					m.FinishedPosition == availableFinishPosition1
				));
			Assert.IsTrue(movements
				.Any(m =>
					m.StartingPosition == piecePosition &&
					m.FinishedPosition == availableFinishPosition2
				));
		}
		[Test]
		public void WhenThereIsPieceBlockingSecondPositionThereShouldBeOnlyOnePossibleMove()
		{
			//BP - black pawn
			//PM - possible move
			//SC - Some chess piece
			//7                        
			//6             BP         
			//5             PM         
			//4             SC         
			//3                        
			//2                        
			//1                        
			//0                        
			//  0  1  2  3  4  5  6  7 
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var targetPiece = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var piecePosition = new Position(4, 6);

			var availableFinishPosition = new Position(4, 5);
			var unavailableFinishPosition = new Position(4, 4);

			targetPiece
				.SetupGet(p => p.Color)
				.Returns(ChessColor.Black);
			targetPiece
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Pawn);
			targetPiece
				.SetupGet(p => p.Position)
				.Returns(piecePosition);
			targetPiece
				.SetupGet(p => p.HasMoved)
				.Returns(false);

			boardMock
				.Setup(b => b.IsPositionTaken(availableFinishPosition))
				.Returns(false);
			boardMock
				.Setup(b => b.IsPositionTaken(unavailableFinishPosition))
				.Returns(true);
			boardMock
				.Setup(b => b.IsEnemyOnPosition(It.IsAny<Position>(), It.IsAny<ChessColor>()))
				.Returns(false);

			var movement = new PawnMovement(boardMock.Object);
			var movements = movement.GetAvailableMoves(targetPiece.Object);

			Assert.AreEqual(1, movements.Count());
			Assert.IsTrue(movements
				.Any(m =>
					m.StartingPosition == piecePosition &&
					m.FinishedPosition == availableFinishPosition
				));
		}
		[Test]
		public void WhenThereIsEnemyStandingInDiagonalFrontOfPawnThereShouldBeCaptureMove()
		{
			//BP - black pawn
			//PM - possible move
			//EC - enemy chess piece
			//7                        
			//6                        
			//5                        
			//4                      BP
			//3                   EC PM
			//2                        
			//1                        
			//0                        
			//  0  1  2  3  4  5  6  7 
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var targetPiece = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var piecePosition = new Position(7, 4);

			var availableFinishPosition = new Position(7, 3);
			var enemyPosition = new Position(6, 3);

			targetPiece
				.SetupGet(p => p.Color)
				.Returns(ChessColor.Black);
			targetPiece
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Pawn);
			targetPiece
				.SetupGet(p => p.Position)
				.Returns(piecePosition);
			targetPiece
				.SetupGet(p => p.HasMoved)
				.Returns(true);

			boardMock
				.Setup(b => b.IsPositionTaken(availableFinishPosition))
				.Returns(false);
			boardMock
				.Setup(b => b.IsPositionTaken(enemyPosition))
				.Returns(true);
			boardMock
				.Setup(b => b.IsEnemyOnPosition(enemyPosition, ChessColor.White))
				.Returns(true);
			boardMock
				.Setup(b => b.IsEnemyOnPosition(
					It.IsNotIn<Position>(new List<Position>() { enemyPosition }), ChessColor.White))
				.Returns(false);

			var movement = new PawnMovement(boardMock.Object);
			var movements = movement.GetAvailableMoves(targetPiece.Object);

			Assert.AreEqual(2, movements.Count());
			Assert.IsTrue(movements
				.Any(m =>
					m.StartingPosition == piecePosition &&
					m.FinishedPosition == availableFinishPosition
				));
			Assert.IsTrue(movements
				.Any(m =>
					m.StartingPosition == piecePosition &&
					m.FinishedPosition == enemyPosition
				));
		}
	}
}