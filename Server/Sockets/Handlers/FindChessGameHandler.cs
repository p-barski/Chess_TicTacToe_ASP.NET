using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Server.Games;
using Server.Games.Chess;
using Server.Sockets.Other;
using Server.Sockets.Messages;

namespace Server.Sockets.Handlers
{
	public class FindChessGameHandler : IMessageHandler
	{
		private readonly ILogger<FindChessGameHandler> logger;
		private readonly ICollections collections;
		private readonly IGameSessionFactory sessionFactory;
		private readonly IMessageSender messageSender;

		public FindChessGameHandler(ILogger<FindChessGameHandler> logger,
			ICollections collections, IGameSessionFactory sessionFactory,
			IMessageSender messageSender)
		{
			this.logger = logger;
			this.collections = collections;
			this.sessionFactory = sessionFactory;
			this.messageSender = messageSender;
		}
		public async Task HandleMessageAsync(IPlayer player, IReceivedMessage message)
		{
			var findChessGameMessage = (FindChessGameMessage)message;

			if (player.GameSessionGUID != Guid.Empty)
			{
				await collections.RemovePlayer(player);
				collections.AddPlayer(player);
			}

			var expectedGame = new ExpectedChess();
			player.SetAsSearchingForGame(expectedGame);
			try
			{
				var opponent = collections.FindPlayerSearchingForGame(player);
				ChessGameSession session = (ChessGameSession)sessionFactory
					.Create(player, opponent, expectedGame);
				collections.AddSession(session);
				logger.LogInformation("Created new chess game session.");

				await messageSender.SendMessageAsync(player.Socket, new GameFoundMessage(true));
				await messageSender.SendMessageAsync(opponent.Socket, new GameFoundMessage(false));

				var piecesAndMovesMessage = new ChessPiecesAndMovesMessage()
				{
					AvailableMoves = session.GetAvailableMoves(),
					Pieces = session.GetAvailablePieces()
				};

				await messageSender.SendMessageAsync(player.Socket, piecesAndMovesMessage);
				await messageSender.SendMessageAsync(opponent.Socket, piecesAndMovesMessage);
			}
			catch (InvalidOperationException) { }
		}
	}
}