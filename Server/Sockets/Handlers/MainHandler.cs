using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Server.TicTacToe;
using Server.Sockets.Other;

namespace Server.Sockets.Handlers
{
	public class MainHandler
	{
		private readonly Collections collections;
		private readonly ILogger<MainHandler> logger;
		private readonly IMessageDeserializer deserializer;
		private readonly IEnumerable<IMessageHandler> handlers;
		public MainHandler(Collections collections, ILogger<MainHandler> logger,
			IMessageDeserializer deserializer, IEnumerable<IMessageHandler> handlers)
		{
			this.collections = collections;
			this.logger = logger;
			this.deserializer = deserializer;
			this.handlers = handlers;
		}
		public async Task HandleMessageAsync(Player player,
			WebSocketReceiveResult result, byte[] buffer)
		{
			if (result.MessageType == WebSocketMessageType.Close)
			{
				await player.Socket.CloseAsync(result.CloseStatus.Value,
					result.CloseStatusDescription, CancellationToken.None);
				return;
			}
			var message = deserializer.Deserialize(buffer, result.Count);
			foreach (var handler in handlers)
			{
				try
				{
					await handler.HandleMessageAsync(player, message);
					break;
				}
				catch (InvalidCastException) { }
			}
		}
	}
}