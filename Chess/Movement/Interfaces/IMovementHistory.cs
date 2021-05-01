namespace Chess.Movement
{
	public interface IMovementHistory : IReadOnlyMovementHistory
	{
		void Add(ChessMove chessMove);
		ChessMove RemoveLastMove();
	}
}