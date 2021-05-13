using System;

namespace Server.Games
{
	public class PlayerType<T> : IPlayerType where T : Enum
	{
		public string StringRepresentation { get => type.ToString(); }
		private T type;
		public PlayerType(T type)
		{
			this.type = type;
		}
	}
}