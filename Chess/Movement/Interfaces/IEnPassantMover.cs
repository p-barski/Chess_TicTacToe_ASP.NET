using System.Collections.Generic;
using Chess.Pieces;

namespace Chess.Movement
{
	public interface IEnPassantMover
	{
		///<returns>
		///Chess piece to remove if it was en passant move and en passant move was performed,
		///null otherwise.
		///</returns>
		IChessPiece PerformEnPassantIfApplicable(ChessMove chessMove,
			IEnumerable<IChessPiece> pieces);
	}
}