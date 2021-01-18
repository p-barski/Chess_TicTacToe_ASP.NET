using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Server.TicTacToe;
using Server.Sockets.Other;
using Server.Sockets.Messages;

namespace Server.Sockets
{
	public class Collections
	{
		private readonly ILogger<Collections> logger;
		private readonly IMessageSender messageSender;
		private readonly ConcurrentDictionary<Player, byte> players
			= new ConcurrentDictionary<Player, byte>();
		private readonly ConcurrentDictionary<GameSession, byte> sessions
			= new ConcurrentDictionary<GameSession, byte>();
		public Collections(ILogger<Collections> logger,
			IMessageSender messageSender)
		{
			this.logger = logger;
			this.messageSender = messageSender;
		}
		public void AddPlayer(Player player)
		{
			players.TryAdd(player, 0);
		}
		public async Task RemovePlayer(Player player)
		{
			logger.LogInformation($"Removing player: {player.GUID}");
			players.TryRemove(player, out byte _);
			if (player.GameSessionGUID == Guid.Empty)
				return;
			var session = FindSessionOfAPlayer(player);
			var message = new SessionClosedMessage("Other player closed the game.");
			if (session.PlayerO == player)
				await messageSender.SendMessageAsync(session.PlayerX.Socket, message);
			else
				await messageSender.SendMessageAsync(session.PlayerO.Socket, message);
			RemoveSession(session);
		}
		public void AddSession(Player first, Player second, int size)
		{
			try
			{
				var session = new GameSession(first, second, size);
				sessions.TryAdd(session, 0);
				logger.LogInformation($"Session added: {session.GUID}");
			}
			catch (InvalidOperationException) { }
		}
		public void RemoveSession(GameSession session)
		{
			logger.LogInformation($"Removing session: {session.GUID}");
			sessions.TryRemove(session, out byte _);
			session.Close();
		}
		public Player FindPlayerSearchingForGame(Player excludedPlayer)
		{
			return players.First(player =>
				player.Key.State == PlayerState.SearchingForGame &&
				player.Key.GUID != excludedPlayer.GUID &&
				player.Key.ExpectedBoardSize == excludedPlayer.ExpectedBoardSize).Key;
		}
		public GameSession FindSessionOfAPlayer(Player player)
		{
			return sessions.First(session => session.Key.GUID == player.GameSessionGUID).Key;
		}
	}
}