using Chess.Game;

namespace Chess
{
	public interface IChessGameFactory
	{
		IChessGame Create();
	}
}