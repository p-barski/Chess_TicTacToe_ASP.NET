using System.Linq;
using Chess.Pieces;

namespace Chess.Board
{
	public class PromotionDetector : IPromotionDetector
	{
		private const int blackYPromotionPosition = 0;
		private const int whiteYPromotionPosition = 7;
		private readonly IChessBoard chessBoard;
		public PromotionDetector(IChessBoard chessBoard)
		{
			this.chessBoard = chessBoard;
		}
		public bool IsPromotionRequired()
		{
			return chessBoard.Pieces
				.Any(p =>
					p.PieceType == ChessPieceType.Pawn &&
					(p.Position.Y == blackYPromotionPosition ||
					p.Position.Y == whiteYPromotionPosition));
		}
	}
}