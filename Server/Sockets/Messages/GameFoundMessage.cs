namespace Server.Sockets.Messages
{
	public class GameFoundMessage : ISendMessage
	{
		public bool IsClientTurn { get; set; }
		public GameFoundMessage() { }
		public GameFoundMessage(bool turn)
		{
			IsClientTurn = turn;
		}
	}
}