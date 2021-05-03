using System.Linq;
using Chess.Pieces;
using Chess.Movement;

namespace Chess.Board
{
	public class CheckDetector : ICheckDetector
	{
		private readonly IChessBoard chessBoard;
		private readonly IMovement movement;
		public CheckDetector(IChessBoard chessBoard, IMovement movement)
		{
			this.chessBoard = chessBoard;
			this.movement = movement;
		}
		public bool IsChecked(ChessColor kingColor)
		{
			var king = chessBoard.GetKing(kingColor);
			return chessBoard.Pieces
				.Where(p => p.Color != kingColor)
				.Any(p => movement
					.GetAvailableMoves(p)
					.Any(m => m.FinishedPosition == king.Position)
				);
		}
	}
}