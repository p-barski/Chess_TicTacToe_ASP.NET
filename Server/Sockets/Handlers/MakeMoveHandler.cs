using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Server.Games;
using Server.Games.TicTacToe;
using Server.Sockets.Other;
using Server.Sockets.Messages;

namespace Server.Sockets.Handlers
{
	public class MakeMoveHandler : IMessageHandler
	{
		private readonly ICollections collections;
		private readonly ILogger<MakeMoveHandler> logger;
		private readonly IMessageSender messageSender;

		public MakeMoveHandler(ICollections collections, ILogger<MakeMoveHandler> logger,
			IMessageSender messageSender)
		{
			this.collections = collections;
			this.logger = logger;
			this.messageSender = messageSender;
		}
		public async Task HandleMessageAsync(IPlayer player, IReceivedMessage msg)
		{
			var castedMsg = (MakeMoveMessage)msg;
			logger.LogInformation($"MakeMoveMessage: X:{castedMsg.X}, Y:{castedMsg.Y}");

			IGameSession session;
			try
			{
				session = collections.FindSessionOfAPlayer(player);
			}
			catch (InvalidOperationException)
			{
				var msgText = "This player is not connected to any game session.";
				await messageSender.SendMessageAsync(player.Socket,
					new InvalidStateMessage(msgText));
				return;
			}

			var result = session.Play(player, new TicTacToeMove(castedMsg.X, castedMsg.Y));
			switch (result)
			{
				case PlayResult.Success:
					await SendToBothAsync(session, result, castedMsg.X, castedMsg.Y);
					break;
				case PlayResult.Draw:
					await SendToBothAsync(session, result, castedMsg.X, castedMsg.Y);
					collections.RemoveSession(session);
					break;
				case PlayResult.YouWin:
					await SendGameFinishedAsync(session, player, castedMsg.X, castedMsg.Y);
					collections.RemoveSession(session);
					break;
				default:
					await SendToSenderAsync(player, result);
					break;
			}
		}
		private async Task SendToBothAsync(IGameSession session, PlayResult result, int x, int y)
		{
			var msgToSend = new MoveResultMessage(result.ToString(), x, y);
			await messageSender.SendMessageAsync(session.PlayerOne.Socket, msgToSend);
			await messageSender.SendMessageAsync(session.PlayerTwo.Socket, msgToSend);
		}
		private async Task SendToSenderAsync(IPlayer player, PlayResult result)
		{
			var msgToSend = new MoveResultMessage(result.ToString(), 0, 0);
			await messageSender.SendMessageAsync(player.Socket, msgToSend);
		}
		private async Task SendGameFinishedAsync(IGameSession session, IPlayer winner, int x, int y)
		{
			IPlayer loser;
			if (session.PlayerTwo == winner)
				loser = session.PlayerOne;
			else
				loser = session.PlayerTwo;

			var msgToWinner = new MoveResultMessage(PlayResult.YouWin.ToString(), x, y);
			var msgToLoser = new MoveResultMessage(PlayResult.YouLose.ToString(), x, y);
			await messageSender.SendMessageAsync(winner.Socket, msgToWinner);
			await messageSender.SendMessageAsync(loser.Socket, msgToLoser);
		}
	}
}