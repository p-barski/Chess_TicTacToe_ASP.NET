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
		public DateTime FinishDate { get; set; }
		public string Result { get; set; }
		public ChessGameDb() { }
		public ChessGameDb(PlayerData whitePlayer,
			PlayerData blackPlayer, DateTime startDate)
		{
			ChessMoves = new();
			WhitePlayer = whitePlayer;
			BlackPlayer = blackPlayer;
			StartDate = startDate;
			FinishDate = DateTime.MinValue;
			Result = "";
		}
	}
}