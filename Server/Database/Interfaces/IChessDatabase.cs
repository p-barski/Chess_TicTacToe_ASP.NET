using System.Threading.Tasks;
using Server.Database.Chess;

namespace Server.Database
{
	public interface IChessDatabase
	{
		Task SaveGameAsync(ChessGameDb chessGame);
		Task UpdateGameAsync(ChessGameDb chessGame);
		Task<ChessGameDb> GetSavedGame(PlayerData whitePlayer, PlayerData blackPlayer);
	}
}