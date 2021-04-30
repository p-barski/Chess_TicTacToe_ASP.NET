using System.Collections.Generic;

namespace Chess.Movement
{
	public interface IReadOnlyMovementHistory
	{
		List<ChessMove> ChessMoves { get; }
	}
}