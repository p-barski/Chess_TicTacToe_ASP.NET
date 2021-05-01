namespace Chess.Pieces
{
	public interface IChessPiece : IReadOnlyChessPiece
	{
		new Position Position { get; set; }
		void IncrementMoveCounter();
		void DecrementMoveCounter();
	}
}