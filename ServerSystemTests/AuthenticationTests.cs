using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Server.Sockets.Messages;

namespace ServerTests
{
	public class AuthenticationTests
	{
		private TestUtils utils;
		[SetUp]
		public async Task Setup()
		{
			utils = new TestUtils();
			utils.DeleteSqliteDatabase();
			await utils.Setup();
		}
		[TearDown]
		public async Task TearDown()
		{
			await utils.TearDown();
		}
		[Test]
		public async Task TryingToLogInAsAnonymousReturnsErrorMessage()
		{
			var cts = new CancellationTokenSource();

			var clientSocket = new ClientWebSocket();

			await clientSocket.ConnectAsync(utils.serverUrl, cts.Token);

			var msg = new AuthenticationMessage()
			{
				Registration = false,
				Username = "Anonymous",
				Password = "pass"
			};

			await utils.SendThroughSocketAsync(clientSocket, msg, cts.Token);
			var result =
				await utils.ReceiveFromSocketAsync<AuthenticationResultMessage>(clientSocket);

			Assert.IsFalse(result.IsSuccess);
			Assert.AreNotEqual("", result.ErrorMessage);

			var status = WebSocketCloseStatus.NormalClosure;
			await clientSocket.CloseAsync(status, "", cts.Token);
		}
		[Test]
		public async Task TryingToRegisterAsAnonymousReturnsErrorMessage()
		{
			var cts = new CancellationTokenSource();

			var clientSocket = new ClientWebSocket();

			await clientSocket.ConnectAsync(utils.serverUrl, cts.Token);

			var msg = new AuthenticationMessage()
			{
				Registration = true,
				Username = "Anonymous",
				Password = "pass1"
			};

			await utils.SendThroughSocketAsync(clientSocket, msg, cts.Token);
			var result =
				await utils.ReceiveFromSocketAsync<AuthenticationResultMessage>(clientSocket);

			Assert.IsFalse(result.IsSuccess);
			Assert.AreNotEqual("", result.ErrorMessage);

			var status = WebSocketCloseStatus.NormalClosure;
			await clientSocket.CloseAsync(status, "", cts.Token);
		}
		[Test]
		public async Task RegisteringAccountIsSuccessful()
		{
			var cts = new CancellationTokenSource();

			var clientSocket = new ClientWebSocket();

			await clientSocket.ConnectAsync(utils.serverUrl, cts.Token);

			var msg = new AuthenticationMessage()
			{
				Registration = true,
				Username = "random",
				Password = "pass"
			};

			await utils.SendThroughSocketAsync(clientSocket, msg, cts.Token);
			var result =
				await utils.ReceiveFromSocketAsync<AuthenticationResultMessage>(clientSocket);

			Assert.IsTrue(result.IsSuccess);

			var status = WebSocketCloseStatus.NormalClosure;
			await clientSocket.CloseAsync(status, "", cts.Token);
		}
		[Test]
		public async Task RegisteringAccountAndLoggingInIsSuccesful()
		{
			var cts = new CancellationTokenSource();

			var clientSocket = new ClientWebSocket();

			await clientSocket.ConnectAsync(utils.serverUrl, cts.Token);

			var msg = new AuthenticationMessage()
			{
				Registration = true,
				Username = "random",
				Password = "pass"
			};

			await utils.SendThroughSocketAsync(clientSocket, msg, cts.Token);
			var result =
				await utils.ReceiveFromSocketAsync<AuthenticationResultMessage>(clientSocket);

			var status = WebSocketCloseStatus.NormalClosure;
			await clientSocket.CloseAsync(status, "", cts.Token);

			var clientSocket2 = new ClientWebSocket();
			await clientSocket2.ConnectAsync(utils.serverUrl, cts.Token);

			msg.Registration = false;

			await utils.SendThroughSocketAsync(clientSocket2, msg, cts.Token);
			result =
				await utils.ReceiveFromSocketAsync<AuthenticationResultMessage>(clientSocket2);

			Assert.IsTrue(result.IsSuccess);

			await clientSocket2.CloseAsync(status, "", cts.Token);
		}
		[Test]
		public async Task TryingToLogInWithIncorrectPasswordReturnsErrorMessage()
		{
			var cts = new CancellationTokenSource();

			var clientSocket = new ClientWebSocket();

			await clientSocket.ConnectAsync(utils.serverUrl, cts.Token);

			var msg = new AuthenticationMessage()
			{
				Registration = true,
				Username = "testusername",
				Password = "correctpassword"
			};

			await utils.SendThroughSocketAsync(clientSocket, msg, cts.Token);
			var result =
				await utils.ReceiveFromSocketAsync<AuthenticationResultMessage>(clientSocket);

			var status = WebSocketCloseStatus.NormalClosure;
			await clientSocket.CloseAsync(status, "", cts.Token);

			var clientSocket2 = new ClientWebSocket();
			await clientSocket2.ConnectAsync(utils.serverUrl, cts.Token);

			msg.Registration = false;
			msg.Password = "wrongpassword";

			await utils.SendThroughSocketAsync(clientSocket2, msg, cts.Token);
			result =
				await utils.ReceiveFromSocketAsync<AuthenticationResultMessage>(clientSocket2);

			Assert.IsFalse(result.IsSuccess);
			Assert.AreNotEqual("", result.ErrorMessage);

			await clientSocket2.CloseAsync(status, "", cts.Token);
		}
		[Test]
		public async Task TryingToLogInWhenThereIsNoAccountCreatedReturnsErrorMessage()
		{
			var cts = new CancellationTokenSource();

			var clientSocket = new ClientWebSocket();

			await clientSocket.ConnectAsync(utils.serverUrl, cts.Token);

			var msg = new AuthenticationMessage()
			{
				Registration = false,
				Username = "heyo",
				Password = "pswd"
			};

			await utils.SendThroughSocketAsync(clientSocket, msg, cts.Token);
			var result =
				await utils.ReceiveFromSocketAsync<AuthenticationResultMessage>(clientSocket);

			Assert.IsFalse(result.IsSuccess);
			Assert.AreNotEqual("", result.ErrorMessage);

			var status = WebSocketCloseStatus.NormalClosure;
			await clientSocket.CloseAsync(status, "", cts.Token);
		}
	}
}