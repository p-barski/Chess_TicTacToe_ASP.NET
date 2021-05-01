using System.Linq;
using Chess.Game;
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
		public ChessPlayResult IsChecked(ChessColor kingColor)
		{
			var king = chessBoard.GetKing(kingColor);
			var isChecked = chessBoard.Pieces
				.Where(p => p.Color != kingColor)
				.Any(p => movement
					.GetAvailableMoves(p)
					.Any(m => m.FinishedPosition == king.Position)
				);
			if (isChecked)
			{
				return ReturnCheck(kingColor);
			}
			return ChessPlayResult.SuccessfulMove;
		}
		private ChessPlayResult ReturnCheck(ChessColor kingColor)
		{
			if (kingColor == ChessColor.Black)
			{
				return ChessPlayResult.BlackChecked;
			}
			return ChessPlayResult.WhiteChecked;
		}
	}
}