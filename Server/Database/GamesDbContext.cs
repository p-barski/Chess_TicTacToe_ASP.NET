using Microsoft.EntityFrameworkCore;
using Server.Database.Chess;

namespace Server.Database
{
	public class GamesDbContext : DbContext
	{
		public DbSet<ChessGameDb> ChessGames { get; private set; }
		public DbSet<ChessMoveDb> ChessMoves { get; private set; }
		public DbSet<PlayerData> PlayerDatas { get; private set; }
		private string connectionString;
		private bool usePostgres;
		public GamesDbContext(string connectionString, bool usePostgres = false)
		{
			this.connectionString = connectionString;
			this.usePostgres = usePostgres;
		}
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (usePostgres)
			{
				optionsBuilder.UseNpgsql(connectionString);
				return;
			}
			optionsBuilder.UseSqlite(connectionString);
		}
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<ChessGameDb>().ToTable("ChessGame");
			modelBuilder.Entity<ChessMoveDb>().ToTable("ChessMove");
			modelBuilder.Entity<PlayerData>().ToTable("PlayerData")
				.HasIndex(p => p.Name)
				.IsUnique();
		}
	}
}