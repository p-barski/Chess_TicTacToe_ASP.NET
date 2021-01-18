using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;
using Microsoft.Extensions.Logging;
using Server.TicTacToe;
using Server.Sockets.Handlers;

namespace Server.Sockets
{
	public class SocketController
	{
		private readonly Collections collections;
		private readonly ILogger<SocketController> logger;
		private readonly MainHandler handler;
		public SocketController(ILogger<SocketController> logger,
			Collections collections, MainHandler handler)
		{
			this.collections = collections;
			this.logger = logger;
			this.handler = handler;
		}
		public async Task ReceiveAsync(WebSocket socket)
		{
			Player player = new Player(socket);
			collections.AddPlayer(player);
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