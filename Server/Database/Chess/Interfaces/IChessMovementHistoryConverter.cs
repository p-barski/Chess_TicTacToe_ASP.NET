using System;
using Chess.Movement;

namespace Server.Database.Chess
{
	public interface IChessMovementHistoryConverter
	{
		ChessGameDb ConvertToDb(IReadOnlyMovementHistory movementHistory,
			PlayerData whitePlayer, PlayerData blackPlayer, DateTime startDate,
			DateTime finishDate, string result);
	}
}