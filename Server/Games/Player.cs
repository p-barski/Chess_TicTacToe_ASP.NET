using System;
using Server.Sockets;
using TicTacToe;

namespace Server.Games
{
	public class Player : IPlayer
	{
		public Guid GUID { get; } = Guid.NewGuid();
		public IWebSocket Socket { get; }
		public XO_Enum Sign { get; private set; }
		public int ExpectedBoardSize { get; private set; }
		public PlayerState State { get; private set; } = PlayerState.Idle;
		public Guid GameSessionGUID { get; private set; } = Guid.Empty;
		public Player(IWebSocket socket)
		{
			Socket = socket;
		}
		public void AddToGame(Guid gameGUID, XO_Enum sign)
		{
			GameSessionGUID = gameGUID;
			Sign = sign;
			State = PlayerState.Playing;
		}
		public void RemoveFromGame()
		{
			GameSessionGUID = Guid.Empty;
			State = PlayerState.Idle;
		}
		public void SetAsSearchingForGame(int size)
		{
			ExpectedBoardSize = size;
			State = PlayerState.SearchingForGame;
		}
	}
}