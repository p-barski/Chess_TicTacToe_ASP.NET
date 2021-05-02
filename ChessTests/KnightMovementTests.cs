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
	public class KnightMovementTests
	{
		[Test]
		public void KnightMoveTest()
		{
			//WK - white knight
			//PM - possible move
			//7                        
			//6                        
			//5          PM    PM      
			//4       PM          PM   
			//3             WK         
			//2       PM          PM   
			//1          PM    PM      
			//0                        
			//  0  1  2  3  4  5  6  7 
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var knightMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);

			var knightPosition = new Position(4, 3);

			var availableFinishPosition01 = new Position(5, 1);
			var availableFinishPosition02 = new Position(6, 2);
			var availableFinishPosition03 = new Position(6, 4);
			var availableFinishPosition04 = new Position(5, 5);
			var availableFinishPosition05 = new Position(3, 5);
			var availableFinishPosition06 = new Position(2, 4);
			var availableFinishPosition07 = new Position(2, 2);
			var availableFinishPosition08 = new Position(3, 1);

			knightMock
				.SetupGet(p => p.Color)
				.Returns(ChessColor.White);
			knightMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Knight);
			knightMock
				.SetupGet(p => p.Position)
				.Returns(knightPosition);
			knightMock
				.SetupGet(p => p.HasMoved)
				.Returns(true);

			boardMock
				.Setup(b => b.IsPositionTaken(It.IsAny<Position>()))
				.Returns(false);
			boardMock
				.Setup(b => b.IsEnemyOnPosition(It.IsAny<Position>(), It.IsAny<ChessColor>()))
				.Returns(false);

			var movement = new KnightMovement(boardMock.Object);
			var availableMoves = movement.GetAvailableMoves(knightMock.Object);

			Assert.AreEqual(8, availableMoves.Count());
			Assert.IsTrue(availableMoves.All(m => m.StartingPosition == knightPosition));

			var finishedPositions = availableMoves
				.Select(m => m.FinishedPosition)
				.ToList();

			Assert.Contains(availableFinishPosition01, finishedPositions);
			Assert.Contains(availableFinishPosition02, finishedPositions);
			Assert.Contains(availableFinishPosition03, finishedPositions);
			Assert.Contains(availableFinishPosition04, finishedPositions);
			Assert.Contains(availableFinishPosition05, finishedPositions);
			Assert.Contains(availableFinishPosition06, finishedPositions);
			Assert.Contains(availableFinishPosition07, finishedPositions);
			Assert.Contains(availableFinishPosition08, finishedPositions);
		}
		[Test]
		public void KnightCanJumpThroughAnyObstacle()
		{
			//BK - black knight
			//PM - possible move
			//EP - enemy piece
			//BP - black piece
			//7 BP BK BP               
			//6 EP BP EP PM             
			//5 PM    PM               
			//4                        
			//3                        
			//2                        
			//1                        
			//0                        
			//  0  1  2  3  4  5  6  7 
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var knightMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);

			var knightPosition = new Position(1, 7);
			var blackPiece1Position = new Position(0, 7);
			var blackPiece2Position = new Position(1, 6);
			var blackPiece3Position = new Position(2, 7);
			var enemyPiece1Position = new Position(0, 6);
			var enemyPiece2Position = new Position(2, 6);

			var availableFinishPosition01 = new Position(0, 5);
			var availableFinishPosition02 = new Position(2, 5);
			var availableFinishPosition03 = new Position(3, 6);

			var friendlyPiecesPositions = new List<Position>(){
				blackPiece1Position, blackPiece2Position,
				blackPiece3Position
			};

			var enemyPositions = new List<Position>(){
				enemyPiece1Position, enemyPiece2Position
			};

			var emptyPositions = new List<Position>(){
				availableFinishPosition01,
				availableFinishPosition02,
				availableFinishPosition03
			};

			knightMock
				.SetupGet(p => p.Color)
				.Returns(ChessColor.Black);
			knightMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Knight);
			knightMock
				.SetupGet(p => p.Position)
				.Returns(knightPosition);
			knightMock
				.SetupGet(p => p.HasMoved)
				.Returns(false);

			boardMock
				.Setup(b => b.IsPositionTaken(It.IsIn<Position>(friendlyPiecesPositions)))
				.Returns(true);
			boardMock
				.Setup(b => b.IsPositionTaken(It.IsIn<Position>(enemyPositions)))
				.Returns(true);
			boardMock
				.Setup(b => b.IsPositionTaken(It.IsIn<Position>(emptyPositions)))
				.Returns(false);
			boardMock
				.Setup(b => b.IsEnemyOnPosition(It.IsIn<Position>(friendlyPiecesPositions), ChessColor.White))
				.Returns(false);
			boardMock
				.Setup(b => b.IsEnemyOnPosition(It.IsIn<Position>(enemyPositions), ChessColor.White))
				.Returns(true);
			boardMock
				.Setup(b => b.IsEnemyOnPosition(It.IsIn<Position>(emptyPositions), ChessColor.White))
				.Returns(false);

			var movement = new KnightMovement(boardMock.Object);
			var availableMoves = movement.GetAvailableMoves(knightMock.Object);

			Assert.AreEqual(3, availableMoves.Count());
			Assert.IsTrue(availableMoves.All(m => m.StartingPosition == knightPosition));

			var finishedPositions = availableMoves
				.Select(m => m.FinishedPosition)
				.ToList();

			Assert.Contains(availableFinishPosition01, finishedPositions);
			Assert.Contains(availableFinishPosition02, finishedPositions);
			Assert.Contains(availableFinishPosition03, finishedPositions);
		}
		[Test]
		public void KnightCanCaptureEnemyPieces()
		{
			//WK - white knight
			//PM - possible move
			//EP - enemy piece
			//7                        
			//6                        
			//5                        
			//4                        
			//3                   EP   
			//2                PM      
			//1                      WK
			//0                PM      
			//  0  1  2  3  4  5  6  7 
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var knightMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);

			var knightPosition = new Position(7, 1);
			var enemyPiecePosition = new Position(6, 3);

			var availableFinishPosition01 = new Position(5, 0);
			var availableFinishPosition02 = new Position(5, 2);

			knightMock
				.SetupGet(p => p.Color)
				.Returns(ChessColor.White);
			knightMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Knight);
			knightMock
				.SetupGet(p => p.Position)
				.Returns(knightPosition);
			knightMock
				.SetupGet(p => p.HasMoved)
				.Returns(true);

			boardMock
				.Setup(b => b.IsPositionTaken(enemyPiecePosition))
				.Returns(true);
			boardMock
				.Setup(b => b.IsPositionTaken(availableFinishPosition01))
				.Returns(false);
			boardMock
				.Setup(b => b.IsPositionTaken(availableFinishPosition02))
				.Returns(false);
			boardMock
				.Setup(b => b.IsEnemyOnPosition(enemyPiecePosition, ChessColor.Black))
				.Returns(true);
			boardMock
				.Setup(b => b.IsEnemyOnPosition(availableFinishPosition01, ChessColor.Black))
				.Returns(false);
			boardMock
				.Setup(b => b.IsEnemyOnPosition(availableFinishPosition02, ChessColor.Black))
				.Returns(false);

			var movement = new KnightMovement(boardMock.Object);
			var availableMoves = movement.GetAvailableMoves(knightMock.Object);

			Assert.AreEqual(3, availableMoves.Count());
			Assert.IsTrue(availableMoves.All(m => m.StartingPosition == knightPosition));

			var finishedPositions = availableMoves
				.Select(m => m.FinishedPosition)
				.ToList();

			Assert.Contains(availableFinishPosition01, finishedPositions);
			Assert.Contains(availableFinishPosition02, finishedPositions);
			Assert.Contains(enemyPiecePosition, finishedPositions);
		}
		[Test]
		public void KnightCantCaptureFriendlyPieces()
		{
			//BK - black knight
			//PM - possible move
			//BP - black piece
			//7                   BK   
			//6             BP         
			//5                BP    BP
			//4                        
			//3                        
			//2                        
			//1                        
			//0                        
			//  0  1  2  3  4  5  6  7 
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var knightMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);

			var knightPosition = new Position(6, 7);
			var blackPiece1Position = new Position(4, 6);
			var blackPiece2Position = new Position(5, 5);
			var blackPiece3Position = new Position(7, 5);

			knightMock
				.SetupGet(p => p.Color)
				.Returns(ChessColor.Black);
			knightMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Knight);
			knightMock
				.SetupGet(p => p.Position)
				.Returns(knightPosition);
			knightMock
				.SetupGet(p => p.HasMoved)
				.Returns(true);

			boardMock
				.Setup(b => b.IsPositionTaken(blackPiece1Position))
				.Returns(true);
			boardMock
				.Setup(b => b.IsPositionTaken(blackPiece2Position))
				.Returns(true);
			boardMock
				.Setup(b => b.IsPositionTaken(blackPiece3Position))
				.Returns(true);
			boardMock
				.Setup(b => b.IsEnemyOnPosition(blackPiece1Position, ChessColor.White))
				.Returns(false);
			boardMock
				.Setup(b => b.IsEnemyOnPosition(blackPiece2Position, ChessColor.White))
				.Returns(false);
			boardMock
				.Setup(b => b.IsEnemyOnPosition(blackPiece3Position, ChessColor.White))
				.Returns(false);

			var movement = new KnightMovement(boardMock.Object);
			var availableMoves = movement.GetAvailableMoves(knightMock.Object);

			Assert.AreEqual(0, availableMoves.Count());
		}
	}
}