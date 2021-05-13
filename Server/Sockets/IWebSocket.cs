using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;

namespace Server.Sockets
{
	public interface IWebSocket : IDisposable
	{
		WebSocketState State { get; }
		Task CloseAsync(WebSocketCloseStatus closeStatus,
			string statusDescription, CancellationToken cancellationToken);
		Task<WebSocketReceiveResult> ReceiveAsync(
			ArraySegment<byte> buffer, CancellationToken cancellationToken);
		Task SendAsync(ArraySegment<byte> buffer,
			WebSocketMessageType messageType, bool endOfMessage,
			CancellationToken cancellationToken);
	}
}