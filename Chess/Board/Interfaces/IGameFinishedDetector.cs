using Chess.Game;
using Chess.Pieces;

namespace Chess.Board
{
	public interface IGameFinishedDetector
	{
		///<returns>
		///BlackChecked, WhiteChecked, WhiteWin, BlackWin, Stalemate
		///or SuccessfulMove.
		///</returns>
		ChessPlayResult IsGameFinished(ChessColor kingColor);
	}
}