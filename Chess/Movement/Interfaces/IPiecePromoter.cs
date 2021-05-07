using System.Collections.Generic;
using Chess.Pieces;

namespace Chess.Movement
{
	public interface IPiecePromoter
	{
		///<returns>
		///True if it was promotion move and pawn was promoted, false otherwise.
		///</returns>
		bool PromoteIfPromotionMove(ChessMove chessMove,
			IEnumerable<IChessPiece> pieces);
		///<returns>
		///Pawn move that happened before promotion.
		///</returns>
		ChessMove DepromoteIfPromotionMove(ChessMove chessMove,
			IEnumerable<IChessPiece> pieces);
	}
}