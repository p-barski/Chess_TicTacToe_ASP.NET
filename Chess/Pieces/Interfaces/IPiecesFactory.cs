using System.Collections.Generic;

namespace Chess.Pieces
{
	public interface IPiecesFactory
	{
		IEnumerable<IChessPiece> Create();
	}
}