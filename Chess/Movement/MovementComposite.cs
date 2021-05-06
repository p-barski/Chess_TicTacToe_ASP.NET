using System.Linq;
using System.Collections.Generic;
using Chess.Pieces;

namespace Chess.Movement
{
	public class MovementComposite : IMovement
	{
		private readonly IReadOnlyList<IMovement> movements;
		public MovementComposite(IEnumerable<IMovement> movements)
		{
			this.movements = movements.ToList();
		}
		public IEnumerable<ChessMove> GetAvailableMoves(IReadOnlyChessPiece chessPiece)
		{
			return movements
				.SelectMany(m => m.GetAvailableMoves(chessPiece))
				.ToList();
		}
	}
}