using System.Linq;
using System.Collections.Generic;
using Chess.Pieces;
using Chess.Movement;

namespace Chess.Board
{
	public class ChessBoard : IChessBoard
	{
		public IEnumerable<IReadOnlyChessPiece> Pieces => readOnlyPieces;
		public IReadOnlyMovementHistory History => pieceMover.History;
		private readonly HashSet<IChessPiece> pieces;
		private readonly HashSet<IReadOnlyChessPiece> readOnlyPieces;
		private readonly List<IChessPiece> removedPieces = new();
		private readonly IPieceMover pieceMover;
		private readonly Dictionary<ChessColor, IReadOnlyChessPiece> kings = new();
		public ChessBoard(IPiecesFactory piecesFactory, IPieceMover pieceMover)
		{
			this.pieces = piecesFactory.Create().ToHashSet();
			this.readOnlyPieces = pieces.Cast<IReadOnlyChessPiece>().ToHashSet();
			this.pieceMover = pieceMover;
			kings.Add(ChessColor.White, GetWhiteKing());
			kings.Add(ChessColor.Black, GetBlackKing());
		}
		public IReadOnlyChessPiece GetKing(ChessColor kingColor) => kings[kingColor];
		public bool IsPositionTaken(Position position)
		{
			return pieces
				.Any(p => p.Position == position);
		}
		public bool IsEnemyOnPosition(Position position, ChessColor enemyColor)
		{
			return pieces
				.Any(p => p.Position == position && p.Color == enemyColor);
		}
		public void Move(ChessMove chessMove)
		{
			var piece = pieceMover.Move(chessMove, pieces);
			if (piece == null)
			{
				return;
			}
			pieces.Remove(piece);
			readOnlyPieces.Remove(piece);
			removedPieces.Add(piece);
		}
		public void ReverseLastMove()
		{
			if (!pieceMover.ReverseLastMove(pieces))
			{
				return;
			}
			var piece = removedPieces[^1];
			readOnlyPieces.Add(piece);
			pieces.Add(piece);
			removedPieces.RemoveAt(removedPieces.Count - 1);
		}
		private IReadOnlyChessPiece GetWhiteKing()
		{
			return GetKingOfColor(ChessColor.White);
		}
		private IReadOnlyChessPiece GetBlackKing()
		{
			return GetKingOfColor(ChessColor.Black);
		}
		private IReadOnlyChessPiece GetKingOfColor(ChessColor kingColor)
		{
			return readOnlyPieces
				.First(p =>
					p.PieceType == ChessPieceType.King &&
					p.Color == kingColor);
		}
	}
}