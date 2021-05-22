using System;
using System.Collections.Generic;

namespace Server.Database.Chess
{
	public class ChessGameDb
	{
		public int Id { get; private set; }
		public List<ChessMoveDb> ChessMoves { get; private set; }
		public PlayerData WhitePlayer { get; private set; }
		public PlayerData BlackPlayer { get; private set; }
		public DateTime StartDate { get; private set; }
		public DateTime FinishDate { get; private set; }
		public string Result { get; private set; }
		public ChessGameDb() { }
		public ChessGameDb(List<ChessMoveDb> chessMoves, PlayerData whitePlayer,
			PlayerData blackPlayer, DateTime startDate, DateTime finishDate,
			string result)
		{
			ChessMoves = chessMoves;
			WhitePlayer = whitePlayer;
			BlackPlayer = blackPlayer;
			StartDate = startDate;
			FinishDate = finishDate;
			Result = result;
		}
	}
}