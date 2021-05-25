using NUnit.Framework;
using Server.Database;

namespace ServerTests
{
	public class PasswordHasherTests
	{
		[Test]
		public void HashedPasswordIsDifferentThanInputPassword()
		{
			var password = "abcdefghijk";
			var hasher = new PasswordHasher();
			var hashedPassword = hasher.HashPassword(password);

			Assert.AreNotEqual(password, hashedPassword);
		}
		[Test]
		public void HashedPasswordIs48CharactersLong()
		{
			var password = "Testpassword";
			var hasher = new PasswordHasher();
			var hashedPassword = hasher.HashPassword(password);

			Assert.AreEqual(48, hashedPassword.Length);
			Assert.IsTrue(hasher.IsPasswordCorrect(password, hashedPassword));
		}
		[Test]
		public void HashingTheSamePasswordTwiceGivesDifferentHashedPassword()
		{
			var password = "pswd";
			var hasher = new PasswordHasher();
			var hashedPassword1 = hasher.HashPassword(password);
			var hashedPassword2 = hasher.HashPassword(password);

			Assert.AreNotEqual(hashedPassword1, hashedPassword2);
		}
		[Test]
		public void IsPasswordCorrectReturnsTrueWhenPasswordIsTheSame()
		{
			var password = " !@#., m";
			var hasher = new PasswordHasher();
			var hashedPassword = hasher.HashPassword(password);

			Assert.IsTrue(hasher.IsPasswordCorrect(password, hashedPassword));
		}
		[Test]
		public void IsPasswordCorrectReturnsFalseWhenPasswordIsDifferent()
		{
			var password = "password123";
			var incorrectPassword = "passworD123";
			var hasher = new PasswordHasher();
			var hashedPassword = hasher.HashPassword(password);

			Assert.IsFalse(hasher.IsPasswordCorrect(incorrectPassword, hashedPassword));
		}
	}
}