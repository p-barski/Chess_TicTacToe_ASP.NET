using System;
using System.Text;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using Newtonsoft.Json;
using Server;
using Server.Games;
using Server.Sockets.Messages;

namespace ServerTests
{
	public class TicTacToeTests
	{
		private IHost host;
		private Thread serverThread;
		private Uri serverUrl = new Uri("ws://localhost:5000");
		private int timeoutMiliseconds = 50;
		[SetUp]
		public async Task Setup()
		{
			host = Program.CreateHostBuilder(new string[0]).Build();
			serverThread = new Thread(() => host.Start());
			serverThread.Start();
			await Task.Delay(50);
		}
		[TearDown]
		public async Task TearDown()
		{
			await host.StopAsync();
			serverThread.Join();
		}
		[Test]
		public async Task ClosingConnectionRightAfterSendingFindGameMessageWontCreateGameSession()
		{
			var cts = new CancellationTokenSource();

			var clientSocket1 = new ClientWebSocket();
			var clientSocket2 = new ClientWebSocket();

			await clientSocket1.ConnectAsync(serverUrl, cts.Token);
			await clientSocket2.ConnectAsync(serverUrl, cts.Token);

			var findMsg = new FindGameMessage() { Size = 3 };

			await SendThroughSocketAsync(clientSocket1, findMsg, cts.Token);

			var status = WebSocketCloseStatus.NormalClosure;
			await clientSocket1.CloseAsync(status, "", cts.Token);

			await SendThroughSocketAsync(clientSocket2, findMsg, cts.Token);

			Assert.ThrowsAsync<TaskCanceledException>(async () =>
				await ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket2));
		}
		[Test]
		public async Task SessionClosedMessageIsSendWhenAnotherPlayerDisconnects()
		{
			var cts = new CancellationTokenSource();

			var clientSocket1 = new ClientWebSocket();
			var clientSocket2 = new ClientWebSocket();

			await clientSocket1.ConnectAsync(serverUrl, cts.Token);
			await clientSocket2.ConnectAsync(serverUrl, cts.Token);

			var findMsg = new FindGameMessage() { Size = 3 };

			await SendThroughSocketAsync(clientSocket1, findMsg, cts.Token);
			await SendThroughSocketAsync(clientSocket2, findMsg, cts.Token);

			await ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket1);
			await ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket2);

			var status = WebSocketCloseStatus.NormalClosure;
			await clientSocket1.CloseAsync(status, "", cts.Token);
			await ReceiveFromSocketAsync<SessionClosedMessage>(
				clientSocket2);

			await clientSocket2.CloseAsync(status, "", cts.Token);
		}
		[Test]
		public async Task TestFindingGameSession()
		{
			var cts = new CancellationTokenSource();

			var clientSocket1 = new ClientWebSocket();
			var clientSocket2 = new ClientWebSocket();

			await clientSocket1.ConnectAsync(serverUrl, cts.Token);
			await clientSocket2.ConnectAsync(serverUrl, cts.Token);

			var findMsg = new FindGameMessage() { Size = 3 };

			await SendThroughSocketAsync(clientSocket1, findMsg, cts.Token);
			await SendThroughSocketAsync(clientSocket2, findMsg, cts.Token);

			var gameFoundMsg1 = await ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket1);
			var gameFoundMsg2 = await ReceiveFromSocketAsync<GameFoundMessage>(
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

			await clientSocket1.ConnectAsync(serverUrl, cts.Token);
			var makeMoveMsg = new MakeMoveMessage() { X = 1, Y = 1 };

			await SendThroughSocketAsync(clientSocket1, makeMoveMsg, cts.Token);

			await ReceiveFromSocketAsync<InvalidStateMessage>(clientSocket1);
		}
		[Test]
		public async Task MakingCorrectMoveReturnsMoveResultMessageWithSuccessMessage()
		{
			var cts = new CancellationTokenSource();

			var clientSocket1 = new ClientWebSocket();
			var clientSocket2 = new ClientWebSocket();

			await clientSocket1.ConnectAsync(serverUrl, cts.Token);
			await clientSocket2.ConnectAsync(serverUrl, cts.Token);

			var findMsg = new FindGameMessage() { Size = 3 };

			await SendThroughSocketAsync(clientSocket1, findMsg, cts.Token);
			await SendThroughSocketAsync(clientSocket2, findMsg, cts.Token);

			var gameFoundMsg1 = await ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket1);
			var gameFoundMsg2 = await ReceiveFromSocketAsync<GameFoundMessage>(
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

			await SendThroughSocketAsync(firstPlayerSocket, new MakeMoveMessage()
			{ X = x, Y = y }, cts.Token);

			var moveResultMessage1 = await ReceiveFromSocketAsync<MoveResultMessage>(
				firstPlayerSocket);
			var moveResultMessage2 = await ReceiveFromSocketAsync<MoveResultMessage>(
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

			await clientSocket1.ConnectAsync(serverUrl, cts.Token);
			await clientSocket2.ConnectAsync(serverUrl, cts.Token);

			var findMsg = new FindGameMessage() { Size = 3 };

			await SendThroughSocketAsync(clientSocket1, findMsg, cts.Token);
			await SendThroughSocketAsync(clientSocket2, findMsg, cts.Token);

			var gameFoundMsg1 = await ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket1);
			var gameFoundMsg2 = await ReceiveFromSocketAsync<GameFoundMessage>(
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

			await SendThroughSocketAsync(secondPlayerSocket, new MakeMoveMessage()
			{ X = 1, Y = 2 }, cts.Token);

			var moveResultMessage2 = await ReceiveFromSocketAsync<MoveResultMessage>(
				secondPlayerSocket);
			Assert.ThrowsAsync<TaskCanceledException>(async () =>
				await ReceiveFromSocketAsync<MoveResultMessage>(firstPlayerSocket));

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

			await clientSocket1.ConnectAsync(serverUrl, cts.Token);
			await clientSocket2.ConnectAsync(serverUrl, cts.Token);

			var findMsg = new FindGameMessage() { Size = 3 };

			await SendThroughSocketAsync(clientSocket1, findMsg, cts.Token);
			await SendThroughSocketAsync(clientSocket2, findMsg, cts.Token);

			var gameFoundMsg1 = await ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket1);
			var gameFoundMsg2 = await ReceiveFromSocketAsync<GameFoundMessage>(
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
			await SendThroughSocketAsync(firstPlayerSocket, new MakeMoveMessage()
			{ X = 0, Y = 0 }, cts.Token);

			await ReceiveFromSocketAsync<MoveResultMessage>(firstPlayerSocket);
			await ReceiveFromSocketAsync<MoveResultMessage>(secondPlayerSocket);

			// X| | 
			// O| | 
			//  | | 
			await SendThroughSocketAsync(secondPlayerSocket, new MakeMoveMessage()
			{ X = 0, Y = 1 }, cts.Token);

			await ReceiveFromSocketAsync<MoveResultMessage>(firstPlayerSocket);
			await ReceiveFromSocketAsync<MoveResultMessage>(secondPlayerSocket);

			// X|X| 
			// O| | 
			//  | | 
			await SendThroughSocketAsync(firstPlayerSocket, new MakeMoveMessage()
			{ X = 1, Y = 0 }, cts.Token);

			await ReceiveFromSocketAsync<MoveResultMessage>(firstPlayerSocket);
			await ReceiveFromSocketAsync<MoveResultMessage>(secondPlayerSocket);

			// X|X| 
			// O| | 
			// O| | 
			await SendThroughSocketAsync(secondPlayerSocket, new MakeMoveMessage()
			{ X = 0, Y = 2 }, cts.Token);

			await ReceiveFromSocketAsync<MoveResultMessage>(firstPlayerSocket);
			await ReceiveFromSocketAsync<MoveResultMessage>(secondPlayerSocket);

			// X|X|X
			// O| | 
			// O| | 
			await SendThroughSocketAsync(firstPlayerSocket, new MakeMoveMessage()
			{ X = 2, Y = 0 }, cts.Token);

			var msg1 = await ReceiveFromSocketAsync<MoveResultMessage>(
				firstPlayerSocket);
			var msg2 = await ReceiveFromSocketAsync<MoveResultMessage>(
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

			await clientSocket1.ConnectAsync(serverUrl, cts.Token);
			await clientSocket2.ConnectAsync(serverUrl, cts.Token);

			var findMsg = new FindGameMessage() { Size = 3 };

			await SendThroughSocketAsync(clientSocket1, findMsg, cts.Token);
			await SendThroughSocketAsync(clientSocket2, findMsg, cts.Token);

			var gameFoundMsg1 = await ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket1);
			var gameFoundMsg2 = await ReceiveFromSocketAsync<GameFoundMessage>(
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
			await SendThroughSocketAsync(firstPlayerSocket, new MakeMoveMessage()
			{ X = 0, Y = 0 }, cts.Token);

			await ReceiveFromSocketAsync<MoveResultMessage>(firstPlayerSocket);
			await ReceiveFromSocketAsync<MoveResultMessage>(secondPlayerSocket);

			// X| | 
			// O| | 
			//  | | 
			await SendThroughSocketAsync(secondPlayerSocket, new MakeMoveMessage()
			{ X = 0, Y = 1 }, cts.Token);

			await ReceiveFromSocketAsync<MoveResultMessage>(firstPlayerSocket);
			await ReceiveFromSocketAsync<MoveResultMessage>(secondPlayerSocket);

			// X| | 
			// O| | 
			//  |X| 
			await SendThroughSocketAsync(firstPlayerSocket, new MakeMoveMessage()
			{ X = 1, Y = 2 }, cts.Token);

			await ReceiveFromSocketAsync<MoveResultMessage>(firstPlayerSocket);
			await ReceiveFromSocketAsync<MoveResultMessage>(secondPlayerSocket);

			// X|O| 
			// O| | 
			//  |x| 
			await SendThroughSocketAsync(secondPlayerSocket, new MakeMoveMessage()
			{ X = 1, Y = 0 }, cts.Token);

			await ReceiveFromSocketAsync<MoveResultMessage>(firstPlayerSocket);
			await ReceiveFromSocketAsync<MoveResultMessage>(secondPlayerSocket);

			// X|O| 
			// O| |X
			//  |x| 
			await SendThroughSocketAsync(firstPlayerSocket, new MakeMoveMessage()
			{ X = 2, Y = 1 }, cts.Token);

			await ReceiveFromSocketAsync<MoveResultMessage>(firstPlayerSocket);
			await ReceiveFromSocketAsync<MoveResultMessage>(secondPlayerSocket);

			// X|O| 
			// O| |X
			//  |x|O
			await SendThroughSocketAsync(secondPlayerSocket, new MakeMoveMessage()
			{ X = 2, Y = 2 }, cts.Token);

			await ReceiveFromSocketAsync<MoveResultMessage>(firstPlayerSocket);
			await ReceiveFromSocketAsync<MoveResultMessage>(secondPlayerSocket);

			// X|O| 
			// O|X|X
			//  |x|O
			await SendThroughSocketAsync(firstPlayerSocket, new MakeMoveMessage()
			{ X = 1, Y = 1 }, cts.Token);

			await ReceiveFromSocketAsync<MoveResultMessage>(firstPlayerSocket);
			await ReceiveFromSocketAsync<MoveResultMessage>(secondPlayerSocket);

			// X|O| 
			// O|X|X
			// O|x|O
			await SendThroughSocketAsync(secondPlayerSocket, new MakeMoveMessage()
			{ X = 0, Y = 2 }, cts.Token);

			var msg1 = await ReceiveFromSocketAsync<MoveResultMessage>(
				firstPlayerSocket);
			var msg2 = await ReceiveFromSocketAsync<MoveResultMessage>(
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

			await clientSocket1.ConnectAsync(serverUrl, cts.Token);
			await clientSocket2.ConnectAsync(serverUrl, cts.Token);
			await clientSocket3.ConnectAsync(serverUrl, cts.Token);
			await clientSocket4.ConnectAsync(serverUrl, cts.Token);

			var findMsg = new FindGameMessage() { Size = 3 };

			await SendThroughSocketAsync(clientSocket1, findMsg, cts.Token);
			await SendThroughSocketAsync(clientSocket2, findMsg, cts.Token);
			await SendThroughSocketAsync(clientSocket3, findMsg, cts.Token);
			await SendThroughSocketAsync(clientSocket4, findMsg, cts.Token);

			var gameFoundMsg1 = await ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket1);
			var gameFoundMsg2 = await ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket2);

			var gameFoundMsg3 = await ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket3);
			var gameFoundMsg4 = await ReceiveFromSocketAsync<GameFoundMessage>(
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

			await SendThroughSocketAsync(firstPlayerSocketOfSecondSession,
				new MakeMoveMessage() { X = 2, Y = 2 }, cts.Token);

			var moveResultMsg1 = await ReceiveFromSocketAsync<MoveResultMessage>(
				firstPlayerSocketOfSecondSession);
			var moveResultMsg2 = await ReceiveFromSocketAsync<MoveResultMessage>(
				secondPlayerSocketOfSecondSession);

			Assert.AreEqual(moveResultMsg1.X, 2);
			Assert.AreEqual(moveResultMsg1.Y, 2);
			Assert.AreEqual(moveResultMsg2.X, 2);
			Assert.AreEqual(moveResultMsg2.Y, 2);

			await SendThroughSocketAsync(firstPlayerSocketOfFirstSession,
				new MakeMoveMessage() { X = 1, Y = 1 }, cts.Token);

			var moveResultMsg3 = await ReceiveFromSocketAsync<MoveResultMessage>(
				firstPlayerSocketOfFirstSession);
			var moveResultMsg4 = await ReceiveFromSocketAsync<MoveResultMessage>(
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

		private async Task<T> ReceiveFromSocketAsync<T>(WebSocket socket)
		{
			var cts = new CancellationTokenSource();
			var buffer = new byte[4000];
			var resultTask = socket.ReceiveAsync(buffer, cts.Token);
			int counter = 0;
			int delayTime = 10;
			while (!resultTask.IsCompleted)
			{
				if (counter * delayTime > timeoutMiliseconds)
				{
					cts.Cancel();
					break;
				}
				await Task.Delay(delayTime);
				counter++;
			}
			var result = await resultTask;
			var jsonStr = Encoding.UTF8.GetString(buffer, 0, result.Count);
			var settings = new JsonSerializerSettings();
			settings.MissingMemberHandling = MissingMemberHandling.Error;
			return JsonConvert.DeserializeObject<T>(jsonStr, settings);
		}
		private async Task SendThroughSocketAsync<T>(WebSocket socket, T msgObject, CancellationToken token)
		{
			var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msgObject));
			await socket.SendAsync(buffer, WebSocketMessageType.Text, true, token);
			await Task.Delay(5);
		}
	}
}