using System;
using System.Security.Cryptography;

namespace Server.Database
{
	//https://stackoverflow.com/questions/4181198/how-to-hash-a-password/10402129#10402129
	public class PasswordHasher : IPasswordHasher
	{
		private const int iterations = 100000;
		private const int saltSize = 16;
		private const int hashBytesSize = 20;
		public string HashPassword(string password)
		{
			byte[] salt;
			new RNGCryptoServiceProvider().GetBytes(salt = new byte[saltSize]);
			var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
			byte[] hash = pbkdf2.GetBytes(hashBytesSize);
			byte[] hashAndSalt = new byte[saltSize + hashBytesSize];
			Array.Copy(salt, 0, hashAndSalt, 0, saltSize);
			Array.Copy(hash, 0, hashAndSalt, saltSize, hashBytesSize);
			return Convert.ToBase64String(hashAndSalt);
		}
		public bool IsPasswordCorrect(string password, string hashedPassword)
		{
			if (hashedPassword == "")
			{
				return false;
			}
			byte[] hashAndSalt = Convert.FromBase64String(hashedPassword);
			byte[] salt = new byte[saltSize];
			Array.Copy(hashAndSalt, 0, salt, 0, saltSize);
			var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
			byte[] hash = pbkdf2.GetBytes(hashBytesSize);
			for (int i = 0; i < hashBytesSize; i++)
			{
				if (hashAndSalt[i + saltSize] != hash[i])
				{
					return false;
				}
			}
			return true;
		}
	}
}