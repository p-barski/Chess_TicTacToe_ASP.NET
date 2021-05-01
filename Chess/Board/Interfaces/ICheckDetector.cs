using Chess.Game;
using Chess.Pieces;

namespace Chess.Board
{
	public interface ICheckDetector
	{
		ChessPlayResult IsChecked(ChessColor kingColor);
	}
}