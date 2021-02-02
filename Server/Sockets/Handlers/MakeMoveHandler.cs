using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Server.TicTacToe;
using Server.Sockets.Other;
using Server.Sockets.Messages;

namespace Server.Sockets.Handlers
{
	public class MakeMoveHandler : IMessageHandler
	{
		private readonly Collections collections;
		private readonly ILogger<MakeMoveHandler> logger;
		private readonly IMessageSender messageSender;

		public MakeMoveHandler(Collections collections, ILogger<MakeMoveHandler> logger,
			IMessageSender messageSender)
		{
			this.collections = collections;
			this.logger = logger;
			this.messageSender = messageSender;
		}
		public async Task HandleMessageAsync(Player player, IReceivedMessage msg)
		{
			var castedMsg = (MakeMoveMessage)msg;
			logger.LogInformation($"MakeMoveMessage: X:{castedMsg.X}, Y:{castedMsg.Y}");

			GameSession session;
			try
			{
				session = collections.FindSessionOfAPlayer(player);
			}
			catch (InvalidOperationException) { return; }

			var result = session.Play(player, castedMsg.X, castedMsg.Y);
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
		private async Task SendToBothAsync(GameSession session, PlayResult result, int x, int y)
		{
			var msgToSend = new MoveResultMessage(result.ToString(), x, y);
			await messageSender.SendMessageAsync(session.PlayerX.Socket, msgToSend);
			await messageSender.SendMessageAsync(session.PlayerO.Socket, msgToSend);
		}
		private async Task SendToSenderAsync(Player player, PlayResult result)
		{
			var msgToSend = new MoveResultMessage(result.ToString(), 0, 0);
			await messageSender.SendMessageAsync(player.Socket, msgToSend);
		}
		private async Task SendGameFinishedAsync(GameSession session, Player winner, int x, int y)
		{
			Player loser;
			if (session.PlayerO == winner)
				loser = session.PlayerX;
			else
				loser = session.PlayerO;

			var msgToWinner = new MoveResultMessage(PlayResult.YouWin.ToString(), x, y);
			var msgToLoser = new MoveResultMessage(PlayResult.YouLose.ToString(), x, y);
			await messageSender.SendMessageAsync(winner.Socket, msgToWinner);
			await messageSender.SendMessageAsync(loser.Socket, msgToLoser);
		}
	}
}