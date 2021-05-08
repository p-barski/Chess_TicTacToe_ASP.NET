using System;
using System.Linq;
using System.Collections.Generic;
using Chess.Pieces;
using Chess.Board;

namespace Chess.Movement
{
	///<summary>
	///Responsible for calculating available legal or illegal en passant move of a pawn.
	///</summary>
	public class EnPassantMovement : IMovement
	{
		private const int boardSize = 8;
		private const int blackYPositionForEnPassant = 3;
		private const int whiteYPositionForEnPassant = 4;
		private readonly IChessBoard chessBoard;
		public EnPassantMovement(IChessBoard chessBoard)
		{
			this.chessBoard = chessBoard;
		}
		public IEnumerable<ChessMove> GetAvailableMoves(IReadOnlyChessPiece chessPiece)
		{
			var availableMoves = new List<ChessMove>();
			if (chessPiece.PieceType != ChessPieceType.Pawn)
			{
				return availableMoves;
			}
			if (!IsPieceOnCorrectRow(chessPiece))
			{
				return availableMoves;
			}
			if (chessBoard.History.ChessMoves.Count == 0)
			{
				return availableMoves;
			}

			var lastMove = chessBoard.History.ChessMoves[^1];
			if (!WasLastMovePawnDoubleMove(lastMove))
			{
				return availableMoves;
			}

			var xDirection = lastMove.FinishedPosition.X - chessPiece.Position.X;
			if (Math.Abs(xDirection) != 1)
			{
				return availableMoves;
			}

			int yDirection = CalculateYDirection(chessPiece.Color);
			var destination = new Position(chessPiece.Position.X + xDirection,
				chessPiece.Position.Y + yDirection);
			var chessMove = new ChessMove(chessPiece.Position, destination);
			availableMoves.Add(chessMove);

			return availableMoves;
		}
		private int CalculateYDirection(ChessColor color)
		{
			if (color == ChessColor.White)
			{
				return 1;
			}
			return -1;
		}
		private bool WasLastMovePawnDoubleMove(ChessMove lastMove)
		{
			var movedPiece = chessBoard.Pieces
				.FirstOrDefault(p => p.Position == lastMove.FinishedPosition);
			if (movedPiece == null)
			{
				return false;
			}
			if (movedPiece.PieceType != ChessPieceType.Pawn)
			{
				return false;
			}
			int difference = lastMove.FinishedPosition.Y - lastMove.StartingPosition.Y;
			return Math.Abs(difference) == 2;
		}
		private bool IsPieceOnCorrectRow(IReadOnlyChessPiece pawn)
		{
			if (pawn.Color == ChessColor.Black)
			{
				return pawn.Position.Y == blackYPositionForEnPassant;
			}
			return pawn.Position.Y == whiteYPositionForEnPassant;
		}
	}
}