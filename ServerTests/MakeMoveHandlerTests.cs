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
	public class MakeMoveHandlerTests
	{
		private readonly Mock<ILogger<MakeMoveHandler>> loggerMock =
			new Mock<ILogger<MakeMoveHandler>>();
		private readonly Mock<IMessageSender> msgSenderMock =
			new Mock<IMessageSender>(MockBehavior.Strict);
		private readonly Mock<ICollections> collectionsMock =
			new Mock<ICollections>(MockBehavior.Strict);

		[Test]
		public async Task WhenThereIsDraw_SendMessagesToBothPlayers_AndRemoveGameSession()
		{
			int x = 0, y = 0;
			var senderMock = new Mock<IPlayer>(MockBehavior.Strict);
			var opponentMock = new Mock<IPlayer>(MockBehavior.Strict);
			var sessionMock = new Mock<IGameSession>(MockBehavior.Strict);
			var senderSocketMock = new Mock<IWebSocket>(MockBehavior.Strict);
			var opponentSocketMock = new Mock<IWebSocket>(MockBehavior.Strict);

			senderMock.SetupGet(p => p.Socket).Returns(senderSocketMock.Object);
			opponentMock.SetupGet(p => p.Socket).Returns(opponentSocketMock.Object);

			sessionMock.SetupGet(s => s.PlayerOne).Returns(senderMock.Object);
			sessionMock.SetupGet(s => s.PlayerTwo).Returns(opponentMock.Object);

			sessionMock.Setup(s => s.Play(senderMock.Object, x, y))
				.Returns(PlayResult.Draw);

			collectionsMock.Setup(c => c.FindSessionOfAPlayer(
				It.Is<IPlayer>(p => p == senderMock.Object)))
				.Returns(sessionMock.Object);

			collectionsMock.Setup(c => c.RemoveSession(sessionMock.Object));

			msgSenderMock.Setup(m => m.SendMessageAsync(It.IsAny<IWebSocket>(),
				It.IsAny<MoveResultMessage>()))
				.Returns(Task.Delay(0));

			var handler = new MakeMoveHandler(collectionsMock.Object,
				loggerMock.Object, msgSenderMock.Object);

			await handler.HandleMessageAsync(senderMock.Object,
				new MakeMoveMessage() { X = x, Y = y });

			msgSenderMock.Verify(m => m.SendMessageAsync(
				It.Is<IWebSocket>(s => s == senderSocketMock.Object),
				It.Is<MoveResultMessage>(
					m => m.Message == PlayResult.Draw.ToString()
					|| m.X == x || m.Y == y)));

			msgSenderMock.Verify(m => m.SendMessageAsync(
				It.Is<IWebSocket>(s => s == opponentSocketMock.Object),
				It.Is<MoveResultMessage>(
					m => m.Message == PlayResult.Draw.ToString()
					|| m.X == x || m.Y == y)));

			collectionsMock.Verify(c => c.RemoveSession(sessionMock.Object));
		}
		public async Task WhenMoveIsSuccessful_SendMessageToBothPlayers()
		{
			int x = 1, y = 2;
			var senderMock = new Mock<IPlayer>(MockBehavior.Strict);
			var opponentMock = new Mock<IPlayer>(MockBehavior.Strict);
			var sessionMock = new Mock<IGameSession>(MockBehavior.Strict);
			var senderSocketMock = new Mock<IWebSocket>(MockBehavior.Strict);
			var opponentSocketMock = new Mock<IWebSocket>(MockBehavior.Strict);

			senderMock.SetupGet(p => p.Socket).Returns(senderSocketMock.Object);
			opponentMock.SetupGet(p => p.Socket).Returns(opponentSocketMock.Object);

			sessionMock.SetupGet(s => s.PlayerOne).Returns(senderMock.Object);
			sessionMock.SetupGet(s => s.PlayerTwo).Returns(opponentMock.Object);

			sessionMock.Setup(s => s.Play(senderMock.Object, x, y))
				.Returns(PlayResult.Success);

			collectionsMock.Setup(c => c.FindSessionOfAPlayer(
				It.Is<IPlayer>(p => p == senderMock.Object)))
				.Returns(sessionMock.Object);

			msgSenderMock.Setup(m => m.SendMessageAsync(
				It.IsAny<IWebSocket>(),
				It.IsAny<MoveResultMessage>()))
				.Returns(Task.Delay(0));

			var handler = new MakeMoveHandler(collectionsMock.Object,
				loggerMock.Object, msgSenderMock.Object);

			await handler.HandleMessageAsync(senderMock.Object,
				new MakeMoveMessage() { X = x, Y = y });

			msgSenderMock.Verify(m => m.SendMessageAsync(
				It.Is<IWebSocket>(s => s == senderSocketMock.Object),
				It.Is<MoveResultMessage>(
					m => m.Message == PlayResult.Success.ToString()
					|| m.X == x || m.Y == y)));

			msgSenderMock.Verify(m => m.SendMessageAsync(
				It.Is<IWebSocket>(s => s == opponentSocketMock.Object),
				It.Is<MoveResultMessage>(
					m => m.Message == PlayResult.Success.ToString()
					|| m.X == x || m.Y == y)));
		}
		[Test]
		public async Task WhenSenderWins_SendMessagesToBothPlayers_AndRemoveGameSession()
		{
			int x = 1, y = 2;
			var senderMock = new Mock<IPlayer>(MockBehavior.Strict);
			var opponentMock = new Mock<IPlayer>(MockBehavior.Strict);
			var sessionMock = new Mock<IGameSession>(MockBehavior.Strict);
			var senderSocketMock = new Mock<IWebSocket>(MockBehavior.Strict);
			var opponentSocketMock = new Mock<IWebSocket>(MockBehavior.Strict);

			senderMock.SetupGet(p => p.Socket).Returns(senderSocketMock.Object);
			opponentMock.SetupGet(p => p.Socket).Returns(opponentSocketMock.Object);

			sessionMock.SetupGet(s => s.PlayerOne).Returns(senderMock.Object);
			sessionMock.SetupGet(s => s.PlayerTwo).Returns(opponentMock.Object);

			sessionMock.Setup(s => s.Play(senderMock.Object, x, y))
				.Returns(PlayResult.YouWin);

			collectionsMock.Setup(c => c.FindSessionOfAPlayer(
				It.Is<IPlayer>(p => p == senderMock.Object)))
				.Returns(sessionMock.Object);

			collectionsMock.Setup(c => c.RemoveSession(sessionMock.Object));

			msgSenderMock.Setup(m => m.SendMessageAsync(It.IsAny<IWebSocket>(),
				It.IsAny<MoveResultMessage>()))
				.Returns(Task.Delay(0));

			var handler = new MakeMoveHandler(collectionsMock.Object,
				loggerMock.Object, msgSenderMock.Object);

			await handler.HandleMessageAsync(senderMock.Object,
				new MakeMoveMessage() { X = x, Y = y });

			msgSenderMock.Verify(m => m.SendMessageAsync(
				It.Is<IWebSocket>(s => s == senderSocketMock.Object),
				It.Is<MoveResultMessage>(
					m => m.Message == PlayResult.YouWin.ToString()
					|| m.X == x || m.Y == y)));

			msgSenderMock.Verify(m => m.SendMessageAsync(
				It.Is<IWebSocket>(s => s == opponentSocketMock.Object),
				It.Is<MoveResultMessage>(
					m => m.Message == PlayResult.YouLose.ToString()
					|| m.X == x || m.Y == y)));

			collectionsMock.Verify(c => c.RemoveSession(sessionMock.Object));
		}
		[Test]
		public async Task WhenItIsNotSendersTurn_SendMessageOnlyToSender()
		{
			int x = 1, y = 2;
			var senderMock = new Mock<IPlayer>(MockBehavior.Strict);
			var opponentMock = new Mock<IPlayer>(MockBehavior.Strict);
			var sessionMock = new Mock<IGameSession>(MockBehavior.Strict);
			var senderSocketMock = new Mock<IWebSocket>(MockBehavior.Strict);

			senderMock.SetupGet(p => p.Socket).Returns(senderSocketMock.Object);

			sessionMock.SetupGet(s => s.PlayerOne).Returns(senderMock.Object);
			sessionMock.SetupGet(s => s.PlayerTwo).Returns(opponentMock.Object);

			sessionMock.Setup(s => s.Play(senderMock.Object, x, y))
				.Returns(PlayResult.NotYourTurn);

			collectionsMock.Setup(c => c.FindSessionOfAPlayer(
				It.Is<IPlayer>(p => p == senderMock.Object)))
				.Returns(sessionMock.Object);

			collectionsMock.Setup(c => c.RemoveSession(sessionMock.Object));

			msgSenderMock.Setup(m => m.SendMessageAsync(
				It.Is<IWebSocket>(s => s == senderSocketMock.Object),
				It.Is<MoveResultMessage>(m => m.Message == PlayResult.NotYourTurn.ToString())))
				.Returns(Task.Delay(0));

			var handler = new MakeMoveHandler(collectionsMock.Object,
				loggerMock.Object, msgSenderMock.Object);

			await handler.HandleMessageAsync(senderMock.Object,
				new MakeMoveMessage() { X = x, Y = y });

			msgSenderMock.Verify(m => m.SendMessageAsync(
				It.Is<IWebSocket>(s => s == senderSocketMock.Object),
				It.Is<MoveResultMessage>(
					m => m.Message == PlayResult.NotYourTurn.ToString()
					|| m.X == x || m.Y == y)));
		}
		[Test]
		public async Task WhenPositionIsTaken_SendMessageOnlyToSender()
		{
			int x = 1, y = 2;
			var senderMock = new Mock<IPlayer>(MockBehavior.Strict);
			var opponentMock = new Mock<IPlayer>(MockBehavior.Strict);
			var sessionMock = new Mock<IGameSession>(MockBehavior.Strict);
			var senderSocketMock = new Mock<IWebSocket>(MockBehavior.Strict);

			senderMock.SetupGet(p => p.Socket).Returns(senderSocketMock.Object);

			sessionMock.SetupGet(s => s.PlayerOne).Returns(senderMock.Object);
			sessionMock.SetupGet(s => s.PlayerTwo).Returns(opponentMock.Object);

			sessionMock.Setup(s => s.Play(senderMock.Object, x, y))
				.Returns(PlayResult.PositionTaken);

			collectionsMock.Setup(c => c.FindSessionOfAPlayer(
				It.Is<IPlayer>(p => p == senderMock.Object)))
				.Returns(sessionMock.Object);

			collectionsMock.Setup(c => c.RemoveSession(sessionMock.Object));

			msgSenderMock.Setup(m => m.SendMessageAsync(
				It.Is<IWebSocket>(s => s == senderSocketMock.Object),
				It.Is<MoveResultMessage>(m => m.Message == PlayResult.PositionTaken.ToString())))
				.Returns(Task.Delay(0));

			var handler = new MakeMoveHandler(collectionsMock.Object,
				loggerMock.Object, msgSenderMock.Object);

			await handler.HandleMessageAsync(senderMock.Object,
				new MakeMoveMessage() { X = x, Y = y });

			msgSenderMock.Verify(m => m.SendMessageAsync(
				It.Is<IWebSocket>(s => s == senderSocketMock.Object),
				It.Is<MoveResultMessage>(
					m => m.Message == PlayResult.PositionTaken.ToString()
					|| m.X == x || m.Y == y)));
		}
		[Test]
		public async Task WhenSenderIsNotInASession_SendErrorMessage()
		{
			int x = 0, y = 1;
			var senderMock = new Mock<IPlayer>(MockBehavior.Strict);
			var senderSocketMock = new Mock<IWebSocket>(MockBehavior.Strict);

			senderMock.SetupGet(p => p.Socket).Returns(senderSocketMock.Object);

			collectionsMock.Setup(c => c.FindSessionOfAPlayer(
				It.Is<IPlayer>(p => p == senderMock.Object)))
				.Returns(() => throw new InvalidOperationException());

			msgSenderMock.Setup(m => m.SendMessageAsync(
				It.IsAny<IWebSocket>(),
				It.IsAny<ISendMessage>()))
				.Returns(Task.Delay(0));

			var handler = new MakeMoveHandler(collectionsMock.Object,
				loggerMock.Object, msgSenderMock.Object);

			await handler.HandleMessageAsync(senderMock.Object,
				new MakeMoveMessage() { X = x, Y = y });

			msgSenderMock.Verify(m => m.SendMessageAsync(
				It.Is<IWebSocket>(s => s == senderSocketMock.Object),
				It.IsAny<InvalidStateMessage>()));
		}
	}
}