using System;
using System.Text;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using Newtonsoft.Json;
using Server;
using Server.Games;
using Server.Sockets.Messages;

namespace ServerTests
{
	public class ChessTests
	{
		private IHost host;
		private Thread serverThread;
		private Uri serverUrl = new Uri("ws://localhost:5000");
		private int timeoutMiliseconds = 500;
		[SetUp]
		public async Task Setup()
		{
			host = Program.CreateHostBuilder(new string[0]).Build();
			serverThread = new Thread(() => host.Start());
			serverThread.Start();
			await Task.Delay(1000);
		}
		[TearDown]
		public async Task TearDown()
		{
			await host.StopAsync();
			serverThread.Join();
		}
		[Test]
		public async Task ClosingConnectionRightAfterSendingFindChessGameMessageWontCreateGameSession()
		{
			var cts = new CancellationTokenSource();

			var clientSocket1 = new ClientWebSocket();
			var clientSocket2 = new ClientWebSocket();

			await clientSocket1.ConnectAsync(serverUrl, cts.Token);
			await clientSocket2.ConnectAsync(serverUrl, cts.Token);

			var findMsg = new FindChessGameMessage() { ChessGame = true };

			await SendThroughSocketAsync(clientSocket1, findMsg, cts.Token);

			var status = WebSocketCloseStatus.NormalClosure;
			await clientSocket1.CloseAsync(status, "", cts.Token);

			await SendThroughSocketAsync(clientSocket2, findMsg, cts.Token);

			Assert.ThrowsAsync<TaskCanceledException>(async () =>
				await ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket2));
		}
		[Test]
		public async Task TestFindingGameSession()
		{
			var cts = new CancellationTokenSource();

			var clientSocket1 = new ClientWebSocket();
			var clientSocket2 = new ClientWebSocket();

			await clientSocket1.ConnectAsync(serverUrl, cts.Token);
			await clientSocket2.ConnectAsync(serverUrl, cts.Token);

			var findMsg = new FindChessGameMessage() { ChessGame = true };

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
		public async Task TestGettingMovesAndPieces()
		{
			var cts = new CancellationTokenSource();

			var clientSocket1 = new ClientWebSocket();
			var clientSocket2 = new ClientWebSocket();

			await clientSocket1.ConnectAsync(serverUrl, cts.Token);
			await clientSocket2.ConnectAsync(serverUrl, cts.Token);

			var findMsg = new FindChessGameMessage() { ChessGame = true };

			await SendThroughSocketAsync(clientSocket1, findMsg, cts.Token);
			await SendThroughSocketAsync(clientSocket2, findMsg, cts.Token);

			var gameFoundMsg1 = await ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket1);
			var gameFoundMsg2 = await ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket2);

			var piecesAndMovesMsg1 = await ReceiveFromSocketAsync<dynamic>(clientSocket1);
			var piecesAndMovesMsg2 = await ReceiveFromSocketAsync<dynamic>(clientSocket2);

			Assert.AreEqual(32, piecesAndMovesMsg1.Pieces.Count);
			Assert.AreEqual(32, piecesAndMovesMsg2.Pieces.Count);
			Assert.AreEqual(40, piecesAndMovesMsg1.AvailableMoves.Count);
			Assert.AreEqual(40, piecesAndMovesMsg2.AvailableMoves.Count);

