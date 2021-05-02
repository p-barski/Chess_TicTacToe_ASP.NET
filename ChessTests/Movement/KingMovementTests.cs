using System.Linq;
using Moq;
using NUnit.Framework;
using Chess;
using Chess.Pieces;
using Chess.Board;
using Chess.Movement;

namespace ChessTests
{
	public class KingMovementTests
	{
		[Test]
		public void KingCanMoveOneTileInEveryDirection()
		{
			//WK - white king
			//PM - possible move
			//7                        
			//6                        
			//5          PM PM PM      
			//4          PM WK PM      
			//3          PM PM PM      
			//2                        
			//1                        
			//0                        
			//  0  1  2  3  4  5  6  7 
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var kingMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var kingPosition = new Position(4, 4);

			var availableFinishPosition1 = new Position(3, 3);
			var availableFinishPosition2 = new Position(3, 4);
			var availableFinishPosition3 = new Position(3, 5);
			var availableFinishPosition4 = new Position(4, 5);
			var availableFinishPosition5 = new Position(5, 5);
			var availableFinishPosition6 = new Position(5, 4);
			var availableFinishPosition7 = new Position(5, 3);
			var availableFinishPosition8 = new Position(4, 3);

			kingMock
				.SetupGet(p => p.Color)
				.Returns(ChessColor.White);
			kingMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.King);
			kingMock
				.SetupGet(p => p.Position)
				.Returns(kingPosition);
			kingMock
				.SetupGet(p => p.HasMoved)
				.Returns(true);

			boardMock
				.Setup(b => b.IsPositionTaken(It.IsAny<Position>()))
				.Returns(false);
			boardMock
				.Setup(b => b.IsEnemyOnPosition(It.IsAny<Position>(), It.IsAny<ChessColor>()))
				.Returns(false);

			var movement = new KingMovement(boardMock.Object);
			var availableMoves = movement.GetAvailableMoves(kingMock.Object);

			Assert.AreEqual(8, availableMoves.Count());
			var finishedPositions = availableMoves
				.Select(m => m.FinishedPosition)
				.ToList();

			Assert.Contains(availableFinishPosition1, finishedPositions);
			Assert.Contains(availableFinishPosition2, finishedPositions);
			Assert.Contains(availableFinishPosition3, finishedPositions);
			Assert.Contains(availableFinishPosition4, finishedPositions);
			Assert.Contains(availableFinishPosition5, finishedPositions);
			Assert.Contains(availableFinishPosition6, finishedPositions);
			Assert.Contains(availableFinishPosition7, finishedPositions);
			Assert.Contains(availableFinishPosition8, finishedPositions);
		}
		[Test]
		public void KingCanCaptureEnemyPieceAndCantCaptureItsOwnPiece()
		{
			//BK - black king
			//PM - possible move
			//BP - black pawn
			//EP - enemy piece
			//7                   PM BK
			//6                   EP BP
			//5                        
			//4                        
			//3                        
			//2                        
			//1                        
			//0                        
			//  0  1  2  3  4  5  6  7 
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var kingMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);

			var kingPosition = new Position(7, 7);

			var availableFinishPosition = new Position(6, 7);
			var enemyPosition = new Position(6, 6);
			var blackPawnPosition = new Position(7, 6);

			kingMock
				.SetupGet(p => p.Color)
				.Returns(ChessColor.Black);
			kingMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.King);
			kingMock
				.SetupGet(p => p.Position)
				.Returns(kingPosition);
			kingMock
				.SetupGet(p => p.HasMoved)
				.Returns(true);

			boardMock
				.Setup(b => b.IsPositionTaken(blackPawnPosition))
				.Returns(true);
			boardMock
				.Setup(b => b.IsPositionTaken(availableFinishPosition))
				.Returns(false);
			boardMock
				.Setup(b => b.IsPositionTaken(enemyPosition))
				.Returns(true);

			boardMock
				.Setup(b => b.IsEnemyOnPosition(blackPawnPosition, ChessColor.White))
				.Returns(false);
			boardMock
				.Setup(b => b.IsEnemyOnPosition(availableFinishPosition, ChessColor.White))
				.Returns(false);
			boardMock
				.Setup(b => b.IsEnemyOnPosition(enemyPosition, ChessColor.White))
				.Returns(true);


			var movement = new KingMovement(boardMock.Object);
			var availableMoves = movement.GetAvailableMoves(kingMock.Object);

			Assert.AreEqual(2, availableMoves.Count());
			var finishedPositions = availableMoves
				.Select(m => m.FinishedPosition)
				.ToList();

			Assert.Contains(availableFinishPosition, finishedPositions);
			Assert.Contains(enemyPosition, finishedPositions);
		}
	}
}