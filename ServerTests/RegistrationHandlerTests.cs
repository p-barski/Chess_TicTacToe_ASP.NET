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
	public class RegistrationHandlerTests
	{
		[Test]
		public async Task IfUsernameIsTakenSendBackErrorMessage()
		{
			var databaseMock = new Mock<IPlayerDataDatabase>(MockBehavior.Strict);
			var hasherMock = new Mock<IPasswordHasher>(MockBehavior.Strict);
			var msgSenderMock = new Mock<IMessageSender>(MockBehavior.Strict);
			var playerMock = new Mock<IPlayer>(MockBehavior.Strict);
			var socketMock = new Mock<IWebSocket>(MockBehavior.Strict);

			var username = "testname";
			var password = "12345";
			var hashedPassword = "!@3dfc8df97iop;lkdf234sd89kg#$!";

			playerMock
				.SetupGet(p => p.Socket)
				.Returns(socketMock.Object);

			hasherMock
				.Setup(h => h.HashPassword(password))
				.Returns(hashedPassword);

			databaseMock
				.Setup(d => d.SavePlayerDataAsync(It.IsAny<PlayerData>()))
				.ReturnsAsync(false);

			msgSenderMock
				.Setup(s => s.SendMessageAsync(socketMock.Object,
					It.IsAny<AuthenticationResultMessage>()))
				.Returns(Task.CompletedTask);

			var msg = new AuthenticationMessage()
			{
				Registration = true,
				Username = username,
				Password = password
			};

			var handler = new RegistrationHandler(databaseMock.Object,
				hasherMock.Object, msgSenderMock.Object);

			await handler.HandleMessageAsync(playerMock.Object, msg);

			msgSenderMock
				.Verify(s => s.SendMessageAsync(socketMock.Object,
					It.Is<AuthenticationResultMessage>(
						m => m.IsSuccess == false && m.ErrorMessage != "")));

			databaseMock
				.Verify(d => d.SavePlayerDataAsync(
					It.Is<PlayerData>(d => d.Name == username && d.Password == hashedPassword)));
		}
		[Test]
		public async Task IfRegistrationWasSuccessfulReturnSuccessMessage()
		{
			var databaseMock = new Mock<IPlayerDataDatabase>(MockBehavior.Strict);
			var hasherMock = new Mock<IPasswordHasher>(MockBehavior.Strict);
			var msgSenderMock = new Mock<IMessageSender>(MockBehavior.Strict);
			var playerMock = new Mock<IPlayer>(MockBehavior.Strict);
			var socketMock = new Mock<IWebSocket>(MockBehavior.Strict);

			var username = "nametest";
			var password = "67890";
			var hashedPassword = "!@fsfs;gd#$!546$%#%@!";

			playerMock
				.SetupGet(p => p.Socket)
				.Returns(socketMock.Object);
			playerMock
				.SetupSet(p => p.PlayerData = It.IsAny<PlayerData>());

			hasherMock
				.Setup(h => h.HashPassword(password))
				.Returns(hashedPassword);

			databaseMock
				.Setup(d => d.SavePlayerDataAsync(It.IsAny<PlayerData>()))
				.ReturnsAsync(true);

			msgSenderMock
				.Setup(s => s.SendMessageAsync(socketMock.Object,
					It.IsAny<AuthenticationResultMessage>()))
				.Returns(Task.CompletedTask);

			var msg = new AuthenticationMessage()
			{
				Registration = true,
				Username = username,
				Password = password
			};

			var handler = new RegistrationHandler(databaseMock.Object,
				hasherMock.Object, msgSenderMock.Object);

			await handler.HandleMessageAsync(playerMock.Object, msg);

			msgSenderMock
				.Verify(s => s.SendMessageAsync(socketMock.Object,
					It.Is<AuthenticationResultMessage>(
						m => m.IsSuccess == true && m.ErrorMessage == "")));

			databaseMock
				.Verify(d => d.SavePlayerDataAsync(
					It.Is<PlayerData>(d => d.Name == username && d.Password == hashedPassword)));

			playerMock
				.VerifySet(p => p.PlayerData = It.IsAny<PlayerData>());
		}
		[Test]
		public void IfItIsNotRegistrationMessageThrowInvalidCastException()
		{
			var databaseMock = new Mock<IPlayerDataDatabase>(MockBehavior.Strict);
			var hasherMock = new Mock<IPasswordHasher>(MockBehavior.Strict);
			var msgSenderMock = new Mock<IMessageSender>(MockBehavior.Strict);
			var playerMock = new Mock<IPlayer>(MockBehavior.Strict);

			var msg = new AuthenticationMessage()
			{
				Registration = false,
				Username = "username",
				Password = "password"
			};

			var handler = new RegistrationHandler(databaseMock.Object,
				hasherMock.Object, msgSenderMock.Object);

			Assert.ThrowsAsync<InvalidCastException>(async () =>
				await handler.HandleMessageAsync(playerMock.Object, msg));
		}
	}
}