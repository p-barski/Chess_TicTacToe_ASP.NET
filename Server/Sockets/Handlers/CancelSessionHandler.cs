using System.Threading.Tasks;
using Server.Games;
using Server.Sockets.Other;
using Server.Sockets.Messages;

namespace Server.Sockets.Handlers
{
	public class CancelSessionHandler : IMessageHandler
	{
		private readonly ICollections collections;
		private readonly IMessageSender messageSender;
		public CancelSessionHandler(ICollections collections, IMessageSender messageSender)
		{
			this.collections = collections;
			this.messageSender = messageSender;
		}
		public async Task HandleMessageAsync(IPlayer player, IReceivedMessage message)
		{
			var castedMessage = (CancelSessionMessage)message;
			await collections.RemovePlayer(player);
			collections.AddPlayer(player);
		}
	}
}