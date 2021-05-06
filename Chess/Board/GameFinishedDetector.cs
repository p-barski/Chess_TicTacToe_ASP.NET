using Chess.Game;
using Chess.Pieces;
using Chess.Movement;

namespace Chess.Board
{
	public class GameFinishedDetector : IGameFinishedDetector
	{
		private readonly ICheckDetector checkDetector;
		private readonly ILegalMovement legalMovement;
		public GameFinishedDetector(ICheckDetector checkDetector,
			ILegalMovement legalMovement)
		{
			this.checkDetector = checkDetector;
			this.legalMovement = legalMovement;
		}
		public ChessPlayResult IsGameFinished(ChessColor kingColor)
		{
			var canMove = legalMovement.HasAnyLegalMoves(kingColor);
			var isChecked = checkDetector.IsChecked(kingColor);
			if (isChecked && canMove)
			{
				return ReturnCheck(kingColor);
			}
			if (isChecked)
			{
				return ReturnCheckmate(kingColor);
			}
			if (!canMove)
			{
				return ChessPlayResult.Stalemate;
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
		private ChessPlayResult ReturnCheckmate(ChessColor kingColor)
		{
			if (kingColor == ChessColor.Black)
			{
				return ChessPlayResult.WhiteWin;
			}
			return ChessPlayResult.BlackWin;
		}
	}
}