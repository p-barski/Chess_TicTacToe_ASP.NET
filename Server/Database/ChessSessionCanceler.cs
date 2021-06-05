using System;
using System.Threading.Tasks;
using Server.Database;
using Server.Games;
using Server.Games.Chess;

namespace Server.Database
{
	public class ChessSessionCanceler : IChessSessionCanceler
	{
		private readonly IChessDatabase databaseAccess;
		public ChessSessionCanceler(IChessDatabase databaseAccess)
		{
			this.databaseAccess = databaseAccess;
		}
		public async Task TryCancel(IGameSession session, IPlayer cancelingPlayer)
		{
			ChessGameSession chessSession;
			try
			{
				chessSession = (ChessGameSession)session;
			}
			catch (InvalidCastException)
			{
				return;
			}

			var gameDb = await databaseAccess
				.GetSavedGame(session.PlayerOne.PlayerData, session.PlayerTwo.PlayerData);

			gameDb.FinishDate = DateTime.UtcNow;
			if (cancelingPlayer == session.PlayerOne)
			{
				gameDb.Result = "BlackWin";
			}
			else
			{
				gameDb.Result = "WhiteWin";
			}

			await databaseAccess.UpdateGameAsync(gameDb);
		}
	}
}