using System;
using System.Threading.Tasks;
using Server.Games;
using Server.Sockets.Other;
using Server.Sockets.Messages;
using Server.Database;

namespace Server.Sockets.Handlers
{
	public class RegistrationHandler : IMessageHandler
	{
		private readonly IPlayerDataDatabase databaseAccess;
		private readonly IPasswordHasher hasher;
		private readonly IMessageSender messageSender;
		public RegistrationHandler(IPlayerDataDatabase databaseAccess,
			IPasswordHasher hasher, IMessageSender messageSender)
		{
			this.databaseAccess = databaseAccess;
			this.hasher = hasher;
			this.messageSender = messageSender;
		}
		public async Task HandleMessageAsync(IPlayer player, IReceivedMessage message)
		{
			var authentication = (AuthenticationMessage)message;
			if (!authentication.Registration)
			{
				throw new InvalidCastException();
			}

			var hashedPassword = hasher.HashPassword(authentication.Password);
			var playerData = new PlayerData(authentication.Username, hashedPassword);

			if (await databaseAccess.SavePlayerDataAsync(playerData))
			{
				player.PlayerData = playerData;
				var successMsg = new AuthenticationResultMessage()
				{
					IsSuccess = true,
					ErrorMessage = ""
				};
				await messageSender.SendMessageAsync(player.Socket, successMsg);
				return;
			}
			var errorMsg = new AuthenticationResultMessage()
			{
				IsSuccess = false,
				ErrorMessage = "Username already taken"
			};
			await messageSender.SendMessageAsync(player.Socket, errorMsg);
		}
	}
}