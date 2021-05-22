using System.Collections.Generic;
using Chess.Game;
using Chess.Board;
using Chess.Pieces;
using Chess.Movement;

namespace Chess
{
	public class ChessGameFactory : IChessGameFactory
	{
		public IChessGame Create()
		{
			var piecesFactory = new PiecesFactory();
			var movementHistory = new MovementHistory();
			var piecePromoter = new PiecePromoter(movementHistory);
			var castlingMover = new CastlingMover(movementHistory);
			var enPassantMover = new EnPassantMover(movementHistory);
			var pieceMover = new PieceMover(movementHistory, piecePromoter,
				castlingMover, enPassantMover);
			var chessBoard = new ChessBoard(piecesFactory, pieceMover);

			List<IMovement> movements = new();
			var pawnMovement = new PawnMovement(chessBoard);
			var enPassantMovement = new EnPassantMovement(chessBoard);
			var kingMovement = new KingMovement(chessBoard);
			var horizontalMovement = new HorizontalMovement(chessBoard);
			var verticalMovement = new VerticalMovement(chessBoard);
			var pdiagonalMovement = new PositiveDiagonalMovement(chessBoard);
			var ndiagonalMovement = new NegativeDiagonalMovement(chessBoard);
			var knightMovement = new KnightMovement(chessBoard);
			movements.Add(pawnMovement);
			movements.Add(enPassantMovement);
			movements.Add(kingMovement);
			movements.Add(horizontalMovement);
			movements.Add(verticalMovement);
			movements.Add(pdiagonalMovement);
			movements.Add(ndiagonalMovement);
			movements.Add(knightMovement);
			var movementComposite = new MovementComposite(movements);

			List<IMovement> movementsWithCastling = new();
			var queensideCastlingMovement =
				new QueensideCastlingMovement(chessBoard, movementComposite);
			var kingsideCastlingMovement =
				new KingsideCastlingMovement(chessBoard, movementComposite);
			movementsWithCastling.Add(movementComposite);
			movementsWithCastling.Add(queensideCastlingMovement);
			movementsWithCastling.Add(kingsideCastlingMovement);
			var movementCompositeWithCastling = new MovementComposite(movementsWithCastling);

			var promotionDetector = new PromotionDetector(chessBoard);

			var checkDetector = new CheckDetector(chessBoard, movementCompositeWithCastling);

			var legalMovement = new LegalMovement(chessBoard,
				movementCompositeWithCastling, checkDetector);

			var moveValidator = new MoveValidator(chessBoard,
				legalMovement, promotionDetector);

			var gameFinishedDetector = new GameFinishedDetector(checkDetector,
				legalMovement);

			return new ChessGame(chessBoard, moveValidator,
				promotionDetector, gameFinishedDetector, legalMovement);
		}
	}
}