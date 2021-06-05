using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using NUnit.Framework;
using Server.Games;
using Server.Sockets.Messages;

namespace ServerTests
{
	public class ChessTests
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
		public async Task ClosingConnectionRightAfterSendingFindChessGameMessageWontCreateGameSession()
		{
			var cts = new CancellationTokenSource();

			var clientSocket1 = new ClientWebSocket();
			var clientSocket2 = new ClientWebSocket();

			await clientSocket1.ConnectAsync(utils.serverUrl, cts.Token);
			await clientSocket2.ConnectAsync(utils.serverUrl, cts.Token);

			var findMsg = new FindChessGameMessage() { ChessGame = true };

			await utils.SendThroughSocketAsync(clientSocket1, findMsg, cts.Token);

			var status = WebSocketCloseStatus.NormalClosure;
			await clientSocket1.CloseAsync(status, "", cts.Token);

			await utils.SendThroughSocketAsync(clientSocket2, findMsg, cts.Token);

			Assert.ThrowsAsync<TaskCanceledException>(async () =>
				await utils.ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket2));
		}
		[Test]
		public async Task TestFindingGameSession()
		{
			var cts = new CancellationTokenSource();

			var clientSocket1 = new ClientWebSocket();
			var clientSocket2 = new ClientWebSocket();

			await clientSocket1.ConnectAsync(utils.serverUrl, cts.Token);
			await clientSocket2.ConnectAsync(utils.serverUrl, cts.Token);

			var findMsg = new FindChessGameMessage() { ChessGame = true };

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
		public async Task TestGettingMovesAndPieces()
		{
			var cts = new CancellationTokenSource();

			var clientSocket1 = new ClientWebSocket();
			var clientSocket2 = new ClientWebSocket();

			await clientSocket1.ConnectAsync(utils.serverUrl, cts.Token);
			await clientSocket2.ConnectAsync(utils.serverUrl, cts.Token);

			var findMsg = new FindChessGameMessage() { ChessGame = true };

			await utils.SendThroughSocketAsync(clientSocket1, findMsg, cts.Token);
			await utils.SendThroughSocketAsync(clientSocket2, findMsg, cts.Token);

			var gameFoundMsg1 = await utils.ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket1);
			var gameFoundMsg2 = await utils.ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket2);

			var piecesAndMovesMsg1 = await utils.ReceiveFromSocketAsync<dynamic>(clientSocket1);
			var piecesAndMovesMsg2 = await utils.ReceiveFromSocketAsync<dynamic>(clientSocket2);

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

			await clientSocket1.ConnectAsync(utils.serverUrl, cts.Token);
			await clientSocket2.ConnectAsync(utils.serverUrl, cts.Token);

			var findMsg = new FindChessGameMessage() { ChessGame = true };

			await utils.SendThroughSocketAsync(clientSocket1, findMsg, cts.Token);
			await utils.SendThroughSocketAsync(clientSocket2, findMsg, cts.Token);

			await utils.ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket1);
			await utils.ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket2);

			await utils.ReceiveFromSocketAsync<dynamic>(
				clientSocket1);
			await utils.ReceiveFromSocketAsync<dynamic>(
				clientSocket2);

			var status = WebSocketCloseStatus.NormalClosure;
			await clientSocket1.CloseAsync(status, "", cts.Token);
			await utils.ReceiveFromSocketAsync<SessionClosedMessage>(
				clientSocket2);

			await clientSocket2.CloseAsync(status, "", cts.Token);
		}
		[Test]
		public async Task SendingMakeChessMoveMessageWhenThereIsNoSessionShouldReturnIvalidStateMessage()
		{
			var cts = new CancellationTokenSource();

			var clientSocket1 = new ClientWebSocket();

			await clientSocket1.ConnectAsync(utils.serverUrl, cts.Token);
			var makeMoveMsg = new MakeChessMoveMessage()
			{
				X_StartPosition = 1,
				Y_StartPosition = 1,
				X_FinishedPosition = 1,
				Y_FinishedPosition = 3
			};

			await utils.SendThroughSocketAsync(clientSocket1, makeMoveMsg, cts.Token);

			await utils.ReceiveFromSocketAsync<InvalidStateMessage>(clientSocket1);
		}
		[Test]
		public async Task MakingCorrectMoveReturnsChessMoveResultMessageWithSuccessMessage()
		{
			var cts = new CancellationTokenSource();

			var clientSocket1 = new ClientWebSocket();
			var clientSocket2 = new ClientWebSocket();

			await clientSocket1.ConnectAsync(utils.serverUrl, cts.Token);
			await clientSocket2.ConnectAsync(utils.serverUrl, cts.Token);

			var findMsg = new FindChessGameMessage() { ChessGame = true };

			await utils.SendThroughSocketAsync(clientSocket1, findMsg, cts.Token);
			await utils.SendThroughSocketAsync(clientSocket2, findMsg, cts.Token);

			var gameFoundMsg1 = await utils.ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket1);
			var gameFoundMsg2 = await utils.ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket2);

			await utils.ReceiveFromSocketAsync<dynamic>(clientSocket1);
			await utils.ReceiveFromSocketAsync<dynamic>(clientSocket2);

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

			await utils.SendThroughSocketAsync(firstPlayerSocket, makeMoveMsg, cts.Token);

			var moveResultMessage1 = await utils.ReceiveFromSocketAsync<ChessPlayResultMessage>(
				firstPlayerSocket);
			var moveResultMessage2 = await utils.ReceiveFromSocketAsync<ChessPlayResultMessage>(
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

			await clientSocket1.ConnectAsync(utils.serverUrl, cts.Token);
			await clientSocket2.ConnectAsync(utils.serverUrl, cts.Token);

			var findMsg = new FindChessGameMessage() { ChessGame = true };

			await utils.SendThroughSocketAsync(clientSocket1, findMsg, cts.Token);
			await utils.SendThroughSocketAsync(clientSocket2, findMsg, cts.Token);

			var gameFoundMsg1 = await utils.ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket1);
			var gameFoundMsg2 = await utils.ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket2);

			await utils.ReceiveFromSocketAsync<dynamic>(clientSocket1);
			await utils.ReceiveFromSocketAsync<dynamic>(clientSocket2);

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

			await utils.SendThroughSocketAsync(secondPlayerSocket, makeMoveMsg, cts.Token);

			var chessPlayMessage = await utils.ReceiveFromSocketAsync<ChessPlayResultMessage>(
				secondPlayerSocket);
			Assert.ThrowsAsync<TaskCanceledException>(async () =>
				await utils.ReceiveFromSocketAsync<ChessPlayResultMessage>(firstPlayerSocket));

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

			await clientSocket1.ConnectAsync(utils.serverUrl, cts.Token);
			await clientSocket2.ConnectAsync(utils.serverUrl, cts.Token);

			var findMsg = new FindChessGameMessage() { ChessGame = true };

			await utils.SendThroughSocketAsync(clientSocket1, findMsg, cts.Token);
			await utils.SendThroughSocketAsync(clientSocket2, findMsg, cts.Token);

			var gameFoundMsg1 = await utils.ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket1);
			var gameFoundMsg2 = await utils.ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket2);

			await utils.ReceiveFromSocketAsync<dynamic>(clientSocket1);
			await utils.ReceiveFromSocketAsync<dynamic>(clientSocket2);

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
				await utils.SendThroughSocketAsync(currentPlayerSocket, moveMsg, cts.Token);
				resultMessage1 =
					await utils.ReceiveFromSocketAsync<ChessPlayResultMessage>(currentPlayerSocket);
				resultMessage2 =
					await utils.ReceiveFromSocketAsync<ChessPlayResultMessage>(nextPlayerSocket);

				await utils.ReceiveFromSocketAsync<dynamic>(currentPlayerSocket);
				await utils.ReceiveFromSocketAsync<dynamic>(nextPlayerSocket);

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

			await clientSocket1.ConnectAsync(utils.serverUrl, cts.Token);
			await clientSocket2.ConnectAsync(utils.serverUrl, cts.Token);

			var findMsg = new FindChessGameMessage() { ChessGame = true };

			await utils.SendThroughSocketAsync(clientSocket1, findMsg, cts.Token);
			await utils.SendThroughSocketAsync(clientSocket2, findMsg, cts.Token);

			var gameFoundMsg1 = await utils.ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket1);
			var gameFoundMsg2 = await utils.ReceiveFromSocketAsync<GameFoundMessage>(
				clientSocket2);

			await utils.ReceiveFromSocketAsync<dynamic>(clientSocket1);
			await utils.ReceiveFromSocketAsync<dynamic>(clientSocket2);

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
				await utils.SendThroughSocketAsync(currentPlayerSocket, moveMsg, cts.Token);
				await utils.ReceiveFromSocketAsync<ChessPlayResultMessage>(currentPlayerSocket);
				await utils.ReceiveFromSocketAsync<ChessPlayResultMessage>(nextPlayerSocket);

				await utils.ReceiveFromSocketAsync<dynamic>(currentPlayerSocket);
				await utils.ReceiveFromSocketAsync<dynamic>(nextPlayerSocket);

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

			await utils.SendThroughSocketAsync(currentPlayerSocket, moveBeforePromotionMsg, cts.Token);
			var resultMessage =
				await utils.ReceiveFromSocketAsync<ChessPlayResultMessage>(currentPlayerSocket);
			Assert.AreEqual(PlayResult.PromotionRequired.ToString(), resultMessage.Message);

			var promotionMoveMsg = new PawnPromotionMessage()
			{
				PromotionPiece = "Queen"
			};

			await utils.SendThroughSocketAsync(currentPlayerSocket, promotionMoveMsg, cts.Token);
			await utils.ReceiveFromSocketAsync<ChessPlayResultMessage>(currentPlayerSocket);
			await utils.ReceiveFromSocketAsync<ChessPlayResultMessage>(nextPlayerSocket);

			await utils.ReceiveFromSocketAsync<dynamic>(currentPlayerSocket);
			await utils.ReceiveFromSocketAsync<dynamic>(nextPlayerSocket);

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

			var findMsg = new FindChessGameMessage() { ChessGame = true };

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

			await utils.ReceiveFromSocketAsync<dynamic>(clientSocket1);
			await utils.ReceiveFromSocketAsync<dynamic>(clientSocket2);
			await utils.ReceiveFromSocketAsync<dynamic>(clientSocket3);
			await utils.ReceiveFromSocketAsync<dynamic>(clientSocket4);

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

			await utils.SendThroughSocketAsync(firstPlayerSocketOfSecondSession,
				chessMoveMsg, cts.Token);

			var moveResultMsg1 = await utils.ReceiveFromSocketAsync<ChessPlayResultMessage>(
				firstPlayerSocketOfSecondSession);
			var moveResultMsg2 = await utils.ReceiveFromSocketAsync<ChessPlayResultMessage>(
				secondPlayerSocketOfSecondSession);

			await utils.SendThroughSocketAsync(firstPlayerSocketOfFirstSession,
				chessMoveMsg, cts.Token);

			var moveResultMsg3 = await utils.ReceiveFromSocketAsync<ChessPlayResultMessage>(
				firstPlayerSocketOfFirstSession);
			var moveResultMsg4 = await utils.ReceiveFromSocketAsync<ChessPlayResultMessage>(
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

			await clientSocket1.ConnectAsync(utils.serverUrl, cts.Token);
			await clientSocket2.ConnectAsync(utils.serverUrl, cts.Token);

			var findMsg = new FindChessGameMessage() { ChessGame = true };

			await utils.SendThroughSocketAsync(clientSocket1, findMsg, cts.Token);
			await utils.SendThroughSocketAsync(clientSocket2, findMsg, cts.Token);

			await utils.ReceiveFromSocketAsync<GameFoundMessage>(clientSocket1);
			await utils.ReceiveFromSocketAsync<GameFoundMessage>(clientSocket2);
			await utils.ReceiveFromSocketAsync<dynamic>(clientSocket1);
			await utils.ReceiveFromSocketAsync<dynamic>(clientSocket2);

			var cancelSessionMsg = new CancelSessionMessage();

			await utils.SendThroughSocketAsync(clientSocket1, cancelSessionMsg, cts.Token);

			await utils.ReceiveFromSocketAsync<SessionClosedMessage>(clientSocket2);

			var status = WebSocketCloseStatus.NormalClosure;
			await clientSocket1.CloseAsync(status, "", cts.Token);
			await clientSocket2.CloseAsync(status, "", cts.Token);
		}
	}
}