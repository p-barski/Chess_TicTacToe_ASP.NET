using System.Collections.Generic;
using Chess.Pieces;

namespace Chess.Movement
{
	public interface IMovement
	{
		///<returns>
		///Available legal and possibly illegal moves.
		///</returns>
		IEnumerable<ChessMove> GetAvailableMoves(IReadOnlyChessPiece chessPiece);
	}
}