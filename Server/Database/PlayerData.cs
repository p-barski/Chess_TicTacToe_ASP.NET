namespace Server.Database
{
	public class PlayerData
	{
		public int Id { get; private set; }
		public string Name { get; private set; }
		public string Password { get; private set; }
		public PlayerData() { }
		public PlayerData(string name, string password)
		{
			Name = name;
			Password = password;
		}
	}
}