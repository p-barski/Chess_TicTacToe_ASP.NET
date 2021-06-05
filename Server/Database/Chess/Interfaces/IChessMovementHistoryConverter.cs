using Chess.Movement;

namespace Server.Database.Chess
{
	public interface IChessMoveConverter
	{
		ChessMoveDb ConvertToDb(ChessMove move);
	}
}