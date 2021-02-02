using System.Net.WebSockets;
using System.Threading.Tasks;
using Server.Sockets.Messages;

namespace Server.Sockets.Other
{
	public interface IMessageSender
	{
		Task SendMessageAsync(WebSocket socket, ISendMessage message);
	}
}