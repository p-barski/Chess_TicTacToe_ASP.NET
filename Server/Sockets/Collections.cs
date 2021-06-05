using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Server.Games;
using Server.Sockets.Other;
using Server.Sockets.Messages;
using Server.Database;

namespace Server.Sockets
{
	public class Collections : ICollections
	{
		private readonly ILogger<Collections> logger;
		private readonly IMessageSender messageSender;
		private readonly IChessSessionCanceler canceler;
		private readonly ConcurrentDictionary<IPlayer, byte> players
			= new ConcurrentDictionary<IPlayer, byte>();
		private readonly ConcurrentDictionary<IGameSession, byte> sessions
			= new ConcurrentDictionary<IGameSession, byte>();
		public Collections(ILogger<Collections> logger,
			IMessageSender messageSender, IChessSessionCanceler canceler)
		{
			this.logger = logger;
			this.messageSender = messageSender;
			this.canceler = canceler;
		}
		public void AddPlayer(IPlayer player)
		{
			players.TryAdd(player, 0);
		}
		public async Task RemovePlayer(IPlayer player)
		{
			logger.LogInformation($"Removing player: {player.GUID}");
			players.TryRemove(player, out byte _);
			if (player.GameSessionGUID == Guid.Empty)
				return;
			var session = FindSessionOfAPlayer(player);
			await canceler.TryCancel(session, player);
			var message = new SessionClosedMessage("Other player closed the game.");
			if (session.PlayerTwo == player)
				await messageSender.SendMessageAsync(session.PlayerOne.Socket, message);
			else
				await messageSender.SendMessageAsync(session.PlayerTwo.Socket, message);
			RemoveSession(session);
		}
		public void AddSession(IGameSession session)
		{
			sessions.TryAdd(session, 0);
			logger.LogInformation($"Session added: {session.GUID}");
		}
		public void RemoveSession(IGameSession session)
		{
			logger.LogInformation($"Removing session: {session.GUID}");
			sessions.TryRemove(session, out byte _);
			session.Close();
		}
		public IPlayer FindPlayerSearchingForGame(IPlayer excludedPlayer)
		{
			return players.First(player =>
				player.Key.State == PlayerState.SearchingForGame &&
				player.Key.GUID != excludedPlayer.GUID &&
				player.Key.ExpectedGame.Equals(excludedPlayer.ExpectedGame)).Key;
		}
		public IGameSession FindSessionOfAPlayer(IPlayer player)
		{
			return sessions.First(session => session.Key.GUID == player.GameSessionGUID).Key;
		}
	}
}