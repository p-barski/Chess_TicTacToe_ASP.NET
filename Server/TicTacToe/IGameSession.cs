using System;

namespace Server.TicTacToe
{
	public interface IGameSession
	{
		Guid GUID { get; }
		IPlayer PlayerOne { get; }
		IPlayer PlayerTwo { get; }
		PlayResult Play(IPlayer from, int x, int y);
		void Close();
	}
}