using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Server.TicTacToe;
using Server.Sockets.Other;
using Server.Sockets.Messages;

namespace Server.Sockets.Handlers
{
	public class FindGameHandler : IMessageHandler
	{
		private readonly ICollections collections;
		private readonly ILogger<FindGameHandler> logger;
		private readonly IMessageSender messageSender;

		public FindGameHandler(ICollections collections, ILogger<FindGameHandler> logger,
			IMessageDeserializer deserializer, IMessageSender messageSender)
		{
			this.collections = collections;
			this.logger = logger;
			this.messageSender = messageSender;
		}
		public async Task HandleMessageAsync(IPlayer player, IReceivedMessage message)
		{
			var castedMessage = (FindGameMessage)message;
			logger.LogInformation(castedMessage.Size.ToString());
			player.SetAsSearchingForGame(castedMessage.Size);
			try
			{
				var opponent = collections.FindPlayerSearchingForGame(player);
				var session = new GameSession(player, opponent, castedMessage.Size);
				collections.AddSession(session);
				await messageSender.SendMessageAsync(player.Socket, new GameFoundMessage(true));
				await messageSender.SendMessageAsync(opponent.Socket, new GameFoundMessage(false));
			}
			catch (InvalidOperationException) { }
		}
	}
}