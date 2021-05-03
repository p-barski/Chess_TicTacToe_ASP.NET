using Chess.Game;
using Chess.Pieces;

namespace Chess.Board
{
	public interface IStalemateDetector
	{
		ChessPlayResult IsStalemate(ChessColor kingColor);
	}
}