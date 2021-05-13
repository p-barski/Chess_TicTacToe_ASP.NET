using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Server.Sockets;
using Server.Sockets.Other;
using Server.Sockets.Handlers;
using Server.Games;

namespace Server
{
	public class Startup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<IGameSessionFactory, GameSessionFactory>();
			services.AddSingleton<ICollections, Collections>();
			services.AddSingleton<IMessageDeserializer, MessageDeserializer>();
			services.AddSingleton<IMessageSender, MessageSender>();
			services.AddSingleton<IMessageHandler, FindGameHandler>();
			services.AddSingleton<IMessageHandler, MakeMoveHandler>();
			services.AddSingleton<ISocketMessageHandler, SocketMessageHandler>();
			services.AddSingleton<SocketController>();
		}
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			app.UseWebSockets();
			app.UseMiddleware<WebSocketMiddleware>();
			app.UseRouting();
			var html = File.ReadAllText("./simpleUI.html");
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapGet("/", async context =>
					await context.Response.WriteAsync(html)
				);
			});
		}
	}
}