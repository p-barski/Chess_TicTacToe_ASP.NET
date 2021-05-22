using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;
using Microsoft.Extensions.Logging;
using Server.Games;
using Server.Sockets.Handlers;
using Server.Database;

namespace Server.Sockets
{
	public class SocketController
	{
		private readonly ICollections collections;
		private readonly ILogger<SocketController> logger;
		private readonly ISocketMessageHandler handler;
		private readonly IPlayerDataDatabase databaseAccess;
		public SocketController(ILogger<SocketController> logger,
			ICollections collections, ISocketMessageHandler handler,
			IPlayerDataDatabase databaseAccess)
		{
			this.collections = collections;
			this.logger = logger;
			this.handler = handler;
			this.databaseAccess = databaseAccess;
		}
		public async Task ReceiveAsync(IWebSocket socket)
		{
			logger.LogInformation("Received new web socket connection.");
			var playerData = databaseAccess.PlayerDataForNotLoggedInPlayers;
			IPlayer player = new Player(socket, playerData);
			collections.AddPlayer(player);
			logger.LogInformation("added new player.");
			var buffer = new byte[1024 * 4];
			try
			{
				while (socket.State == WebSocketState.Open)
				{
					var result = await socket.ReceiveAsync(buffer, CancellationToken.None);
					await handler.HandleMessageAsync(player, result, buffer);
				}
			}
			catch (WebSocketException e)
			{
				logger.LogInformation(e.Message);
			}
			await collections.RemovePlayer(player);
		}
	}
}