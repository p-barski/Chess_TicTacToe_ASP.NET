using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Server.Games;
using Server.Database;
using Server.Sockets;
using Server.Sockets.Other;
using Server.Sockets.Handlers;
using Server.Sockets.Messages;

namespace ServerTests
{
	public class LoginHandlerTests
	{
		[Test]
		public async Task IfThereIsNoSuchUserSendBackErrorMessage()
		{
			var databaseMock = new Mock<IPlayerDataDatabase>(MockBehavior.Strict);
			var hasherMock = new Mock<IPasswordHasher>(MockBehavior.Strict);
			var msgSenderMock = new Mock<IMessageSender>(MockBehavior.Strict);
			var playerMock = new Mock<IPlayer>(MockBehavior.Strict);
			var socketMock = new Mock<IWebSocket>(MockBehavior.Strict);

			var username = "namename";
			var password = "qwerty";

			playerMock
				.SetupGet(p => p.Socket)
				.Returns(socketMock.Object);

			databaseMock
				.Setup(d => d.GetPlayerData(username))
				.Returns<PlayerData>(null);

			msgSenderMock
				.Setup(s => s.SendMessageAsync(socketMock.Object,
					It.IsAny<AuthenticationResultMessage>()))
				.Returns(Task.CompletedTask);

			var msg = new AuthenticationMessage()
			{
				Registration = false,
				Username = username,
				Password = password
			};

			var handler = new LoginHandler(databaseMock.Object,
				hasherMock.Object, msgSenderMock.Object);

			await handler.HandleMessageAsync(playerMock.Object, msg);

			msgSenderMock
				.Verify(s => s.SendMessageAsync(socketMock.Object,
					It.Is<AuthenticationResultMessage>(
						m => m.IsSuccess == false && m.ErrorMessage != "")));
		}
		[Test]
		public async Task IfPasswordIsIncorrectSendBackErrorMessage()
		{
			var databaseMock = new Mock<IPlayerDataDatabase>(MockBehavior.Strict);
			var hasherMock = new Mock<IPasswordHasher>(MockBehavior.Strict);
			var msgSenderMock = new Mock<IMessageSender>(MockBehavior.Strict);
			var playerMock = new Mock<IPlayer>(MockBehavior.Strict);
			var socketMock = new Mock<IWebSocket>(MockBehavior.Strict);

			var username = "test1234";
			var password = "testest";
			var hashedPassword = "!@sdfds;546412@#$#%#%$!@fsf#$!";
			var playerData = new PlayerData(username, hashedPassword);

			playerMock
				.SetupGet(p => p.Socket)
				.Returns(socketMock.Object);

			hasherMock
				.Setup(h => h.IsPasswordCorrect(password, hashedPassword))
				.Returns(false);

			databaseMock
				.Setup(d => d.GetPlayerData(username))
				.Returns(playerData);

			msgSenderMock
				.Setup(s => s.SendMessageAsync(socketMock.Object,
					It.IsAny<AuthenticationResultMessage>()))
				.Returns(Task.CompletedTask);

			var msg = new AuthenticationMessage()
			{
				Registration = false,
				Username = username,
				Password = password
			};

			var handler = new LoginHandler(databaseMock.Object,
				hasherMock.Object, msgSenderMock.Object);

			await handler.HandleMessageAsync(playerMock.Object, msg);

			msgSenderMock
				.Verify(s => s.SendMessageAsync(socketMock.Object,
					It.Is<AuthenticationResultMessage>(
						m => m.IsSuccess == false && m.ErrorMessage != "")));
		}
		[Test]
		public async Task IfLoginWasSuccessfulReturnSuccessMessage()
		{
			var databaseMock = new Mock<IPlayerDataDatabase>(MockBehavior.Strict);
			var hasherMock = new Mock<IPasswordHasher>(MockBehavior.Strict);
			var msgSenderMock = new Mock<IMessageSender>(MockBehavior.Strict);
			var playerMock = new Mock<IPlayer>(MockBehavior.Strict);
			var socketMock = new Mock<IWebSocket>(MockBehavior.Strict);

			var username = "a";
			var password = "poiuyt";
			var hashedPassword = "!@46$^^$&;sdfsdf#$%s#$!546$%#%@!";
			var playerData = new PlayerData(username, hashedPassword);

			playerMock
				.SetupGet(p => p.Socket)
				.Returns(socketMock.Object);
			playerMock
				.SetupSet(p => p.PlayerData = It.IsAny<PlayerData>());

			hasherMock
				.Setup(h => h.IsPasswordCorrect(password, hashedPassword))
				.Returns(true);

			databaseMock
				.Setup(d => d.GetPlayerData(username))
				.Returns(playerData);

			msgSenderMock
				.Setup(s => s.SendMessageAsync(socketMock.Object,
					It.IsAny<AuthenticationResultMessage>()))
				.Returns(Task.CompletedTask);

			var msg = new AuthenticationMessage()
			{
				Registration = false,
				Username = username,
				Password = password
			};

			var handler = new LoginHandler(databaseMock.Object,
				hasherMock.Object, msgSenderMock.Object);

			await handler.HandleMessageAsync(playerMock.Object, msg);

			msgSenderMock
				.Verify(s => s.SendMessageAsync(socketMock.Object,
					It.Is<AuthenticationResultMessage>(
						m => m.IsSuccess == true && m.ErrorMessage == "")));

			playerMock
				.VerifySet(p => p.PlayerData = playerData);
		}
		[Test]
		public void IfItIsNotLoginMessageThrowInvalidCastException()
		{
			var databaseMock = new Mock<IPlayerDataDatabase>(MockBehavior.Strict);
			var hasherMock = new Mock<IPasswordHasher>(MockBehavior.Strict);
			var msgSenderMock = new Mock<IMessageSender>(MockBehavior.Strict);
			var playerMock = new Mock<IPlayer>(MockBehavior.Strict);

			var msg = new AuthenticationMessage()
			{
				Registration = true,
				Username = "username",
				Password = "password"
			};

			var handler = new LoginHandler(databaseMock.Object,
				hasherMock.Object, msgSenderMock.Object);

			Assert.ThrowsAsync<InvalidCastException>(async () =>
				await handler.HandleMessageAsync(playerMock.Object, msg));
		}
	}
}