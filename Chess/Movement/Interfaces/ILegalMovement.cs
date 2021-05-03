using System.Collections.Generic;
using Chess.Pieces;

namespace Chess.Movement
{
	public interface ILegalMovement
	{
		///<summary>
		///Makes sure that moves won't result in a check of current player.
		///</summary>
		IEnumerable<ChessMove> GetAvailableLegalMoves(IReadOnlyChessPiece chessPiece);
		bool HasAnyLegalMoves(ChessColor playerColor);
	}
}