using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;

namespace Server.Sockets
{
	public class WebSocketWrapper : IWebSocket
	{
		private readonly WebSocket socket;
		public string CloseStatusDescription { get => socket.CloseStatusDescription; }
		public WebSocketCloseStatus? CloseStatus { get => socket.CloseStatus; }
		public WebSocketState State { get => socket.State; }
		public string SubProtocol { get => socket.SubProtocol; }
		public WebSocketWrapper(WebSocket socket)
		{
			this.socket = socket;
		}
		public void Abort()
		{
			socket.Abort();
		}
		public void Dispose()
		{
			socket.Dispose();
		}
		public async Task CloseAsync(WebSocketCloseStatus closeStatus,
			string statusDescription, CancellationToken cancellationToken)
		{
			await socket.CloseAsync(closeStatus, statusDescription, cancellationToken);
		}
		public async Task CloseOutputAsync(WebSocketCloseStatus closeStatus,
			string statusDescription, CancellationToken cancellationToken)
		{
			await socket.CloseOutputAsync(closeStatus, statusDescription, cancellationToken);
		}
		public async Task<WebSocketReceiveResult> ReceiveAsync(
			ArraySegment<byte> buffer, CancellationToken cancellationToken)
		{
			return await socket.ReceiveAsync(buffer, cancellationToken);
		}
		public async Task SendAsync(ArraySegment<byte> buffer,
			WebSocketMessageType messageType, bool endOfMessage,
			CancellationToken cancellationToken)
		{
			await socket.SendAsync(buffer, messageType, endOfMessage, cancellationToken);
		}
	}
}