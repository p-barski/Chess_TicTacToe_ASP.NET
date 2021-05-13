using System;

namespace Server.Games
{
	public interface IGameSession
	{
		Guid GUID { get; }
		IPlayer PlayerOne { get; }
		IPlayer PlayerTwo { get; }
		PlayResult Play(IPlayer from, IGameMove gameMove);
		void Close();
	}
}