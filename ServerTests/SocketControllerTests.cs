using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Moq;
using Server.Sockets;
using Server.Sockets.Handlers;
using Server.Games;
using Server.Database;

namespace ServerTests
{
	public class SocketControllerTests
	{
		private readonly Mock<ILogger<SocketController>> loggerMock =
			new Mock<ILogger<SocketController>>();
		private readonly Mock<ICollections> collectionsMock =
			new Mock<ICollections>(MockBehavior.Strict);
		private readonly Mock<ISocketMessageHandler> msgHandlerMock =
			new Mock<ISocketMessageHandler>(MockBehavior.Strict);
		private readonly Mock<IPlayerDataDatabase> databaseAcessMock =
			new Mock<IPlayerDataDatabase>(MockBehavior.Strict);
		[Test]
		public async Task WhenSocketConnectionTerminates_PlayerShouldBeRemovedFromCollections()
		{
			collectionsMock.Setup(c => c.AddPlayer(It.IsAny<IPlayer>()));
			collectionsMock.Setup(c => c.RemovePlayer(It.IsAny<IPlayer>()))
				.Returns(() => Task.Delay(0));
			databaseAcessMock
				.SetupGet(d => d.PlayerDataForNotLoggedInPlayers)
				.Returns(new PlayerData("mock", "test"));

			var socketMock = new Mock<IWebSocket>(MockBehavior.Strict);
			socketMock.SetupGet(s => s.State).Returns(WebSocketState.Open);
			socketMock.Setup(s => s.ReceiveAsync(It.IsAny<ArraySegment<byte>>(),
				It.IsAny<CancellationToken>()))
				.Returns(() => throw new WebSocketException());

			var controller = new SocketController(loggerMock.Object,
				collectionsMock.Object, msgHandlerMock.Object,
				databaseAcessMock.Object);

			await controller.ReceiveAsync(socketMock.Object);

			collectionsMock.Verify(c => c.AddPlayer(It.IsAny<IPlayer>()));
			collectionsMock.Verify(c => c.RemovePlayer(It.IsAny<IPlayer>()));
		}
	}
}