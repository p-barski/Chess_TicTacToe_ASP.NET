using System.Collections.Generic;
using Chess.Pieces;

namespace Chess.Movement
{
	public interface ILegalMovement
	{
		///<summary>
		///Makes sure that moves won't check current player.
		///</summary>
		IEnumerable<ChessMove> GetAvailableLegalMoves(IReadOnlyChessPiece chessPiece);
	}
}