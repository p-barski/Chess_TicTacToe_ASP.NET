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
	public class KingsideCastlingMovementTests
	{
		[Test]
		public void WhenCastlingIsPossibleReturnListWithChessMove()
		{
			//WK - white king
			//WR - white rook
			//7                        
			//6                        
			//5                        
			//4                        
			//3                        
			//2                        
			//1                        
			//0             WK       WR
			//  0  1  2  3  4  5  6  7 
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var movementMock = new Mock<IMovement>(MockBehavior.Strict);
			var kingMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var rookMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);

			var kingPosition = new Position(4, 0);
			var rookPosition = new Position(7, 0);

			var emptyPositions = new List<Position>(){
				new Position(5, 0), new Position(6, 0)
			};

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
				.Returns(false);

			rookMock
				.SetupGet(p => p.Color)
				.Returns(ChessColor.White);
			rookMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Rook);
			rookMock
				.SetupGet(p => p.Position)
				.Returns(rookPosition);
			rookMock
				.SetupGet(p => p.HasMoved)
				.Returns(false);

			var pieces = new List<IReadOnlyChessPiece>(){
				kingMock.Object, rookMock.Object
			};

			boardMock
				.Setup(b => b.IsPositionTaken(It.IsIn<Position>(emptyPositions)))
				.Returns(false);
			boardMock
				.SetupGet(b => b.Pieces)
				.Returns(pieces);

			var movement = new KingsideCastlingMovement(boardMock.Object,
				movementMock.Object);
			var availableMoves = movement.GetAvailableMoves(kingMock.Object);

			Assert.AreEqual(1, availableMoves.Count());
			var castlingMove = availableMoves.First();
			Assert.AreEqual(kingPosition, castlingMove.StartingPosition);
			Assert.AreEqual(rookPosition, castlingMove.FinishedPosition);
		}
		[Test]
		public void WhenKingIsCheckedReturnEmptyList()
		{
			//BK - black king
			//BR - black rook
			//WQ - white queen
			//7             BK       BR
			//6                        
			//5                        
			//4                        
			//3                        
			//2                        
			//1                        
			//0             WQ         
			//  0  1  2  3  4  5  6  7 
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var movementMock = new Mock<IMovement>(MockBehavior.Strict);
			var kingMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var rookMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var whiteQueenMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);

			var kingPosition = new Position(4, 7);
			var rookPosition = new Position(7, 7);
			var whiteQueenPosition = new Position(4, 0);

			var emptyPositions = new List<Position>(){
				new Position(5, 7), new Position(6, 7)
			};

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
				.Returns(false);

			rookMock
				.SetupGet(p => p.Color)
				.Returns(ChessColor.Black);
			rookMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Rook);
			rookMock
				.SetupGet(p => p.Position)
				.Returns(rookPosition);
			rookMock
				.SetupGet(p => p.HasMoved)
				.Returns(false);

			whiteQueenMock
				.SetupGet(p => p.Color)
				.Returns(ChessColor.White);
			whiteQueenMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Queen);
			whiteQueenMock
				.SetupGet(p => p.Position)
				.Returns(whiteQueenPosition);
			whiteQueenMock
				.SetupGet(p => p.HasMoved)
				.Returns(true);

			var pieces = new List<IReadOnlyChessPiece>(){
				kingMock.Object, rookMock.Object, whiteQueenMock.Object
			};

			boardMock
				.Setup(b => b.IsPositionTaken(It.IsIn<Position>(emptyPositions)))
				.Returns(false);
			boardMock
				.SetupGet(b => b.Pieces)
				.Returns(pieces);

			var queenMoves = new List<ChessMove>(){
				new ChessMove(whiteQueenPosition, kingPosition)
			};

			movementMock
				.Setup(m => m.GetAvailableMoves(whiteQueenMock.Object))
				.Returns(queenMoves);

			var movement = new KingsideCastlingMovement(boardMock.Object,
				movementMock.Object);
			var availableMoves = movement.GetAvailableMoves(kingMock.Object);

			Assert.AreEqual(0, availableMoves.Count());
		}
		[Test]
		public void WhenKingWouldEndUpCheckedAfterCastlingReturnEmptyList()
		{
			//WK - white king
			//WR - white rook
			//BQ - black queen
			//7                   BQ   
			//6                        
			//5                        
			//4                        
			//3                        
			//2                        
			//1                        
			//0             WK       WR
			//  0  1  2  3  4  5  6  7 
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var movementMock = new Mock<IMovement>(MockBehavior.Strict);
			var kingMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var rookMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var blackQueenMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);

			var kingPosition = new Position(4, 0);
			var rookPosition = new Position(7, 0);
			var blackQueenPosition = new Position(6, 7);

			var emptyPositions = new List<Position>(){
				new Position(5, 0), new Position(6, 0)
			};

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
				.Returns(false);

			rookMock
				.SetupGet(p => p.Color)
				.Returns(ChessColor.White);
			rookMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Rook);
			rookMock
				.SetupGet(p => p.Position)
				.Returns(rookPosition);
			rookMock
				.SetupGet(p => p.HasMoved)
				.Returns(false);

			blackQueenMock
				.SetupGet(p => p.Color)
				.Returns(ChessColor.Black);
			blackQueenMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Queen);
			blackQueenMock
				.SetupGet(p => p.Position)
				.Returns(blackQueenPosition);
			blackQueenMock
				.SetupGet(p => p.HasMoved)
				.Returns(true);

			var pieces = new List<IReadOnlyChessPiece>(){
				kingMock.Object, rookMock.Object, blackQueenMock.Object
			};

			boardMock
				.Setup(b => b.IsPositionTaken(It.IsIn<Position>(emptyPositions)))
				.Returns(false);
			boardMock
				.SetupGet(b => b.Pieces)
				.Returns(pieces);

			var queenMoves = new List<ChessMove>(){
				new ChessMove(blackQueenPosition, new Position(6, 0))
			};

			movementMock
				.Setup(m => m.GetAvailableMoves(blackQueenMock.Object))
				.Returns(queenMoves);

			var movement = new KingsideCastlingMovement(boardMock.Object,
				movementMock.Object);
			var availableMoves = movement.GetAvailableMoves(kingMock.Object);

			Assert.AreEqual(0, availableMoves.Count());
		}
		[Test]
		public void WhenKingPassesThroughAttackedSquareReturnEmptyList()
		{
			//BK - black king
			//BR - black rook
			//WQ - white queen
			//7             BK       BR
			//6                        
			//5                        
			//4                        
			//3                        
			//2                        
			//1                        
			//0                WQ      
			//  0  1  2  3  4  5  6  7 
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var movementMock = new Mock<IMovement>(MockBehavior.Strict);
			var kingMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var rookMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var whiteQueenMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);

			var kingPosition = new Position(4, 7);
			var rookPosition = new Position(7, 7);
			var whiteQueenPosition = new Position(5, 0);

			var emptyPositions = new List<Position>(){
				new Position(5, 7), new Position(6, 7)
			};

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
				.Returns(false);

			rookMock
				.SetupGet(p => p.Color)
				.Returns(ChessColor.Black);
			rookMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Rook);
			rookMock
				.SetupGet(p => p.Position)
				.Returns(rookPosition);
			rookMock
				.SetupGet(p => p.HasMoved)
				.Returns(false);

			whiteQueenMock
				.SetupGet(p => p.Color)
				.Returns(ChessColor.White);
			whiteQueenMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Queen);
			whiteQueenMock
				.SetupGet(p => p.Position)
				.Returns(whiteQueenPosition);
			whiteQueenMock
				.SetupGet(p => p.HasMoved)
				.Returns(true);

			var pieces = new List<IReadOnlyChessPiece>(){
				kingMock.Object, rookMock.Object, whiteQueenMock.Object
			};

			boardMock
				.Setup(b => b.IsPositionTaken(It.IsIn<Position>(emptyPositions)))
				.Returns(false);
			boardMock
				.SetupGet(b => b.Pieces)
				.Returns(pieces);

			var queenMoves = new List<ChessMove>(){
				new ChessMove(whiteQueenPosition, new Position(5, 7))
			};

			movementMock
				.Setup(m => m.GetAvailableMoves(whiteQueenMock.Object))
				.Returns(queenMoves);

			var movement = new KingsideCastlingMovement(boardMock.Object,
				movementMock.Object);
			var availableMoves = movement.GetAvailableMoves(kingMock.Object);

			Assert.AreEqual(0, availableMoves.Count());
		}
		[Test]
		public void WhenPieceIsBlockingTheWayReturnsEmptyList()
		{
			//WK - white king
			//WR - white rook
			//WP - white piece
			//7                        
			//6                        
			//5                        
			//4                        
			//3                        
			//2                        
			//1                        
			//0             WK    WP WR
			//  0  1  2  3  4  5  6  7 
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var movementMock = new Mock<IMovement>(MockBehavior.Strict);
			var kingMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var rookMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var otherPieceMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);

			var kingPosition = new Position(4, 0);
			var rookPosition = new Position(7, 0);
			var otherPiecePosition = new Position(6, 0);

			var emptyPositions = new List<Position>(){
				new Position(5, 0)
			};

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
				.Returns(false);

			rookMock
				.SetupGet(p => p.Color)
				.Returns(ChessColor.White);
			rookMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Rook);
			rookMock
				.SetupGet(p => p.Position)
				.Returns(rookPosition);
			rookMock
				.SetupGet(p => p.HasMoved)
				.Returns(false);

			otherPieceMock
				.SetupGet(p => p.Color)
				.Returns(ChessColor.White);
			otherPieceMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Queen);
			otherPieceMock
				.SetupGet(p => p.Position)
				.Returns(otherPiecePosition);
			otherPieceMock
				.SetupGet(p => p.HasMoved)
				.Returns(true);

			var pieces = new List<IReadOnlyChessPiece>(){
				kingMock.Object, rookMock.Object, otherPieceMock.Object
			};

			boardMock
				.Setup(b => b.IsPositionTaken(It.IsIn<Position>(emptyPositions)))
				.Returns(false);
			boardMock
				.Setup(b => b.IsPositionTaken(otherPiecePosition))
				.Returns(true);
			boardMock
				.SetupGet(b => b.Pieces)
				.Returns(pieces);

			var movement = new KingsideCastlingMovement(boardMock.Object,
				movementMock.Object);
			var availableMoves = movement.GetAvailableMoves(kingMock.Object);

			Assert.AreEqual(0, availableMoves.Count());
		}
		[Test]
		public void WhenKingHasMovedReturnsEmptyList()
		{
			//BK - black king
			//BR - black rook
			//7             BK       BR
			//6                        
			//5                        
			//4                        
			//3                        
			//2                        
			//1                        
			//0                        
			//  0  1  2  3  4  5  6  7 
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var movementMock = new Mock<IMovement>(MockBehavior.Strict);
			var kingMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);

			var kingPosition = new Position(4, 7);

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

			var movement = new KingsideCastlingMovement(boardMock.Object,
				movementMock.Object);
			var availableMoves = movement.GetAvailableMoves(kingMock.Object);

			Assert.AreEqual(0, availableMoves.Count());
		}
		[Test]
		public void WhenRookHasMovedReturnsEmptyList()
		{
			//WK - white king
			//WR - white rook
			//7                        
			//6                        
			//5                        
			//4                        
			//3                        
			//2                        
			//1                        
			//0             WK       WR
			//  0  1  2  3  4  5  6  7 
			var boardMock = new Mock<IChessBoard>(MockBehavior.Strict);
			var movementMock = new Mock<IMovement>(MockBehavior.Strict);
			var kingMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);
			var rookMock = new Mock<IReadOnlyChessPiece>(MockBehavior.Strict);

			var kingPosition = new Position(4, 0);
			var rookPosition = new Position(7, 0);

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
				.Returns(false);

			rookMock
				.SetupGet(p => p.Color)
				.Returns(ChessColor.White);
			rookMock
				.SetupGet(p => p.PieceType)
				.Returns(ChessPieceType.Rook);
			rookMock
				.SetupGet(p => p.Position)
				.Returns(rookPosition);
			rookMock
				.SetupGet(p => p.HasMoved)
				.Returns(true);

			var pieces = new List<IReadOnlyChessPiece>(){
				kingMock.Object, rookMock.Object
			};

			boardMock
				.SetupGet(b => b.Pieces)
				.Returns(pieces);

			var movement = new KingsideCastlingMovement(boardMock.Object,
				movementMock.Object);
			var availableMoves = movement.GetAvailableMoves(kingMock.Object);

			Assert.AreEqual(0, availableMoves.Count());
		}
	}
}