using System.Collections.Generic;
using Server.Games.Chess;

namespace Server.Sockets.Messages
{
	public class ChessPiecesAndMovesMessage : ISendMessage
	{
		public List<ChessPieceWrapper> Pieces { get; set; }
		public List<ChessMoveWrapper> AvailableMoves { get; set; }
	}
}