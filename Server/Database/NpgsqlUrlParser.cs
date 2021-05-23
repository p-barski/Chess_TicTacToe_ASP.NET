using System;
using Npgsql;

namespace Server.Database
{
	public class NpgsqlUrlParser
	{
		//https://stackoverflow.com/questions/45907900/net-core-database-url-parser/45916910#45916910
		public static string ParseToEFCoreConnectionString(string databaseUrl)
		{
			var uri = new Uri(databaseUrl);
			var userInfo = uri.UserInfo.Split(':');

			var builder = new NpgsqlConnectionStringBuilder
			{
				Host = uri.Host,
				Port = uri.Port,
				Username = userInfo[0],
				Password = userInfo[1],
				Database = uri.LocalPath.TrimStart('/')
			};
			return builder.ToString();
		}
	}
}