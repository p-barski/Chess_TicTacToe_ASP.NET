using System;

namespace Server.Database
{
	public class EnvironmentVariableGetter
	{
		public static string GetConnectionStringEnvironmentVar()
		{
			return Environment.GetEnvironmentVariable("DATABASE_URL");
		}
		public static bool GetWhetherToUsePostgresEnvironmentVar()
		{
			return Environment.GetEnvironmentVariable("USE_POSTGRES") == "true";
		}
	}
}