using System.Collections.Generic;

namespace Chess.Pieces
{
	public class PiecesFactory : IPiecesFactory
	{
		private const int boardSize = 8;
		private const int blackYPosition = 7;
		private const int whiteYPosition = 0;
		public IEnumerable<IChessPiece> Create()
		{
			HashSet<IChessPiece> pieces = new();

			CreateWhitePawns(pieces);
			CreateBlackPawns(pieces);
			CreateRooks(pieces);
			CreateKnights(pieces);
			CreateBishops(pieces);
			CreateQueens(pieces);
			CreateKings(pieces);

			return pieces;
		}
		private void CreateWhitePawns(HashSet<IChessPiece> pieces)
		{
			const int whitePawnYPosition = 1;
			for (int x = 0; x < boardSize; x++)
			{
				var pawn = new ChessPiece(ChessPieceType.Pawn,
					ChessColor.White, new Position(x, whitePawnYPosition));
				pieces.Add(pawn);
			}
		}
		private void CreateBlackPawns(HashSet<IChessPiece> pieces)
		{
			const int blackPawnYPosition = 6;
			for (int x = 0; x < boardSize; x++)
			{
				var pawn = new ChessPiece(ChessPieceType.Pawn,
					ChessColor.Black, new Position(x, blackPawnYPosition));
				pieces.Add(pawn);
			}
		}
		private void CreateRooks(HashSet<IChessPiece> pieces)
		{
			const int leftRookXPosition = 0;
			const int rightRookXPosition = 7;

			var leftBlackRook = new ChessPiece(ChessPieceType.Rook,
				ChessColor.Black, new Position(leftRookXPosition, blackYPosition));
			var rightBlackRook = new ChessPiece(ChessPieceType.Rook,
				ChessColor.Black, new Position(rightRookXPosition, blackYPosition));

			var leftWhiteRook = new ChessPiece(ChessPieceType.Rook,
				ChessColor.White, new Position(leftRookXPosition, whiteYPosition));
			var rightWhiteRook = new ChessPiece(ChessPieceType.Rook,
				ChessColor.White, new Position(rightRookXPosition, whiteYPosition));

			pieces.Add(leftBlackRook);
			pieces.Add(rightBlackRook);
			pieces.Add(leftWhiteRook);
			pieces.Add(rightWhiteRook);
		}
		private void CreateKnights(HashSet<IChessPiece> pieces)
		{
			const int leftKnightXPosition = 1;
			const int rightKnightXPosition = 6;

			var leftBlackKnight = new ChessPiece(ChessPieceType.Knight,
				ChessColor.Black, new Position(leftKnightXPosition, blackYPosition));
			var rightBlackKnight = new ChessPiece(ChessPieceType.Knight,
				ChessColor.Black, new Position(rightKnightXPosition, blackYPosition));

			var leftWhiteKnight = new ChessPiece(ChessPieceType.Knight,
				ChessColor.White, new Position(leftKnightXPosition, whiteYPosition));
			var rightWhiteKnight = new ChessPiece(ChessPieceType.Knight,
				ChessColor.White, new Position(rightKnightXPosition, whiteYPosition));

			pieces.Add(leftBlackKnight);
			pieces.Add(rightBlackKnight);
			pieces.Add(leftWhiteKnight);
			pieces.Add(rightWhiteKnight);
		}
		private void CreateBishops(HashSet<IChessPiece> pieces)
		{
			const int leftBishopXPosition = 2;
			const int rightBishopXPosition = 5;

			var leftBlackBishop = new ChessPiece(ChessPieceType.Bishop,
				ChessColor.Black, new Position(leftBishopXPosition, blackYPosition));
			var rightBlackBishop = new ChessPiece(ChessPieceType.Bishop,
				ChessColor.Black, new Position(rightBishopXPosition, blackYPosition));

			var leftWhiteBishop = new ChessPiece(ChessPieceType.Bishop,
				ChessColor.White, new Position(leftBishopXPosition, whiteYPosition));
			var rightWhiteBishop = new ChessPiece(ChessPieceType.Bishop,
				ChessColor.White, new Position(rightBishopXPosition, whiteYPosition));

			pieces.Add(leftBlackBishop);
			pieces.Add(rightBlackBishop);
			pieces.Add(leftWhiteBishop);
			pieces.Add(rightWhiteBishop);
		}
		private void CreateQueens(HashSet<IChessPiece> pieces)
		{
			const int queenXPosition = 3;

			var blackQueen = new ChessPiece(ChessPieceType.Queen,
				ChessColor.Black, new Position(queenXPosition, blackYPosition));

			var whiteQueen = new ChessPiece(ChessPieceType.Queen,
				ChessColor.White, new Position(queenXPosition, whiteYPosition));

			pieces.Add(blackQueen);
			pieces.Add(whiteQueen);
		}
		private void CreateKings(HashSet<IChessPiece> pieces)
		{
			const int kingXPosition = 4;

			var blackKing = new ChessPiece(ChessPieceType.King,
				ChessColor.Black, new Position(kingXPosition, blackYPosition));

			var whiteKing = new ChessPiece(ChessPieceType.King,
				ChessColor.White, new Position(kingXPosition, whiteYPosition));

			pieces.Add(blackKing);
			pieces.Add(whiteKing);
		}
	}
}