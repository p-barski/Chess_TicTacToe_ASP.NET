using System;
using System.Text;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Server;

namespace ServerTests
{
	public class TestUtils
	{
		public readonly Uri serverUrl = new Uri("ws://localhost:5000");
		private IHost host;
		private Thread serverThread;
		private int timeoutMiliseconds = 500;
		public async Task Setup()
		{
			host = Program.CreateHostBuilder(new string[0]).Build();
			serverThread = new Thread(() => host.Start());
			serverThread.Start();
			await Task.Delay(1000);
		}
		public async Task TearDown()
		{
			await host.StopAsync();
			serverThread.Join();
		}
		public async Task<T> ReceiveFromSocketAsync<T>(WebSocket socket)
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
		public async Task SendThroughSocketAsync<T>(WebSocket socket, T msgObject,
			CancellationToken token)
		{
			var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msgObject));
			await socket.SendAsync(buffer, WebSocketMessageType.Text, true, token);
			await Task.Delay(5);
		}
	}
}