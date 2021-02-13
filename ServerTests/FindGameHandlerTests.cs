using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Moq;
using Server.Sockets;
using Server.Sockets.Other;
using Server.Sockets.Messages;
using Server.Sockets.Handlers;
using Server.TicTacToe;

namespace ServerTests
{
	public class FindGameHandlerTests
	{
		private readonly Mock<ILogger<FindGameHandler>> loggerMock =
			new Mock<ILogger<FindGameHandler>>();
		private readonly Mock<ICollections> collectionsMock =
			new Mock<ICollections>(MockBehavior.Strict);
		private readonly Mock<IMessageSender> msgSenderMock =
			new Mock<IMessageSender>(MockBehavior.Strict);

		[Test]
		public async Task WhenThereIsNoOpposingPlayer_MarkPlayerAsSearchingForGame()
		{
			int gameSize = 3;

			var sessionFactoryMock = new Mock<IGameSessionFactory>(MockBehavior.Strict);
			var playerMock = new Mock<IPlayer>(MockBehavior.Strict);

			playerMock.SetupGet(p => p.GameSessionGUID)
				.Returns(Guid.Empty);
			playerMock.Setup(p => p.SetAsSearchingForGame(
				It.Is<int>(i => i == gameSize)));

			collectionsMock.Setup(c => c.FindPlayerSearchingForGame(
				It.Is<IPlayer>(p => p == playerMock.Object)))
				.Returns(() => throw new InvalidOperationException());

			var handler = new FindGameHandler(loggerMock.Object,
				collectionsMock.Object, sessionFactoryMock.Object,
				msgSenderMock.Object);

			await handler.HandleMessageAsync(playerMock.Object,
				new FindGameMessage() { Size = gameSize });

			playerMock.Verify(p => p.SetAsSearchingForGame(It.Is<int>(i => i == gameSize)));
			collectionsMock.Verify(c => c.FindPlayerSearchingForGame(
				It.Is<IPlayer>(p => p == playerMock.Object)));
		}
		[Test]
		public async Task WhenThereIsOpposingPlayer_CreateGameSession_AddItToCollections_AndSendMessageBackToPlayers()
		{
			int gameSize = 3;

			var gameSessionMock = new Mock<IGameSession>(MockBehavior.Strict);
			var sessionFactoryMock = new Mock<IGameSessionFactory>(MockBehavior.Strict);
			var playerMock = new Mock<IPlayer>(MockBehavior.Strict);
			var opponentMock = new Mock<IPlayer>(MockBehavior.Strict);
			var playerSocketMock = new Mock<IWebSocket>(MockBehavior.Strict);
			var opponentSocketMock = new Mock<IWebSocket>(MockBehavior.Strict);

			playerMock.SetupGet(p => p.GameSessionGUID)
				.Returns(Guid.Empty);
			playerMock.SetupGet(p => p.Socket)
				.Returns(playerSocketMock.Object);
			playerMock.Setup(p => p.SetAsSearchingForGame(
				It.Is<int>(i => i == gameSize)));

			opponentMock.SetupGet(p => p.Socket)
				.Returns(opponentSocketMock.Object);

			sessionFactoryMock.Setup(f => f.Create(
				It.IsAny<IPlayer>(), It.IsAny<IPlayer>(), It.IsAny<int>()))
				.Returns(gameSessionMock.Object);

			collectionsMock.Setup(c => c.AddSession(It.Is<IGameSession>(
				s => s == gameSessionMock.Object)));
			collectionsMock.Setup(c => c.FindPlayerSearchingForGame(
				It.Is<IPlayer>(p => p == playerMock.Object)))
				.Returns(() => opponentMock.Object);

			msgSenderMock.Setup(m => m.SendMessageAsync(
				It.IsAny<IWebSocket>(),
				It.IsAny<GameFoundMessage>()))
				.Returns(() => Task.Delay(0));

			var handler = new FindGameHandler(loggerMock.Object,
				collectionsMock.Object, sessionFactoryMock.Object,
				msgSenderMock.Object);

			await handler.HandleMessageAsync(playerMock.Object,
				new FindGameMessage() { Size = gameSize });

			playerMock.Verify(p => p.SetAsSearchingForGame(
				It.Is<int>(i => i == gameSize)));

			collectionsMock.Verify(c => c.FindPlayerSearchingForGame(
				It.Is<IPlayer>(p => p == playerMock.Object)));
			collectionsMock.Verify(c => c.AddSession(It.Is<IGameSession>(
				s => s == gameSessionMock.Object)));

			msgSenderMock.Verify(m => m.SendMessageAsync(
				It.Is<IWebSocket>(s => s == playerSocketMock.Object),
				It.IsAny<GameFoundMessage>()));

			msgSenderMock.Verify(m => m.SendMessageAsync(
				It.Is<IWebSocket>(s => s == opponentSocketMock.Object),
				It.IsAny<GameFoundMessage>()));

			sessionFactoryMock.Verify(f => f.Create(
				It.Is<IPlayer>(p1 => p1 == playerMock.Object),
				It.Is<IPlayer>(p2 => p2 == opponentMock.Object),
				It.Is<int>(i => i == gameSize)));
		}
		[Test]
		public async Task WhenPlayerIsAlreadyConnectedToAGameSession_SendInvalidStateMessageBack()
		{
			int gameSize = 3;

			var gameSessionMock = new Mock<IGameSession>(MockBehavior.Strict);
			var sessionFactoryMock = new Mock<IGameSessionFactory>(MockBehavior.Strict);
			var playerMock = new Mock<IPlayer>(MockBehavior.Strict);
			var playerSocketMock = new Mock<IWebSocket>(MockBehavior.Strict);

			playerMock.SetupGet(p => p.GameSessionGUID)
				.Returns(Guid.NewGuid());
			playerMock.SetupGet(p => p.Socket).Returns(playerSocketMock.Object);

			msgSenderMock.Setup(m => m.SendMessageAsync(
				It.IsAny<IWebSocket>(),
				It.IsAny<InvalidStateMessage>()))
				.Returns(() => Task.Delay(0));

			var handler = new FindGameHandler(loggerMock.Object,
				collectionsMock.Object, sessionFactoryMock.Object,
				msgSenderMock.Object);

			await handler.HandleMessageAsync(playerMock.Object,
				new FindGameMessage() { Size = gameSize });

			msgSenderMock.Verify(m => m.SendMessageAsync(
				It.Is<IWebSocket>(s => s == playerSocketMock.Object),
				It.IsAny<InvalidStateMessage>()));
		}
	}
}