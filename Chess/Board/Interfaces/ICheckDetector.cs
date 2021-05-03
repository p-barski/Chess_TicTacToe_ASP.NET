using Chess.Pieces;

namespace Chess.Board
{
	public interface ICheckDetector
	{
		bool IsChecked(ChessColor kingColor);
	}
}