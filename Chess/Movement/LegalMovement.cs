using System.Linq;
using System.Collections.Generic;
using Chess.Board;
using Chess.Pieces;

namespace Chess.Movement
{
	public class LegalMovement : ILegalMovement
	{
		private readonly IChessBoard chessBoard;
		private readonly IMovement movement;
		private readonly ICheckDetector checkDetector;
		public LegalMovement(IChessBoard chessBoard,
			IMovement movement, ICheckDetector checkDetector)
		{
			this.chessBoard = chessBoard;
			this.movement = movement;
			this.checkDetector = checkDetector;
		}
		public IEnumerable<ChessMove> GetAvailableLegalMoves(IReadOnlyChessPiece chessPiece)
		{
			var filteredMoves = new List<ChessMove>();
			foreach (var chessMove in movement.GetAvailableMoves(chessPiece))
			{
				chessBoard.Move(chessMove);
				if (!checkDetector.IsChecked(chessPiece.Color))
				{
					filteredMoves.Add(chessMove);
				}
				chessBoard.ReverseLastMove();
			}
			return filteredMoves;
		}
		public bool HasAnyLegalMoves(ChessColor playerColor)
		{
			var pieceThatCanMove = chessBoard.Pieces
				.Where(p => p.Color == playerColor)
				.ToList()
				.FirstOrDefault(p =>
					GetAvailableLegalMoves(p)
					.Any());

			return pieceThatCanMove != null;
		}
	}
}