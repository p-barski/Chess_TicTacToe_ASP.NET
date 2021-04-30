using System.Linq;
using System.Collections.Generic;
using Chess.Board;
using Chess.Pieces;
using Chess.Movement;

namespace Chess.Game
{
	public class ChessGame : IChessGame
	{
		public ChessColor CurrentPlayer { get; private set; }
		public IReadOnlyMovementHistory MovementHistory => chessBoard.History;
		public IEnumerable<IReadOnlyChessPiece> Pieces => chessBoard.Pieces;
		private readonly IChessBoard chessBoard;
		private readonly IMoveValidator moveValidator;
		private readonly IPromotionDetector promotionDetector;
		private readonly IGameFinishedDetector gameFinishedDetector;
		private readonly ILegalMovement legalMovement;
		private ChessPlayResult checkResult = ChessPlayResult.SuccessfulMove;
		public ChessGame(
			IChessBoard chessBoard,
			IMoveValidator moveValidator,
			IPromotionDetector promotionDetector,
			IGameFinishedDetector gameFinishedDetector,
			ILegalMovement legalMovement)
		{
			CurrentPlayer = ChessColor.White;
			this.chessBoard = chessBoard;
			this.moveValidator = moveValidator;
			this.promotionDetector = promotionDetector;
			this.gameFinishedDetector = gameFinishedDetector;
			this.legalMovement = legalMovement;
		}
		public ChessPlayResult Play(ChessMove chessMove, ChessColor player)
		{
			if (IsGameFinished())
			{
				return checkResult;
			}
			if (player != CurrentPlayer)
			{
				return ChessPlayResult.WrongPlayer;
			}
			if (!moveValidator.ValidateAndMove(chessMove, player))
			{
				return ChessPlayResult.InvalidMove;
			}

			if (promotionDetector.IsPromotionRequired())
			{
				return ChessPlayResult.PromotionRequired;
			}
			CurrentPlayer = CurrentPlayer.Opposite();

			checkResult = gameFinishedDetector.IsGameFinished(CurrentPlayer);
			return checkResult;
		}
		public IEnumerable<ChessMove> GetAvailableLegalMoves(ChessColor player)
		{
			return chessBoard.Pieces
				.Where(p => p.Color == player)
				.ToList()
				.SelectMany(p => legalMovement.GetAvailableLegalMoves(p));
		}
		private bool IsGameFinished()
		{
			if (checkResult == ChessPlayResult.BlackWin ||
				checkResult == ChessPlayResult.WhiteWin ||
				checkResult == ChessPlayResult.Stalemate)
			{
				return true;
			}
			return false;
		}
	}
}