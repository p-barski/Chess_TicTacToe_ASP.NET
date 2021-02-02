using System.Threading.Tasks;
using Server.TicTacToe;
using Server.Sockets.Messages;

namespace Server.Sockets.Handlers
{
	public interface IMessageHandler
	{
		Task HandleMessageAsync(IPlayer player, IReceivedMessage message);
	}
}