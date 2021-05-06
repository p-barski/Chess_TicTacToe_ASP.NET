using System.Linq;
using Chess.Board;
using Chess.Pieces;

namespace Chess.Movement
{
	public class MoveValidator : IMoveValidator
	{
		private readonly ILegalMovement legalMovement;
		private readonly IChessBoard chessBoard;
		private readonly IPromotionDetector promotionDetector;
		public MoveValidator(IChessBoard chessBoard, ILegalMovement legalMovement,
			IPromotionDetector promotionDetector)
		{
			this.chessBoard = chessBoard;
			this.legalMovement = legalMovement;
			this.promotionDetector = promotionDetector;
		}
		public bool ValidateAndMove(ChessMove chessMove, ChessColor playerColor)
		{
			if (promotionDetector.IsPromotionRequired())
			{
				return ValidateAndPromote(chessMove);
			}
			var piece = chessBoard.Pieces
				.FirstOrDefault(p => p.Position == chessMove.StartingPosition);
			if (piece == null)
			{
				return false;
			}
			if (piece.Color != playerColor)
			{
				return false;
			}

			var movements = legalMovement.GetAvailableLegalMoves(piece);
			if (!movements.Contains(chessMove))
			{
				return false;
			}

			chessBoard.Move(chessMove);

			return true;
		}
		private bool ValidateAndPromote(ChessMove chessMove)
		{
			if (!IsPromotionMove(chessMove))
			{
				return false;
			}
			chessBoard.Move(chessMove);
			return true;
		}
		private bool IsPromotionMove(ChessMove chessMove)
		{
			return
				chessMove.PawnPromotion != ChessPieceType.Pawn &&
				chessMove.PawnPromotion != ChessPieceType.King;
		}
	}
}