using System.Linq;
using Chess.Game;
using Chess.Pieces;
using Chess.Movement;

namespace Chess.Board
{
	public class StalemateDetector : IStalemateDetector
	{
		private readonly IChessBoard chessBoard;
		private readonly ILegalMovement legalMovement;
		public StalemateDetector(IChessBoard chessBoard, ILegalMovement legalMovement)
		{
			this.chessBoard = chessBoard;
			this.legalMovement = legalMovement;
		}
		public ChessPlayResult IsStalemate(ChessColor kingColor)
		{
			var pieceThatCanMove = chessBoard.Pieces
				.Where(p => p.Color == kingColor)
				.ToList()
				.FirstOrDefault(p => legalMovement
					.GetAvailableLegalMoves(p)
					.Any());

			if (pieceThatCanMove == null)
			{
				return ChessPlayResult.Stalemate;
			}
			return ChessPlayResult.SuccessfulMove;
		}
	}
}