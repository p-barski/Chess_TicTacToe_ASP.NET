using System;
using Chess.Movement;

namespace Server.Database.Chess
{
	public class ChessMoveConverter : IChessMoveConverter
	{
		public ChessMoveDb ConvertToDb(ChessMove move)
		{
			return new ChessMoveDb(move.StartingPosition.X,
				move.StartingPosition.Y, move.FinishedPosition.X,
				move.FinishedPosition.Y, move.PawnPromotion.ToString(),
				DateTime.UtcNow);
		}
	}
}