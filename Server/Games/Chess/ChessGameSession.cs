using System;
using System.Linq;
using System.Collections.Generic;
using Chess;
using Chess.Game;
using Chess.Movement;
using Chess.Pieces;

namespace Server.Games.Chess
{
	public class ChessGameSession : IGameSession
	{
		public Guid GUID { get; } = Guid.NewGuid();
		public IPlayer PlayerOne { get; }
		public IPlayer PlayerTwo { get; }
		public DateTime StartDate { get; } = DateTime.UtcNow;
		public IReadOnlyMovementHistory MovementHistory { get => game.MovementHistory; }
		private IChessGame game;
		public ChessGameSession(IPlayer playerOne, IPlayer playerTwo,
			IChessGameFactory chessGameFactory)
		{
			lock (playerOne)
			{
				lock (playerTwo)
				{
					if (playerOne.GameSessionGUID != Guid.Empty || playerTwo.GameSessionGUID != Guid.Empty)
						throw new InvalidOperationException("Player is already assigned to a game session.");

					PlayerOne = playerOne;
					PlayerTwo = playerTwo;
					PlayerOne.AddToGame(GUID, new PlayerType<ChessColor>(ChessColor.White));
					PlayerTwo.AddToGame(GUID, new PlayerType<ChessColor>(ChessColor.Black));
					game = chessGameFactory.Create();
				}
			}
		}
		public PlayResult Play(IPlayer from, IGameMove gameMove)
		{
			var playerColor = from.PlayerType.StringRepresentation.ToEnum<ChessColor>();
			if (gameMove is PromotionMove)
			{
				PromotionMove move = (PromotionMove)gameMove;
				var chessMove = new ChessMove(
					new Position(0, 0), new Position(0, 0),
					pawnPromotion: move.PromotionPiece.ToEnum<ChessPieceType>());
				return TurnChessResultIntoPlayResult(game.Play(chessMove, playerColor));
			}

			if (gameMove is ChessGameMove)
			{
				ChessGameMove move = (ChessGameMove)gameMove;
				ChessMove chessMove = new ChessMove(
					new Position(move.X_StartPosition, move.Y_StartPosition),
					new Position(move.X_FinishedPosition, move.Y_FinishedPosition));

				return TurnChessResultIntoPlayResult(game.Play(chessMove, playerColor));
			}
			return PlayResult.Error;
		}
		public void Close()
		{
			PlayerOne.RemoveFromGame();
			PlayerTwo.RemoveFromGame();
		}
		public List<ChessMoveWrapper> GetAvailableMoves()
		{
			return game
				.GetAvailableLegalMoves(ChessColor.White)
				.Union(game.GetAvailableLegalMoves(ChessColor.Black))
				.Select(m => new ChessMoveWrapper(m))
				.ToList();
		}
		public List<ChessPieceWrapper> GetAvailablePieces()
		{
			return game.Pieces
				.Select(p => new ChessPieceWrapper(p))
				.ToList();
		}
		private PlayResult TurnChessResultIntoPlayResult(ChessPlayResult result)
		{
			switch (result)
			{
				case ChessPlayResult.SuccessfulMove:
					return PlayResult.Success;
				case ChessPlayResult.PromotionRequired:
					return PlayResult.PromotionRequired;
				case ChessPlayResult.Stalemate:
					return PlayResult.Draw;
				case ChessPlayResult.WhiteWin:
				case ChessPlayResult.BlackWin:
					return PlayResult.YouWin;
				case ChessPlayResult.BlackChecked:
				case ChessPlayResult.WhiteChecked:
					return PlayResult.Check;
				case ChessPlayResult.WrongPlayer:
					return PlayResult.NotYourTurn;
				default:
					return PlayResult.Error;
			}
		}
	}
}