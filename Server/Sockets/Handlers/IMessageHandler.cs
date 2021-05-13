using System.Threading.Tasks;
using Server.Games;
using Server.Sockets.Messages;

namespace Server.Sockets.Handlers
{
	public interface IMessageHandler
	{
		Task HandleMessageAsync(IPlayer player, IReceivedMessage message);
	}
}