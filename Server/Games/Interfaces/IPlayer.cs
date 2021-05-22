using System;
using Server.Sockets;
using Server.Database;

namespace Server.Games
{
	public interface IPlayer
	{
		Guid GUID { get; }
		IWebSocket Socket { get; }
		PlayerData PlayerData { get; set; }
		IPlayerType PlayerType { get; }
		IExpectedGame ExpectedGame { get; }
		PlayerState State { get; }
		Guid GameSessionGUID { get; }
		void AddToGame(Guid gameGUID, IPlayerType playerType);
		void RemoveFromGame();
		void SetAsSearchingForGame(IExpectedGame expectedGame);
	}
}