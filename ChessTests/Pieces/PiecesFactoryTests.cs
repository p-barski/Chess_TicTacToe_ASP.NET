using System.Linq;
using NUnit.Framework;
using Chess.Pieces;

namespace ChessTests
{
	public class PiecesFactoryTests
	{
		[Test]
		public void ThereAre32ChessPiecesCreated()
		{
			var factory = new PiecesFactory();
			var pieces = factory.Create();
			Assert.AreEqual(32, pieces.Count());
		}
		[Test]
		public void ThereIsWhiteKingCreated()
		{
			var factory = new PiecesFactory();
			var pieces = factory.Create();
			var filteredPieces = pieces
				.Where(p => p.PieceType == ChessPieceType.King &&
					p.Color == ChessColor.White)
				.ToList();
			Assert.AreEqual(1, filteredPieces.Count);
		}
		[Test]
		public void ThereIsBlackKingCreated()
		{
			var factory = new PiecesFactory();
			var pieces = factory.Create();
			var filteredPieces = pieces
				.Where(p => p.PieceType == ChessPieceType.King &&
					p.Color == ChessColor.Black)
				.ToList();
			Assert.AreEqual(1, filteredPieces.Count);
		}
		[Test]
		public void ThereAre8BlackPawnsCreated()
		{
			var factory = new PiecesFactory();
			var pieces = factory.Create();
			var filteredPieces = pieces
				.Where(p => p.PieceType == ChessPieceType.Pawn &&
					p.Color == ChessColor.Black)
				.ToList();
			Assert.AreEqual(8, filteredPieces.Count);
		}
		[Test]
		public void ThereAre8WhitePawnsCreated()
		{
			var factory = new PiecesFactory();
			var pieces = factory.Create();
			var filteredPieces = pieces
				.Where(p => p.PieceType == ChessPieceType.Pawn &&
					p.Color == ChessColor.White)
				.ToList();
			Assert.AreEqual(8, filteredPieces.Count);
		}
	}
}