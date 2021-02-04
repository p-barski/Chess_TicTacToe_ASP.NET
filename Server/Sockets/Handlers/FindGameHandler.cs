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
		private readonly ILogger<FindGameHandler> logger;
		private readonly ICollections collections;
		private readonly IGameSessionFactory sessionFactory;
		private readonly IMessageSender messageSender;

		public FindGameHandler(ILogger<FindGameHandler> logger, ICollections collections,
			IGameSessionFactory sessionFactory, IMessageSender messageSender)
		{
			this.logger = logger;
			this.collections = collections;
			this.sessionFactory = sessionFactory;
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
				var session = sessionFactory.Create(player, opponent, castedMessage.Size);
				collections.AddSession(session);
				await messageSender.SendMessageAsync(player.Socket, new GameFoundMessage(true));
				await messageSender.SendMessageAsync(opponent.Socket, new GameFoundMessage(false));
			}
			catch (InvalidOperationException) { }
		}
	}
}