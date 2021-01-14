using Microsoft.EntityFrameworkCore;
using RisikoOnline.Entities;

namespace RisikoOnline
{
    public class AppDbContext : DbContext
    {
        private const string ConnectionString = "Data Source=dev.db";

        public DbSet<Player> Players { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Player.Configure(modelBuilder);
        }
    }
}
