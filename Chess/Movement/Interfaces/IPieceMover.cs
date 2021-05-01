using System.Collections.Generic;
using Chess.Pieces;

namespace Chess.Movement
{
	public interface IPieceMover
	{
		IReadOnlyMovementHistory History { get; }
		///<returns>
		///Chess piece that should be removed from board.
		///Null if no piece should be removed.
		///</returns>
		IChessPiece Move(ChessMove chessMove, IEnumerable<IChessPiece> pieces);
		///<returns>
		///Boolean whether to add last removed piece to available pieces.
		///</returns>
		bool ReverseLastMove(IEnumerable<IChessPiece> pieces);
	}
}