			var status = WebSocketCloseStatus.NormalClosure;
			await clientSocket1.CloseAsync(status, "", cts.Token);
			await clientSocket2.CloseAsync(status, "", cts.Token);
		}
		[Test]
		public async Task SessionClosedMessageIsSendWhenAnotherPlayerDisconnects()
		{
			var cts = new CancellationTokenSource();

			var clientSocket1 = new ClientWebSocket();
			var clientSocket2 = new ClientWebSocket();

			await clientSocket1.ConnectAsync(serverUrl, cts.Token);
			await clientSocket2.ConnectAsync(serverUrl, cts.Token);

			var findMsg = new FindChessGameMessage() { ChessGame = true };

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
		public async Task SendingMakeChessMoveMessageWhenThereIsNoSessionShouldReturnIvalidStateMessage()
		{
			var cts = new CancellationTokenSource();

			var clientSocket1 = new ClientWebSocket();

			await clientSocket1.ConnectAsync(serverUrl, cts.Token);
			var makeMoveMsg = new MakeChessMoveMessage()
			{
				X_StartPosition = 1,
				Y_StartPosition = 1,
				X_FinishedPosition = 1,
				Y_FinishedPosition = 3
			};

			await SendThroughSocketAsync(clientSocket1, makeMoveMsg, cts.Token);

			await ReceiveFromSocketAsync<InvalidStateMessage>(clientSocket1);
		}
		[Test]
		public async Task MakingCorrectMoveReturnsChessMoveResultMessageWithSuccessMessage()
		{
			var cts = new CancellationTokenSource();

			var clientSocket1 = new ClientWebSocket();
			var clientSocket2 = new ClientWebSocket();

			await clientSocket1.ConnectAsync(serverUrl, cts.Token);
			await clientSocket2.ConnectAsync(serverUrl, cts.Token);

			var findMsg = new FindChessGameMessage() { ChessGame = true };

			await SendThroughSocketAsync(clientSocket1, findMsg, cts.Token);
			await SendThroughSocketAsync(clientSocket2, findMsg, cts.Token);

			var gameFoundMsg1 = await ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket1);
			var gameFoundMsg2 = await ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket2);

			await ReceiveFromSocketAsync<dynamic>(clientSocket1);
			await ReceiveFromSocketAsync<dynamic>(clientSocket2);

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

			var makeMoveMsg = new MakeChessMoveMessage()
			{
				X_StartPosition = 0,
				Y_StartPosition = 1,
				X_FinishedPosition = 0,
				Y_FinishedPosition = 3
			};

			await SendThroughSocketAsync(firstPlayerSocket, makeMoveMsg, cts.Token);

			var moveResultMessage1 = await ReceiveFromSocketAsync<ChessPlayResultMessage>(
				firstPlayerSocket);
			var moveResultMessage2 = await ReceiveFromSocketAsync<ChessPlayResultMessage>(
				secondPlayerSocket);

			Assert.AreEqual(PlayResult.Success.ToString(), moveResultMessage1.Message);
			Assert.AreEqual(PlayResult.Success.ToString(), moveResultMessage2.Message);

			var status = WebSocketCloseStatus.NormalClosure;
			await clientSocket1.CloseAsync(status, "", cts.Token);
			await clientSocket2.CloseAsync(status, "", cts.Token);
		}
		[Test]
		public async Task MakingMoveWhenItIsNotPlayersTurnReturnsChessMoveResultMessageWithNotYourTurnMessageOnlyToSender()
		{
			var cts = new CancellationTokenSource();

			var clientSocket1 = new ClientWebSocket();
			var clientSocket2 = new ClientWebSocket();

			await clientSocket1.ConnectAsync(serverUrl, cts.Token);
			await clientSocket2.ConnectAsync(serverUrl, cts.Token);

			var findMsg = new FindChessGameMessage() { ChessGame = true };

			await SendThroughSocketAsync(clientSocket1, findMsg, cts.Token);
			await SendThroughSocketAsync(clientSocket2, findMsg, cts.Token);

			var gameFoundMsg1 = await ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket1);
			var gameFoundMsg2 = await ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket2);

			await ReceiveFromSocketAsync<dynamic>(clientSocket1);
			await ReceiveFromSocketAsync<dynamic>(clientSocket2);

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

			var makeMoveMsg = new MakeChessMoveMessage()
			{
				X_StartPosition = 0,
				Y_StartPosition = 1,
				X_FinishedPosition = 0,
				Y_FinishedPosition = 3
			};

			await SendThroughSocketAsync(secondPlayerSocket, makeMoveMsg, cts.Token);

			var chessPlayMessage = await ReceiveFromSocketAsync<ChessPlayResultMessage>(
				secondPlayerSocket);
			Assert.ThrowsAsync<TaskCanceledException>(async () =>
				await ReceiveFromSocketAsync<ChessPlayResultMessage>(firstPlayerSocket));

			Assert.AreEqual(PlayResult.NotYourTurn.ToString(),
				chessPlayMessage.Message);

			var status = WebSocketCloseStatus.NormalClosure;
			await secondPlayerSocket.CloseAsync(status, "", cts.Token);
		}
		[Test]
		public async Task TestWhiteWin()
		{
			var cts = new CancellationTokenSource();

			var clientSocket1 = new ClientWebSocket();
			var clientSocket2 = new ClientWebSocket();

			await clientSocket1.ConnectAsync(serverUrl, cts.Token);
			await clientSocket2.ConnectAsync(serverUrl, cts.Token);

			var findMsg = new FindChessGameMessage() { ChessGame = true };

			await SendThroughSocketAsync(clientSocket1, findMsg, cts.Token);
			await SendThroughSocketAsync(clientSocket2, findMsg, cts.Token);

			var gameFoundMsg1 = await ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket1);
			var gameFoundMsg2 = await ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket2);

			await ReceiveFromSocketAsync<dynamic>(clientSocket1);
			await ReceiveFromSocketAsync<dynamic>(clientSocket2);

			ClientWebSocket currentPlayerSocket, nextPlayerSocket;
			if (gameFoundMsg1.IsClientTurn)
			{
				currentPlayerSocket = clientSocket1;
				nextPlayerSocket = clientSocket2;
			}
			else
			{
				currentPlayerSocket = clientSocket2;
				nextPlayerSocket = clientSocket1;
			}

			var moveMsgs = new List<MakeChessMoveMessage>()
			{
				//king's white pawn two up
				new MakeChessMoveMessage(){
					X_StartPosition = 4, Y_StartPosition = 1,
					X_FinishedPosition = 4, Y_FinishedPosition = 3
				},
				//right bishop's black pawn two down
				new MakeChessMoveMessage(){
					X_StartPosition = 5, Y_StartPosition = 6,
					X_FinishedPosition = 5, Y_FinishedPosition = 4
				},
				//leftmost white pawn two up
				new MakeChessMoveMessage(){
					X_StartPosition = 0, Y_StartPosition = 1,
					X_FinishedPosition = 0, Y_FinishedPosition = 3
				},
				//right knight's black pawn two down
				new MakeChessMoveMessage(){
					X_StartPosition = 6, Y_StartPosition = 6,
					X_FinishedPosition = 6, Y_FinishedPosition = 4
				},
				//white queen checkmate
				new MakeChessMoveMessage(){
					X_StartPosition = 3, Y_StartPosition = 0,
					X_FinishedPosition = 7, Y_FinishedPosition = 4
				},
			};

			ChessPlayResultMessage resultMessage1 = null;
			ChessPlayResultMessage resultMessage2 = null;

			foreach (var moveMsg in moveMsgs)
			{
				await SendThroughSocketAsync(currentPlayerSocket, moveMsg, cts.Token);
				resultMessage1 =
					await ReceiveFromSocketAsync<ChessPlayResultMessage>(currentPlayerSocket);
				resultMessage2 =
					await ReceiveFromSocketAsync<ChessPlayResultMessage>(nextPlayerSocket);

				await ReceiveFromSocketAsync<dynamic>(currentPlayerSocket);
				await ReceiveFromSocketAsync<dynamic>(nextPlayerSocket);

				(currentPlayerSocket, nextPlayerSocket) = (nextPlayerSocket, currentPlayerSocket);
			}

			Assert.AreEqual(PlayResult.YouWin.ToString(), resultMessage1.Message);
			Assert.AreEqual(PlayResult.YouLose.ToString(), resultMessage2.Message);

			var status = WebSocketCloseStatus.NormalClosure;
			await clientSocket1.CloseAsync(status, "", cts.Token);
			await clientSocket2.CloseAsync(status, "", cts.Token);
		}
		[Test]
		public async Task QuickestBlackPromotionTest()
		{
			var cts = new CancellationTokenSource();

			var clientSocket1 = new ClientWebSocket();
			var clientSocket2 = new ClientWebSocket();

			await clientSocket1.ConnectAsync(serverUrl, cts.Token);
			await clientSocket2.ConnectAsync(serverUrl, cts.Token);

			var findMsg = new FindChessGameMessage() { ChessGame = true };

			await SendThroughSocketAsync(clientSocket1, findMsg, cts.Token);
			await SendThroughSocketAsync(clientSocket2, findMsg, cts.Token);

			var gameFoundMsg1 = await ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket1);
			var gameFoundMsg2 = await ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket2);

			await ReceiveFromSocketAsync<dynamic>(clientSocket1);
			await ReceiveFromSocketAsync<dynamic>(clientSocket2);

			ClientWebSocket currentPlayerSocket, nextPlayerSocket;
			if (gameFoundMsg1.IsClientTurn)
			{
				currentPlayerSocket = clientSocket1;
				nextPlayerSocket = clientSocket2;
			}
			else
			{
				currentPlayerSocket = clientSocket2;
				nextPlayerSocket = clientSocket1;
			}

			var moveMsgs = new List<MakeChessMoveMessage>(){
				//leftmost white pawn two up
				new MakeChessMoveMessage(){
					X_StartPosition = 0, Y_StartPosition = 1,
					X_FinishedPosition = 0, Y_FinishedPosition = 3
				},
				//left knight's black pawn two down
				new MakeChessMoveMessage(){
					X_StartPosition = 1, Y_StartPosition = 6,
					X_FinishedPosition = 1, Y_FinishedPosition = 4
				},
				//left white rook two up
				new MakeChessMoveMessage(){
					X_StartPosition = 0, Y_StartPosition = 0,
					X_FinishedPosition = 0, Y_FinishedPosition = 2
				},
				//left knight's black pawn capture of white pawn
				new MakeChessMoveMessage(){
					X_StartPosition = 1, Y_StartPosition = 4,
					X_FinishedPosition = 0, Y_FinishedPosition = 3
				},
				//left white rook four right
				new MakeChessMoveMessage(){
					X_StartPosition = 0, Y_StartPosition = 2,
					X_FinishedPosition = 4, Y_FinishedPosition = 2
				},
				//left knight's black pawn one down
				new MakeChessMoveMessage(){
					X_StartPosition = 0, Y_StartPosition = 3,
					X_FinishedPosition = 0, Y_FinishedPosition = 2
				},
				//right white knight move
				new MakeChessMoveMessage(){
					X_StartPosition = 6, Y_StartPosition = 0,
					X_FinishedPosition = 7, Y_FinishedPosition = 2
				},
				//left knight's black pawn one down
				new MakeChessMoveMessage(){
					X_StartPosition = 0, Y_StartPosition = 2,
					X_FinishedPosition = 0, Y_FinishedPosition = 1
				},
				//right white knight move
				new MakeChessMoveMessage(){
					X_StartPosition = 7, Y_StartPosition = 2,
					X_FinishedPosition = 6, Y_FinishedPosition = 4
				}
			};

			foreach (var moveMsg in moveMsgs)
			{
				await SendThroughSocketAsync(currentPlayerSocket, moveMsg, cts.Token);
				await ReceiveFromSocketAsync<ChessPlayResultMessage>(currentPlayerSocket);
				await ReceiveFromSocketAsync<ChessPlayResultMessage>(nextPlayerSocket);

				await ReceiveFromSocketAsync<dynamic>(currentPlayerSocket);
				await ReceiveFromSocketAsync<dynamic>(nextPlayerSocket);

				(currentPlayerSocket, nextPlayerSocket) = (nextPlayerSocket, currentPlayerSocket);
			}

			//left knight's black pawn reaching promotion area
			var moveBeforePromotionMsg = new MakeChessMoveMessage()
			{
				X_StartPosition = 0,
				Y_StartPosition = 1,
				X_FinishedPosition = 0,
				Y_FinishedPosition = 0
			};

			await SendThroughSocketAsync(currentPlayerSocket, moveBeforePromotionMsg, cts.Token);
			var resultMessage =
				await ReceiveFromSocketAsync<ChessPlayResultMessage>(currentPlayerSocket);
			Assert.AreEqual(PlayResult.PromotionRequired.ToString(), resultMessage.Message);

			var promotionMoveMsg = new PawnPromotionMessage()
			{
				PromotionPiece = "Queen"
			};

			await SendThroughSocketAsync(currentPlayerSocket, promotionMoveMsg, cts.Token);
			await ReceiveFromSocketAsync<ChessPlayResultMessage>(currentPlayerSocket);
			await ReceiveFromSocketAsync<ChessPlayResultMessage>(nextPlayerSocket);

			await ReceiveFromSocketAsync<dynamic>(currentPlayerSocket);
			await ReceiveFromSocketAsync<dynamic>(nextPlayerSocket);

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

			var findMsg = new FindChessGameMessage() { ChessGame = true };

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

			await ReceiveFromSocketAsync<dynamic>(clientSocket1);
			await ReceiveFromSocketAsync<dynamic>(clientSocket2);
			await ReceiveFromSocketAsync<dynamic>(clientSocket3);
			await ReceiveFromSocketAsync<dynamic>(clientSocket4);

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

			var chessMoveMsg = new MakeChessMoveMessage()
			{
				X_StartPosition = 5,
				Y_StartPosition = 1,
				X_FinishedPosition = 5,
				Y_FinishedPosition = 3
			};

			await SendThroughSocketAsync(firstPlayerSocketOfSecondSession,
				chessMoveMsg, cts.Token);

			var moveResultMsg1 = await ReceiveFromSocketAsync<ChessPlayResultMessage>(
				firstPlayerSocketOfSecondSession);
			var moveResultMsg2 = await ReceiveFromSocketAsync<ChessPlayResultMessage>(
				secondPlayerSocketOfSecondSession);

			await SendThroughSocketAsync(firstPlayerSocketOfFirstSession,
				chessMoveMsg, cts.Token);

			var moveResultMsg3 = await ReceiveFromSocketAsync<ChessPlayResultMessage>(
				firstPlayerSocketOfFirstSession);
			var moveResultMsg4 = await ReceiveFromSocketAsync<ChessPlayResultMessage>(
				secondPlayerSocketOfFirstSession);

			var status = WebSocketCloseStatus.NormalClosure;
			await clientSocket1.CloseAsync(status, "", cts.Token);
			await clientSocket2.CloseAsync(status, "", cts.Token);
			await clientSocket3.CloseAsync(status, "", cts.Token);
			await clientSocket4.CloseAsync(status, "", cts.Token);
		}
		[Test]
		public async Task CancelingChessSessionSendsMessageToTheOtherPlayer()
		{
			var cts = new CancellationTokenSource();

			var clientSocket1 = new ClientWebSocket();
			var clientSocket2 = new ClientWebSocket();

			await clientSocket1.ConnectAsync(serverUrl, cts.Token);
			await clientSocket2.ConnectAsync(serverUrl, cts.Token);

			var findMsg = new FindChessGameMessage() { ChessGame = true };

			await SendThroughSocketAsync(clientSocket1, findMsg, cts.Token);
			await SendThroughSocketAsync(clientSocket2, findMsg, cts.Token);

			await ReceiveFromSocketAsync<GameFoundMessage>(clientSocket1);
			await ReceiveFromSocketAsync<GameFoundMessage>(clientSocket2);
			await ReceiveFromSocketAsync<dynamic>(clientSocket1);
			await ReceiveFromSocketAsync<dynamic>(clientSocket2);

			var cancelSessionMsg = new CancelSessionMessage();

			await SendThroughSocketAsync(clientSocket1, cancelSessionMsg, cts.Token);

			await ReceiveFromSocketAsync<SessionClosedMessage>(clientSocket2);

			var status = WebSocketCloseStatus.NormalClosure;
			await clientSocket1.CloseAsync(status, "", cts.Token);
			await clientSocket2.CloseAsync(status, "", cts.Token);
		}

		private async Task<T> ReceiveFromSocketAsync<T>(WebSocket socket)
		{
			var cts = new CancellationTokenSource();
			var buffer = new byte[16384];
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