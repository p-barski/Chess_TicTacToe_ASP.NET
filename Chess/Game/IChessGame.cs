using System.Collections.Generic;
using Chess.Pieces;
using Chess.Movement;

namespace Chess.Game
{
	///<summary>
	///Controls the flow of the game.
	///Gives access to readonly elements of the game.
	///</summary>
	public interface IChessGame
	{
		ChessColor CurrentPlayer { get; }
		IReadOnlyMovementHistory MovementHistory { get; }
		///<returns>
		///Available pieces in the game.
		///</returns>
		IEnumerable<IReadOnlyChessPiece> Pieces { get; }
		///<summary>
		///Tries to apply chess move for given player.
		///</summary>
		ChessPlayResult Play(ChessMove chessMove, ChessColor player);
		IEnumerable<ChessMove> GetAvailableLegalMoves(ChessColor player);
	}
}