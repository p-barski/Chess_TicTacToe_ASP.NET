using System.Threading.Tasks;
using System.Collections.Generic;
using Server.Database.Chess;

namespace Server.Database
{
	public interface IChessDatabase
	{
		Task SaveGameAsync(ChessGameDb chessGame);
		IEnumerable<ChessGameDb> GetAllSavedGames();
	}
}