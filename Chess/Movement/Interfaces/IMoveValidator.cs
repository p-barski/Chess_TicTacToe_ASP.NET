using Chess.Pieces;

namespace Chess.Movement
{
	public interface IMoveValidator
	{
		bool ValidateAndMove(ChessMove chessMove, ChessColor playerColor);
	}
}