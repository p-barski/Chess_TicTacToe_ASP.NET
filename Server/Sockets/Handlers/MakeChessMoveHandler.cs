using System;
using System.Linq;
using System.Threading.Tasks;
using Server.Games;
using Server.Games.Chess;
using Server.Sockets.Other;
using Server.Sockets.Messages;
using Server.Database;
using Server.Database.Chess;
using Chess.Movement;

namespace Server.Sockets.Handlers
{
	public class MakeChessMoveHandler : IMessageHandler
	{
		private readonly ICollections collections;
		private readonly IMessageSender messageSender;
		private readonly IChessMoveConverter converter;
		private readonly IChessDatabase databaseAccess;
		public MakeChessMoveHandler(ICollections collections, IMessageSender messageSender,
			IChessMoveConverter converter, IChessDatabase databaseAccess)
		{
			this.collections = collections;
			this.messageSender = messageSender;
			this.converter = converter;
			this.databaseAccess = databaseAccess;
		}
		public async Task HandleMessageAsync(IPlayer player, IReceivedMessage msg)
		{
			IGameMove chessGameMove = null;
			if (msg is MakeChessMoveMessage)
			{
				var chessMoveMsg = (MakeChessMoveMessage)msg;
				chessGameMove = new ChessGameMove(
					chessMoveMsg.X_StartPosition, chessMoveMsg.Y_StartPosition,
					chessMoveMsg.X_FinishedPosition, chessMoveMsg.Y_FinishedPosition);
			}
			else
			{
				var chessMoveMsg = (PawnPromotionMessage)msg;
				chessGameMove = new PromotionMove(chessMoveMsg.PromotionPiece);
			}

			ChessGameSession session;
			try
			{
				session = (ChessGameSession)collections.FindSessionOfAPlayer(player);
			}
			catch (InvalidOperationException)
			{
				var msgText = "This player is not connected to any game session.";
				await messageSender.SendMessageAsync(player.Socket,
					new InvalidStateMessage(msgText));
				return;
			}

			var result = session.Play(player, chessGameMove);
			switch (result)
			{
				case PlayResult.Success:
				case PlayResult.Check:
					await SaveChessMoveToDatabase(session);
					await SendToBothAsync(session, player, result);
					break;
				case PlayResult.Draw:
					await SaveChessMoveToDatabase(session);
					await SendStalemateAsync(session);
					collections.RemoveSession(session);
					break;
				case PlayResult.YouWin:
					await SaveChessMoveToDatabase(session);
					await SendGameFinishedAsync(session, player);
					collections.RemoveSession(session);
					break;
				case PlayResult.PromotionRequired:
					await SaveChessMoveToDatabase(session);
					await SendPromotionAsync(player);
					break;
				default:
					await SendErrorMsgToSenderAsync(player, result);
					break;
			}
		}
		private async Task SendToBothAsync(ChessGameSession session, IPlayer sender,
			PlayResult result)
		{
			var otherPlayer = GetOtherPlayer(session, sender);
			var msgToSend = new ChessPlayResultMessage()
			{
				Message = result.ToString()
			};

			await messageSender.SendMessageAsync(sender.Socket, msgToSend);
			msgToSend.IsClientTurn = true;
			await messageSender.SendMessageAsync(otherPlayer.Socket, msgToSend);

			await SendChessPiecesAndMovesMessageAsync(session);
		}
		private async Task SendErrorMsgToSenderAsync(IPlayer player, PlayResult result)
		{
			var msgToSend = new ChessPlayResultMessage() { Message = result.ToString() };
			await messageSender.SendMessageAsync(player.Socket, msgToSend);
		}
		private async Task SendGameFinishedAsync(ChessGameSession session,
			IPlayer winner)
		{
			var loser = GetOtherPlayer(session, winner);

			var msgToWinner = new ChessPlayResultMessage()
			{
				Message = PlayResult.YouWin.ToString()
			};
			var msgToLoser = new ChessPlayResultMessage()
			{
				Message = PlayResult.YouLose.ToString()
			};

			await messageSender.SendMessageAsync(winner.Socket, msgToWinner);
			await messageSender.SendMessageAsync(loser.Socket, msgToLoser);
			await SendChessPiecesAndMovesMessageAsync(session);

			if (IsWhitePlayer(session, winner))
			{
				await FinalizeChessGameSession(session, "WhiteWin");
				return;
			}
			await FinalizeChessGameSession(session, "BlackWin");
		}
		private async Task SendStalemateAsync(ChessGameSession session)
		{
			var stalemateMsg = new ChessPlayResultMessage()
			{
				Message = PlayResult.Draw.ToString()
			};
			await FinalizeChessGameSession(session, "Stalemate");

			await messageSender.SendMessageAsync(session.PlayerOne.Socket, stalemateMsg);
			await messageSender.SendMessageAsync(session.PlayerTwo.Socket, stalemateMsg);
			await SendChessPiecesAndMovesMessageAsync(session);
		}
		private async Task SendPromotionAsync(IPlayer player)
		{
			var promotionMsg = new ChessPlayResultMessage()
			{
				Message = PlayResult.PromotionRequired.ToString(),
				IsClientTurn = true,
				IsPromotionRequired = true
			};

			await messageSender.SendMessageAsync(player.Socket, promotionMsg);
		}
		private async Task SendChessPiecesAndMovesMessageAsync(ChessGameSession session)
		{
			var movesAndPiecesMessage = new ChessPiecesAndMovesMessage()
			{
				AvailableMoves = session.GetAvailableMoves(),
				Pieces = session.GetAvailablePieces()
			};

			await messageSender.SendMessageAsync(session.PlayerOne.Socket, movesAndPiecesMessage);
			await messageSender.SendMessageAsync(session.PlayerTwo.Socket, movesAndPiecesMessage);
		}
		private IPlayer GetOtherPlayer(IGameSession session, IPlayer player)
		{
			if (session.PlayerOne == player)
			{
				return session.PlayerTwo;
			}
			return session.PlayerOne;
		}
		private bool IsWhitePlayer(IGameSession session, IPlayer player)
		{
			return session.PlayerOne == player;
		}
		private async Task SaveChessMoveToDatabase(ChessGameSession session)
		{
			var gameDb = await databaseAccess.GetSavedGame(session.PlayerOne.PlayerData,
				session.PlayerTwo.PlayerData);
			if (gameDb == null)
			{
				return;
			}
			gameDb.ChessMoves.Add(converter.ConvertToDb(session.MovementHistory.ChessMoves[^1]));
			await databaseAccess.UpdateGameAsync(gameDb);
		}
		private async Task FinalizeChessGameSession(ChessGameSession session, string result)
		{
			var gameDb = await databaseAccess.GetSavedGame(session.PlayerOne.PlayerData,
				session.PlayerTwo.PlayerData);
			if (gameDb == null)
			{
				return;
			}
			gameDb.Result = result;
			gameDb.FinishDate = DateTime.UtcNow;
			await databaseAccess.UpdateGameAsync(gameDb);
		}
	}
}