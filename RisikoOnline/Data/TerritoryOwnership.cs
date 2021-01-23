using Microsoft.EntityFrameworkCore;

namespace RisikoOnline.Data
{
    public class TerritoryOwnership
    {
        public int MatchId { get; set; }
        public Match Match { get; set; }
        
        public string PlayerName { get; set; }
        public Player Player { get; set; }
        
        public PlayerState PlayerState { get; set; }
        
        public Territory Territory { get; set; }
        public int Armies { get; set; }

        public static void Configure(ModelBuilder builder)
        {
            var territoryOwnershipEntity = builder.Entity<TerritoryOwnership>();

            territoryOwnershipEntity
                .HasKey(to => new {to.MatchId, to.PlayerName, to.Territory});

            territoryOwnershipEntity
                .Property(to => to.Territory)
                .IsRequired();

            territoryOwnershipEntity
                .HasOne(to => to.Match)
                .WithMany(m => m.Ownerships)
                .HasForeignKey(to => to.MatchId)
                .IsRequired();

            territoryOwnershipEntity
                .HasOne(to => to.Player)
                .WithMany()
                .HasForeignKey(to => to.PlayerName)
                .IsRequired();

            territoryOwnershipEntity
                .HasOne(to => to.PlayerState)
                .WithMany(ps => ps.Ownerships)
                .HasForeignKey(to => new {to.MatchId, to.PlayerName})
                .IsRequired();
        }
    }
}
