using System;
using Server.Sockets;
using TicTacToe;

namespace Server.Games
{
	public interface IPlayer
	{
		Guid GUID { get; }
		IWebSocket Socket { get; }
		IPlayerType PlayerType { get; }
		int ExpectedBoardSize { get; }
		PlayerState State { get; }
		Guid GameSessionGUID { get; }
		void AddToGame(Guid gameGUID, IPlayerType playerType);
		void RemoveFromGame();
		void SetAsSearchingForGame(int size);
	}
}