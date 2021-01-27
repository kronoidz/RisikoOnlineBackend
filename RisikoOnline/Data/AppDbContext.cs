using Microsoft.EntityFrameworkCore;


namespace RisikoOnline.Data
{
    public class AppDbContext : DbContext
    {
        private const string ConnectionString = "Data Source=dev.db";

        public DbSet<Player> Players { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<PlayerState> PlayerStates { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Player.Configure(modelBuilder);
            Invitation.Configure(modelBuilder);
            MatchHistoryRecord.Configure(modelBuilder);
            TerritoryOwnership.Configure(modelBuilder);
            PlayerState.Configure(modelBuilder);
            Match.Configure(modelBuilder);
        }
    }
}
