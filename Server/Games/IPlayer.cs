using System;
using Server.Sockets;
using TicTacToe;

namespace Server.Games
{
	public interface IPlayer
	{
		Guid GUID { get; }
		IWebSocket Socket { get; }
		XO_Enum Sign { get; }
		int ExpectedBoardSize { get; }
		PlayerState State { get; }
		Guid GameSessionGUID { get; }
		void AddToGame(Guid gameGUID, XO_Enum sign);
		void RemoveFromGame();
		void SetAsSearchingForGame(int size);
	}
}