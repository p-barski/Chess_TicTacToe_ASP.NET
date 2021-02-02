using System;
using System.Net.WebSockets;
using TicTacToe;

namespace Server.TicTacToe
{
	public interface IPlayer
	{
		Guid GUID { get; }
		WebSocket Socket { get; }
		XO_Enum Sign { get; }
		int ExpectedBoardSize { get; }
		PlayerState State { get; }
		Guid GameSessionGUID { get; }
		void AddToGame(Guid gameGUID, XO_Enum sign);
		void RemoveFromGame();
		void SetAsSearchingForGame(int size);
	}
}