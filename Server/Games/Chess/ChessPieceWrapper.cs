using Chess.Pieces;

namespace Server.Games.Chess
{
	public class ChessPieceWrapper
	{
		public IReadOnlyChessPiece ChessPiece { get; }
		public ChessPieceWrapper(IReadOnlyChessPiece chessPiece)
		{
			ChessPiece = chessPiece;
		}
	}
}