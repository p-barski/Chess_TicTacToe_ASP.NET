using System.Collections.Generic;
using Chess.Pieces;
using Chess.Board;

namespace Chess.Movement
{
	///<summary>
	///Responsible for calculating available legal or illegal moves of a pawn.
	///</summary>
	public class PawnMovement : IMovement
	{
		private const int boardSize = 8;
		private readonly IChessBoard chessBoard;
		public PawnMovement(IChessBoard chessBoard)
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

			int yDirection = CalculateYDirection(chessPiece.Color);

			AddNormalMoves(chessPiece, yDirection, availableMoves);
			AddPossibleRightCapture(chessPiece, yDirection, availableMoves);
			AddPossibleLeftCapture(chessPiece, yDirection, availableMoves);

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
		private void AddNormalMoves(IReadOnlyChessPiece chessPiece, int yDirection,
			List<ChessMove> availableMoves)
		{
			var possibleDestination = new Position(chessPiece.Position.X,
				chessPiece.Position.Y + yDirection);
			if (chessBoard.IsPositionTaken(possibleDestination))
			{
				return;
			}

			var chessMove = new ChessMove(chessPiece.Position, possibleDestination);
			availableMoves.Add(chessMove);
			AddDoubleMove(chessPiece, yDirection, availableMoves);
		}
		private void AddDoubleMove(IReadOnlyChessPiece chessPiece, int yDirection,
			List<ChessMove> availableMoves)
		{
			if (chessPiece.HasMoved)
			{
				return;
			}
			var possibleDestination = new Position(chessPiece.Position.X,
				chessPiece.Position.Y + (yDirection * 2));
			if (chessBoard.IsPositionTaken(possibleDestination))
			{
				return;
			}
			var chessMove = new ChessMove(chessPiece.Position, possibleDestination);
			availableMoves.Add(chessMove);
		}
		private void AddPossibleRightCapture(IReadOnlyChessPiece chessPiece, int yDirection,
			List<ChessMove> availableMoves)
		{
			var xPosition = chessPiece.Position.X + 1;
			if (xPosition >= boardSize)
			{
				return;
			}

			var possibleCapturePosition = new Position(xPosition,
				chessPiece.Position.Y + yDirection);

			if (!chessBoard.IsEnemyOnPosition(possibleCapturePosition,
				chessPiece.Color.Opposite()))
			{
				return;
			}

			var chessMove = new ChessMove(chessPiece.Position, possibleCapturePosition);
			availableMoves.Add(chessMove);
		}
		private void AddPossibleLeftCapture(IReadOnlyChessPiece chessPiece, int yDirection,
			List<ChessMove> availableMoves)
		{
			var xPosition = chessPiece.Position.X - 1;
			if (xPosition < 0)
			{
				return;
			}

			var possibleCapturePosition = new Position(xPosition,
				chessPiece.Position.Y + yDirection);

			if (!chessBoard.IsEnemyOnPosition(possibleCapturePosition,
				chessPiece.Color.Opposite()))
			{
				return;
			}

			var chessMove = new ChessMove(chessPiece.Position, possibleCapturePosition);
			availableMoves.Add(chessMove);
		}
	}
}