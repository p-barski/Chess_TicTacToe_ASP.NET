using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Moq;
using Server.Sockets;
using Server.Sockets.Other;
using Server.Sockets.Messages;
using Server.Games;

namespace ServerTests
{
	public class CollectionsTests
	{
		private readonly Mock<ILogger<Collections>> loggerMock =
			new Mock<ILogger<Collections>>();
		private readonly Mock<IMessageSender> msgSenderMock =
			new Mock<IMessageSender>(MockBehavior.Strict);

		[Test]
		public async Task WhenPlayer_ThatIsInGameSession_IsRemoved_SessionClosedMessageIsSentToOtherPlayer()
		{
			msgSenderMock.Setup(m => m.SendMessageAsync(It.IsAny<IWebSocket>(),
				It.IsAny<SessionClosedMessage>())).Returns(() => Task.Delay(0));

			var player1Mock = new Mock<IPlayer>(MockBehavior.Strict);
			var player2Mock = new Mock<IPlayer>(MockBehavior.Strict);

			var player1SocketMock = new Mock<IWebSocket>();
			var player2SocketMock = new Mock<IWebSocket>();

			var gameSessionMock = new Mock<IGameSession>(MockBehavior.Strict);

			var player1Guid = Guid.NewGuid();
			var player2Guid = Guid.NewGuid();
			var sessionGuid = Guid.NewGuid();

			player1Mock.SetupGet(p => p.GUID).Returns(player1Guid);
			player1Mock.SetupGet(p => p.GameSessionGUID).Returns(sessionGuid);
			player1Mock.SetupGet(p => p.Socket).Returns(player1SocketMock.Object);

			player2Mock.SetupGet(p => p.GUID).Returns(player2Guid);
			player2Mock.SetupGet(p => p.GameSessionGUID).Returns(sessionGuid);
			player2Mock.SetupGet(p => p.Socket).Returns(player2SocketMock.Object);

			gameSessionMock.SetupGet(s => s.GUID).Returns(sessionGuid);
			gameSessionMock.SetupGet(s => s.PlayerOne).Returns(player1Mock.Object);
			gameSessionMock.SetupGet(s => s.PlayerTwo).Returns(player2Mock.Object);
			gameSessionMock.Setup(s => s.Close());

			var collections = new Collections(loggerMock.Object, msgSenderMock.Object);

			collections.AddPlayer(player1Mock.Object);
			collections.AddPlayer(player2Mock.Object);
			collections.AddSession(gameSessionMock.Object);
			await collections.RemovePlayer(player1Mock.Object);

			msgSenderMock.Verify(m => m.SendMessageAsync(player2SocketMock.Object,
				It.IsAny<SessionClosedMessage>()));
			gameSessionMock.Verify(s => s.Close());
		}
		[Test]
		public void FindPlayerSearchingForGame_FindsOpponentWhenThereIsOne()
		{
			var player1Mock = new Mock<IPlayer>(MockBehavior.Strict);
			var player2Mock = new Mock<IPlayer>(MockBehavior.Strict);

			var player1Guid = Guid.NewGuid();
			var player2Guid = Guid.NewGuid();

			player1Mock.SetupGet(p => p.GUID).Returns(player1Guid);
			player1Mock.SetupGet(p => p.State).Returns(PlayerState.SearchingForGame);
			player1Mock.SetupGet(p => p.ExpectedBoardSize).Returns(5);

			player2Mock.SetupGet(p => p.GUID).Returns(player2Guid);
			player2Mock.SetupGet(p => p.State).Returns(PlayerState.SearchingForGame);
			player2Mock.SetupGet(p => p.ExpectedBoardSize).Returns(5);

			var collections = new Collections(loggerMock.Object, msgSenderMock.Object);

			collections.AddPlayer(player1Mock.Object);
			collections.AddPlayer(player2Mock.Object);

			var opponent = collections.FindPlayerSearchingForGame(player1Mock.Object);

			Assert.AreEqual(player2Mock.Object, opponent);
		}
		[Test]
		public void FindPlayerSearchingForGame_ThrowsInvalidOperationException_WhenThereIsNoOtherPlayerSearchingForGame()
		{
			var player1Mock = new Mock<IPlayer>(MockBehavior.Strict);
			var player2Mock = new Mock<IPlayer>(MockBehavior.Strict);
			var player3Mock = new Mock<IPlayer>(MockBehavior.Strict);
			var player4Mock = new Mock<IPlayer>(MockBehavior.Strict);

			var player1Guid = Guid.NewGuid();
			var player2Guid = Guid.NewGuid();
			var player3Guid = Guid.NewGuid();
			var player4Guid = Guid.NewGuid();

			player1Mock.SetupGet(p => p.GUID).Returns(player1Guid);
			player1Mock.SetupGet(p => p.State).Returns(PlayerState.SearchingForGame);
			player1Mock.SetupGet(p => p.ExpectedBoardSize).Returns(5);

			player2Mock.SetupGet(p => p.GUID).Returns(player2Guid);
			player2Mock.SetupGet(p => p.State).Returns(PlayerState.Playing);
			player2Mock.SetupGet(p => p.ExpectedBoardSize).Returns(5);

			player3Mock.SetupGet(p => p.GUID).Returns(player3Guid);
			player3Mock.SetupGet(p => p.State).Returns(PlayerState.Playing);
			player3Mock.SetupGet(p => p.ExpectedBoardSize).Returns(5);

			player4Mock.SetupGet(p => p.GUID).Returns(player4Guid);
			player4Mock.SetupGet(p => p.State).Returns(PlayerState.Idle);
			player4Mock.SetupGet(p => p.ExpectedBoardSize).Returns(5);

			var collections = new Collections(loggerMock.Object, msgSenderMock.Object);

			collections.AddPlayer(player1Mock.Object);
			collections.AddPlayer(player2Mock.Object);
			collections.AddPlayer(player3Mock.Object);
			collections.AddPlayer(player4Mock.Object);

			Assert.Throws<InvalidOperationException>(
				() => collections.FindPlayerSearchingForGame(player1Mock.Object));
		}
	}
}