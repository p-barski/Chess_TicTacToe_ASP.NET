using System.Collections.Generic;

namespace Chess.Movement
{
	public interface IReadOnlyMovementHistory
	{
		IReadOnlyList<ChessMove> ChessMoves { get; }
	}
}