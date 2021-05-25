using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Server.Games;
using Server.Sockets.Messages;

namespace ServerTests
{
	public class TicTacToeTests
	{
		private TestUtils utils;
		[SetUp]
		public async Task Setup()
		{
			utils = new TestUtils();
			await utils.Setup();
		}
		[TearDown]
		public async Task TearDown()
		{
			await utils.TearDown();
		}
		[Test]
		public async Task ClosingConnectionRightAfterSendingFindGameMessageWontCreateGameSession()
		{
			var cts = new CancellationTokenSource();

			var clientSocket1 = new ClientWebSocket();
			var clientSocket2 = new ClientWebSocket();

			await clientSocket1.ConnectAsync(utils.serverUrl, cts.Token);
			await clientSocket2.ConnectAsync(utils.serverUrl, cts.Token);

			var findMsg = new FindGameMessage() { Size = 3 };

			await utils.SendThroughSocketAsync(clientSocket1, findMsg, cts.Token);

			var status = WebSocketCloseStatus.NormalClosure;
			await clientSocket1.CloseAsync(status, "", cts.Token);

			await utils.SendThroughSocketAsync(clientSocket2, findMsg, cts.Token);

			Assert.ThrowsAsync<TaskCanceledException>(async () =>
				await utils.ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket2));
		}
		[Test]
		public async Task SessionClosedMessageIsSendWhenAnotherPlayerDisconnects()
		{
			var cts = new CancellationTokenSource();

			var clientSocket1 = new ClientWebSocket();
			var clientSocket2 = new ClientWebSocket();

			await clientSocket1.ConnectAsync(utils.serverUrl, cts.Token);
			await clientSocket2.ConnectAsync(utils.serverUrl, cts.Token);

			var findMsg = new FindGameMessage() { Size = 3 };

			await utils.SendThroughSocketAsync(clientSocket1, findMsg, cts.Token);
			await utils.SendThroughSocketAsync(clientSocket2, findMsg, cts.Token);

			await utils.ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket1);
			await utils.ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket2);

			var status = WebSocketCloseStatus.NormalClosure;
			await clientSocket1.CloseAsync(status, "", cts.Token);
			await utils.ReceiveFromSocketAsync<SessionClosedMessage>(
				clientSocket2);

			await clientSocket2.CloseAsync(status, "", cts.Token);
		}
		[Test]
		public async Task TestFindingGameSession()
		{
			var cts = new CancellationTokenSource();

			var clientSocket1 = new ClientWebSocket();
			var clientSocket2 = new ClientWebSocket();

			await clientSocket1.ConnectAsync(utils.serverUrl, cts.Token);
			await clientSocket2.ConnectAsync(utils.serverUrl, cts.Token);

			var findMsg = new FindGameMessage() { Size = 3 };

			await utils.SendThroughSocketAsync(clientSocket1, findMsg, cts.Token);
			await utils.SendThroughSocketAsync(clientSocket2, findMsg, cts.Token);

			var gameFoundMsg1 = await utils.ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket1);
			var gameFoundMsg2 = await utils.ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket2);

			Assert.AreNotEqual(gameFoundMsg1.IsClientTurn,
				gameFoundMsg2.IsClientTurn);

			var status = WebSocketCloseStatus.NormalClosure;
			await clientSocket1.CloseAsync(status, "", cts.Token);
			await clientSocket2.CloseAsync(status, "", cts.Token);
		}
		[Test]
		public async Task SendingMakeMoveMessageWhenThereIsNoSessionShouldNotReturnAnything()
		{
			var cts = new CancellationTokenSource();

			var clientSocket1 = new ClientWebSocket();

			await clientSocket1.ConnectAsync(utils.serverUrl, cts.Token);
			var makeMoveMsg = new MakeMoveMessage() { X = 1, Y = 1 };

			await utils.SendThroughSocketAsync(clientSocket1, makeMoveMsg, cts.Token);

			await utils.ReceiveFromSocketAsync<InvalidStateMessage>(clientSocket1);
		}
		[Test]
		public async Task MakingCorrectMoveReturnsMoveResultMessageWithSuccessMessage()
		{
			var cts = new CancellationTokenSource();

			var clientSocket1 = new ClientWebSocket();
			var clientSocket2 = new ClientWebSocket();

			await clientSocket1.ConnectAsync(utils.serverUrl, cts.Token);
			await clientSocket2.ConnectAsync(utils.serverUrl, cts.Token);

			var findMsg = new FindGameMessage() { Size = 3 };

			await utils.SendThroughSocketAsync(clientSocket1, findMsg, cts.Token);
			await utils.SendThroughSocketAsync(clientSocket2, findMsg, cts.Token);

			var gameFoundMsg1 = await utils.ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket1);
			var gameFoundMsg2 = await utils.ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket2);

			ClientWebSocket firstPlayerSocket, secondPlayerSocket;
			if (gameFoundMsg1.IsClientTurn)
			{
				firstPlayerSocket = clientSocket1;
				secondPlayerSocket = clientSocket2;
			}
			else
			{
				firstPlayerSocket = clientSocket2;
				secondPlayerSocket = clientSocket1;
			}

			int x = 1;
			int y = 2;

			await utils.SendThroughSocketAsync(firstPlayerSocket, new MakeMoveMessage()
			{ X = x, Y = y }, cts.Token);

			var moveResultMessage1 = await utils.ReceiveFromSocketAsync<MoveResultMessage>(
				firstPlayerSocket);
			var moveResultMessage2 = await utils.ReceiveFromSocketAsync<MoveResultMessage>(
				secondPlayerSocket);

			Assert.AreEqual(PlayResult.Success.ToString(), moveResultMessage1.Message);
			Assert.AreEqual(PlayResult.Success.ToString(), moveResultMessage2.Message);
			Assert.AreEqual(x, moveResultMessage1.X);
			Assert.AreEqual(x, moveResultMessage2.X);
			Assert.AreEqual(y, moveResultMessage1.Y);
			Assert.AreEqual(y, moveResultMessage2.Y);

			var status = WebSocketCloseStatus.NormalClosure;
			await clientSocket1.CloseAsync(status, "", cts.Token);
			await clientSocket2.CloseAsync(status, "", cts.Token);
		}
		[Test]
		public async Task MakingIncorrectMoveReturnsMoveResultMessageWithNotYourTurnMessageOnlyToSender()
		{
			var cts = new CancellationTokenSource();

			var clientSocket1 = new ClientWebSocket();
			var clientSocket2 = new ClientWebSocket();

			await clientSocket1.ConnectAsync(utils.serverUrl, cts.Token);
			await clientSocket2.ConnectAsync(utils.serverUrl, cts.Token);

			var findMsg = new FindGameMessage() { Size = 3 };

			await utils.SendThroughSocketAsync(clientSocket1, findMsg, cts.Token);
			await utils.SendThroughSocketAsync(clientSocket2, findMsg, cts.Token);

			var gameFoundMsg1 = await utils.ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket1);
			var gameFoundMsg2 = await utils.ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket2);

			ClientWebSocket firstPlayerSocket, secondPlayerSocket;
			if (gameFoundMsg1.IsClientTurn)
			{
				firstPlayerSocket = clientSocket1;
				secondPlayerSocket = clientSocket2;
			}
			else
			{
				firstPlayerSocket = clientSocket2;
				secondPlayerSocket = clientSocket1;
			}

			await utils.SendThroughSocketAsync(secondPlayerSocket, new MakeMoveMessage()
			{ X = 1, Y = 2 }, cts.Token);

			var moveResultMessage2 = await utils.ReceiveFromSocketAsync<MoveResultMessage>(
				secondPlayerSocket);
			Assert.ThrowsAsync<TaskCanceledException>(async () =>
				await utils.ReceiveFromSocketAsync<MoveResultMessage>(firstPlayerSocket));

			Assert.AreEqual(PlayResult.NotYourTurn.ToString(),
				moveResultMessage2.Message);
			Assert.AreEqual(0, moveResultMessage2.X);
			Assert.AreEqual(0, moveResultMessage2.Y);

			var status = WebSocketCloseStatus.NormalClosure;
			await secondPlayerSocket.CloseAsync(status, "", cts.Token);
		}
		[Test]
		public async Task TestWinningTheGame()
		{
			var cts = new CancellationTokenSource();

			var clientSocket1 = new ClientWebSocket();
			var clientSocket2 = new ClientWebSocket();

			await clientSocket1.ConnectAsync(utils.serverUrl, cts.Token);
			await clientSocket2.ConnectAsync(utils.serverUrl, cts.Token);

			var findMsg = new FindGameMessage() { Size = 3 };

			await utils.SendThroughSocketAsync(clientSocket1, findMsg, cts.Token);
			await utils.SendThroughSocketAsync(clientSocket2, findMsg, cts.Token);

			var gameFoundMsg1 = await utils.ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket1);
			var gameFoundMsg2 = await utils.ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket2);

			ClientWebSocket firstPlayerSocket, secondPlayerSocket;
			if (gameFoundMsg1.IsClientTurn)
			{
				firstPlayerSocket = clientSocket1;
				secondPlayerSocket = clientSocket2;
			}
			else
			{
				firstPlayerSocket = clientSocket2;
				secondPlayerSocket = clientSocket1;
			}

			// X| | 
			//  | | 
			//  | | 
			await utils.SendThroughSocketAsync(firstPlayerSocket, new MakeMoveMessage()
			{ X = 0, Y = 0 }, cts.Token);

			await utils.ReceiveFromSocketAsync<MoveResultMessage>(firstPlayerSocket);
			await utils.ReceiveFromSocketAsync<MoveResultMessage>(secondPlayerSocket);

			// X| | 
			// O| | 
			//  | | 
			await utils.SendThroughSocketAsync(secondPlayerSocket, new MakeMoveMessage()
			{ X = 0, Y = 1 }, cts.Token);

			await utils.ReceiveFromSocketAsync<MoveResultMessage>(firstPlayerSocket);
			await utils.ReceiveFromSocketAsync<MoveResultMessage>(secondPlayerSocket);

			// X|X| 
			// O| | 
			//  | | 
			await utils.SendThroughSocketAsync(firstPlayerSocket, new MakeMoveMessage()
			{ X = 1, Y = 0 }, cts.Token);

			await utils.ReceiveFromSocketAsync<MoveResultMessage>(firstPlayerSocket);
			await utils.ReceiveFromSocketAsync<MoveResultMessage>(secondPlayerSocket);

			// X|X| 
			// O| | 
			// O| | 
			await utils.SendThroughSocketAsync(secondPlayerSocket, new MakeMoveMessage()
			{ X = 0, Y = 2 }, cts.Token);

			await utils.ReceiveFromSocketAsync<MoveResultMessage>(firstPlayerSocket);
			await utils.ReceiveFromSocketAsync<MoveResultMessage>(secondPlayerSocket);

			// X|X|X
			// O| | 
			// O| | 
			await utils.SendThroughSocketAsync(firstPlayerSocket, new MakeMoveMessage()
			{ X = 2, Y = 0 }, cts.Token);

			var msg1 = await utils.ReceiveFromSocketAsync<MoveResultMessage>(
				firstPlayerSocket);
			var msg2 = await utils.ReceiveFromSocketAsync<MoveResultMessage>(
				secondPlayerSocket);

			Assert.AreEqual(PlayResult.YouWin.ToString(), msg1.Message);
			Assert.AreEqual(PlayResult.YouLose.ToString(), msg2.Message);

			var status = WebSocketCloseStatus.NormalClosure;
			await clientSocket1.CloseAsync(status, "", cts.Token);
			await clientSocket2.CloseAsync(status, "", cts.Token);
		}
		[Test]
		public async Task TestDrawingTheGame()
		{
			var cts = new CancellationTokenSource();

			var clientSocket1 = new ClientWebSocket();
			var clientSocket2 = new ClientWebSocket();

			await clientSocket1.ConnectAsync(utils.serverUrl, cts.Token);
			await clientSocket2.ConnectAsync(utils.serverUrl, cts.Token);

			var findMsg = new FindGameMessage() { Size = 3 };

			await utils.SendThroughSocketAsync(clientSocket1, findMsg, cts.Token);
			await utils.SendThroughSocketAsync(clientSocket2, findMsg, cts.Token);

			var gameFoundMsg1 = await utils.ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket1);
			var gameFoundMsg2 = await utils.ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket2);

			ClientWebSocket firstPlayerSocket, secondPlayerSocket;
			if (gameFoundMsg1.IsClientTurn)
			{
				firstPlayerSocket = clientSocket1;
				secondPlayerSocket = clientSocket2;
			}
			else
			{
				firstPlayerSocket = clientSocket2;
				secondPlayerSocket = clientSocket1;
			}

			// X| | 
			//  | | 
			//  | | 
			await utils.SendThroughSocketAsync(firstPlayerSocket, new MakeMoveMessage()
			{ X = 0, Y = 0 }, cts.Token);

			await utils.ReceiveFromSocketAsync<MoveResultMessage>(firstPlayerSocket);
			await utils.ReceiveFromSocketAsync<MoveResultMessage>(secondPlayerSocket);

			// X| | 
			// O| | 
			//  | | 
			await utils.SendThroughSocketAsync(secondPlayerSocket, new MakeMoveMessage()
			{ X = 0, Y = 1 }, cts.Token);

			await utils.ReceiveFromSocketAsync<MoveResultMessage>(firstPlayerSocket);
			await utils.ReceiveFromSocketAsync<MoveResultMessage>(secondPlayerSocket);

			// X| | 
			// O| | 
			//  |X| 
			await utils.SendThroughSocketAsync(firstPlayerSocket, new MakeMoveMessage()
			{ X = 1, Y = 2 }, cts.Token);

			await utils.ReceiveFromSocketAsync<MoveResultMessage>(firstPlayerSocket);
			await utils.ReceiveFromSocketAsync<MoveResultMessage>(secondPlayerSocket);

			// X|O| 
			// O| | 
			//  |x| 
			await utils.SendThroughSocketAsync(secondPlayerSocket, new MakeMoveMessage()
			{ X = 1, Y = 0 }, cts.Token);

			await utils.ReceiveFromSocketAsync<MoveResultMessage>(firstPlayerSocket);
			await utils.ReceiveFromSocketAsync<MoveResultMessage>(secondPlayerSocket);

			// X|O| 
			// O| |X
			//  |x| 
			await utils.SendThroughSocketAsync(firstPlayerSocket, new MakeMoveMessage()
			{ X = 2, Y = 1 }, cts.Token);

			await utils.ReceiveFromSocketAsync<MoveResultMessage>(firstPlayerSocket);
			await utils.ReceiveFromSocketAsync<MoveResultMessage>(secondPlayerSocket);

			// X|O| 
			// O| |X
			//  |x|O
			await utils.SendThroughSocketAsync(secondPlayerSocket, new MakeMoveMessage()
			{ X = 2, Y = 2 }, cts.Token);

			await utils.ReceiveFromSocketAsync<MoveResultMessage>(firstPlayerSocket);
			await utils.ReceiveFromSocketAsync<MoveResultMessage>(secondPlayerSocket);

			// X|O| 
			// O|X|X
			//  |x|O
			await utils.SendThroughSocketAsync(firstPlayerSocket, new MakeMoveMessage()
			{ X = 1, Y = 1 }, cts.Token);

			await utils.ReceiveFromSocketAsync<MoveResultMessage>(firstPlayerSocket);
			await utils.ReceiveFromSocketAsync<MoveResultMessage>(secondPlayerSocket);

			// X|O| 
			// O|X|X
			// O|x|O
			await utils.SendThroughSocketAsync(secondPlayerSocket, new MakeMoveMessage()
			{ X = 0, Y = 2 }, cts.Token);

			var msg1 = await utils.ReceiveFromSocketAsync<MoveResultMessage>(
				firstPlayerSocket);
			var msg2 = await utils.ReceiveFromSocketAsync<MoveResultMessage>(
				secondPlayerSocket);

			Assert.AreEqual(PlayResult.Draw.ToString(), msg1.Message);
			Assert.AreEqual(PlayResult.Draw.ToString(), msg2.Message);

			var status = WebSocketCloseStatus.NormalClosure;
			await clientSocket1.CloseAsync(status, "", cts.Token);
			await clientSocket2.CloseAsync(status, "", cts.Token);
		}
		[Test]
		public async Task TestMultipleSessions()
		{
			var cts = new CancellationTokenSource();

			var clientSocket1 = new ClientWebSocket();
			var clientSocket2 = new ClientWebSocket();
			var clientSocket3 = new ClientWebSocket();
			var clientSocket4 = new ClientWebSocket();

			await clientSocket1.ConnectAsync(utils.serverUrl, cts.Token);
			await clientSocket2.ConnectAsync(utils.serverUrl, cts.Token);
			await clientSocket3.ConnectAsync(utils.serverUrl, cts.Token);
			await clientSocket4.ConnectAsync(utils.serverUrl, cts.Token);

			var findMsg = new FindGameMessage() { Size = 3 };

			await utils.SendThroughSocketAsync(clientSocket1, findMsg, cts.Token);
			await utils.SendThroughSocketAsync(clientSocket2, findMsg, cts.Token);
			await utils.SendThroughSocketAsync(clientSocket3, findMsg, cts.Token);
			await utils.SendThroughSocketAsync(clientSocket4, findMsg, cts.Token);

			var gameFoundMsg1 = await utils.ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket1);
			var gameFoundMsg2 = await utils.ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket2);

			var gameFoundMsg3 = await utils.ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket3);
			var gameFoundMsg4 = await utils.ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket4);

			Assert.AreNotEqual(gameFoundMsg1.IsClientTurn, gameFoundMsg2.IsClientTurn);
			Assert.AreNotEqual(gameFoundMsg3.IsClientTurn, gameFoundMsg4.IsClientTurn);

			ClientWebSocket firstPlayerSocketOfFirstSession;
			ClientWebSocket secondPlayerSocketOfFirstSession;
			if (gameFoundMsg1.IsClientTurn)
			{
				firstPlayerSocketOfFirstSession = clientSocket1;
				secondPlayerSocketOfFirstSession = clientSocket2;
			}
			else
			{
				firstPlayerSocketOfFirstSession = clientSocket2;
				secondPlayerSocketOfFirstSession = clientSocket1;
			}

			ClientWebSocket firstPlayerSocketOfSecondSession;
			ClientWebSocket secondPlayerSocketOfSecondSession;
			if (gameFoundMsg3.IsClientTurn)
			{
				firstPlayerSocketOfSecondSession = clientSocket3;
				secondPlayerSocketOfSecondSession = clientSocket4;
			}
			else
			{
				firstPlayerSocketOfSecondSession = clientSocket4;
				secondPlayerSocketOfSecondSession = clientSocket3;
			}

			await utils.SendThroughSocketAsync(firstPlayerSocketOfSecondSession,
				new MakeMoveMessage() { X = 2, Y = 2 }, cts.Token);

			var moveResultMsg1 = await utils.ReceiveFromSocketAsync<MoveResultMessage>(
				firstPlayerSocketOfSecondSession);
			var moveResultMsg2 = await utils.ReceiveFromSocketAsync<MoveResultMessage>(
				secondPlayerSocketOfSecondSession);

			Assert.AreEqual(moveResultMsg1.X, 2);
			Assert.AreEqual(moveResultMsg1.Y, 2);
			Assert.AreEqual(moveResultMsg2.X, 2);
			Assert.AreEqual(moveResultMsg2.Y, 2);

			await utils.SendThroughSocketAsync(firstPlayerSocketOfFirstSession,
				new MakeMoveMessage() { X = 1, Y = 1 }, cts.Token);

			var moveResultMsg3 = await utils.ReceiveFromSocketAsync<MoveResultMessage>(
				firstPlayerSocketOfFirstSession);
			var moveResultMsg4 = await utils.ReceiveFromSocketAsync<MoveResultMessage>(
				secondPlayerSocketOfFirstSession);

			Assert.AreEqual(moveResultMsg3.X, 1);
			Assert.AreEqual(moveResultMsg3.Y, 1);
			Assert.AreEqual(moveResultMsg4.X, 1);
			Assert.AreEqual(moveResultMsg4.Y, 1);

			var status = WebSocketCloseStatus.NormalClosure;
			await clientSocket1.CloseAsync(status, "", cts.Token);
			await clientSocket2.CloseAsync(status, "", cts.Token);
			await clientSocket3.CloseAsync(status, "", cts.Token);
			await clientSocket4.CloseAsync(status, "", cts.Token);
		}
		[Test]
		public async Task CancelingTicTacToeSessionSendsMessageToTheOtherPlayer()
		{
			var cts = new CancellationTokenSource();

			var clientSocket1 = new ClientWebSocket();
			var clientSocket2 = new ClientWebSocket();

			await clientSocket1.ConnectAsync(utils.serverUrl, cts.Token);
			await clientSocket2.ConnectAsync(utils.serverUrl, cts.Token);

			var findMsg = new FindGameMessage() { Size = 4 };

			await utils.SendThroughSocketAsync(clientSocket1, findMsg, cts.Token);
			await utils.SendThroughSocketAsync(clientSocket2, findMsg, cts.Token);

			await utils.ReceiveFromSocketAsync<GameFoundMessage>(clientSocket1);
			await utils.ReceiveFromSocketAsync<GameFoundMessage>(clientSocket2);

			var cancelSessionMsg = new CancelSessionMessage();

			await utils.SendThroughSocketAsync(clientSocket1, cancelSessionMsg, cts.Token);

			await utils.ReceiveFromSocketAsync<SessionClosedMessage>(clientSocket2);

			var status = WebSocketCloseStatus.NormalClosure;
			await clientSocket1.CloseAsync(status, "", cts.Token);
			await clientSocket2.CloseAsync(status, "", cts.Token);
		}
	}
}