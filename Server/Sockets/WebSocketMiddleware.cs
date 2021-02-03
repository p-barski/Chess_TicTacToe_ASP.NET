using System.Threading.Tasks;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Server.Sockets
{
	public class WebSocketMiddleware
	{
		private readonly RequestDelegate next;
		private readonly SocketController controller;
		private readonly ILogger<WebSocketMiddleware> logger;
		public WebSocketMiddleware(RequestDelegate next,
			SocketController controller, ILogger<WebSocketMiddleware> logger)
		{
			this.next = next;
			this.controller = controller;
			this.logger = logger;
		}
		public async Task InvokeAsync(HttpContext context)
		{
			if (!context.WebSockets.IsWebSocketRequest)
			{
				await next(context);
				return;
			}
			WebSocket socket = await context.WebSockets.AcceptWebSocketAsync();
			logger.LogInformation("Websocket connected");
			await controller.ReceiveAsync(new WebSocketWrapper(socket));
		}
	}
}