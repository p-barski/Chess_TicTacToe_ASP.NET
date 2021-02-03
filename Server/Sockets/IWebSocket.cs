using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;

namespace Server.Sockets
{
	public interface IWebSocket : IDisposable
	{
		string CloseStatusDescription { get; }
		WebSocketCloseStatus? CloseStatus { get; }
		WebSocketState State { get; }
		string SubProtocol { get; }
		void Abort();
		Task CloseAsync(WebSocketCloseStatus closeStatus,
			string statusDescription, CancellationToken cancellationToken);
		Task CloseOutputAsync(WebSocketCloseStatus closeStatus,
			string statusDescription, CancellationToken cancellationToken);
		Task<WebSocketReceiveResult> ReceiveAsync(
			ArraySegment<byte> buffer, CancellationToken cancellationToken);
		Task SendAsync(ArraySegment<byte> buffer,
			WebSocketMessageType messageType, bool endOfMessage,
			CancellationToken cancellationToken);
	}
}