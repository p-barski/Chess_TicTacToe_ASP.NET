using System;
using System.Threading.Tasks;
using Server.Games;
using Server.Sockets.Other;
using Server.Sockets.Messages;
using Server.Database;

namespace Server.Sockets.Handlers
{
	public class LoginHandler : IMessageHandler
	{
		private readonly IPlayerDataDatabase databaseAccess;
		private readonly IPasswordHasher hasher;
		private readonly IMessageSender messageSender;
		public LoginHandler(IPlayerDataDatabase databaseAccess,
			IPasswordHasher hasher, IMessageSender messageSender)
		{
			this.databaseAccess = databaseAccess;
			this.hasher = hasher;
			this.messageSender = messageSender;
		}
		public async Task HandleMessageAsync(IPlayer player, IReceivedMessage message)
		{
			var authentication = (AuthenticationMessage)message;
			if (authentication.Registration)
			{
				throw new InvalidCastException();
			}
			var playerData = databaseAccess.GetPlayerData(authentication.Username);

			if (playerData == null)
			{
				await HandleIncorrectPassword(player);
				return;
			}
			if (!hasher.IsPasswordCorrect(authentication.Password, playerData.Password))
			{
				await HandleIncorrectPassword(player);
				return;
			}

			player.PlayerData = playerData;
			var successMsg = new AuthenticationResultMessage()
			{
				IsSuccess = true,
				ErrorMessage = ""
			};
			await messageSender.SendMessageAsync(player.Socket, successMsg);
		}
		private async Task HandleIncorrectPassword(IPlayer player)
		{
			var errorMsg = new AuthenticationResultMessage()
			{
				IsSuccess = false,
				ErrorMessage = "Wrong username and/or password."
			};
			await messageSender.SendMessageAsync(player.Socket, errorMsg);
		}
	}
}