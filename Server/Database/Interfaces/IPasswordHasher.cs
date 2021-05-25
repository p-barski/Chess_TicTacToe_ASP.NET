namespace Server.Database
{
	public interface IPasswordHasher
	{
		string HashPassword(string password);
		bool IsPasswordCorrect(string password, string hashedPassword);
	}
}