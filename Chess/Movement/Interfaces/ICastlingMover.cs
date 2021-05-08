using System.Collections.Generic;
using Chess.Pieces;

namespace Chess.Movement
{
	public interface ICastlingMover
	{
		///<returns>
		///True if it was castling move and castling move was performed,
		///false otherwise.
		///</returns>
		bool PerformCastlingIfCastlingMove(ChessMove chessMove,
			IEnumerable<IChessPiece> pieces);
		///<returns>
		///True if it was castling move and castling move was reversed,
		///false otherwise.
		///</returns>
		bool UndoCastlingIfCastlingMove(ChessMove chessMove,
			IEnumerable<IChessPiece> pieces);
	}
}