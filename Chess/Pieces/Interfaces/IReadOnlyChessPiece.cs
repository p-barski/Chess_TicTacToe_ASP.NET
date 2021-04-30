namespace Chess.Pieces
{
	public interface IReadOnlyChessPiece
	{
		ChessPieceType PieceType { get; }
		ChessColor Color { get; }
		Position Position { get; }
		bool HasMoved { get; }
	}
}