using System;
using System.Linq;
using Chess.Movement;

namespace Server.Database.Chess
{
	public class ChessMovementHistoryConverter : IChessMovementHistoryConverter
	{
		public ChessGameDb ConvertToDb(IReadOnlyMovementHistory movementHistory,
			PlayerData whitePlayer, PlayerData blackPlayer, DateTime startDate,
			DateTime finishDate, string result)
		{
			int idx = 0;
			var chessMovesDb = movementHistory.ChessMoves
				.Select(m =>
					new ChessMoveDb(idx++, m.StartingPosition.X,
						m.StartingPosition.Y, m.FinishedPosition.X,
						m.FinishedPosition.Y, m.PawnPromotion.ToString()))
				.ToList();
			return new ChessGameDb(chessMovesDb, whitePlayer, blackPlayer, startDate, finishDate, result);
		}
	}
}