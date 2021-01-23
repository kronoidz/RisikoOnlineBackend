using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RisikoOnline.Data
{
    public class PlayerState
    {
        public int MatchId { get; set; }
        public Match Match { get; set; }
        
        public string PlayerName { get; set; }
        public Player Player { get; set; }
        
        public List<TerritoryOwnership> Ownerships { get; set; }

        // At the beginning of a match all clients have to submit a valid initial
        // configuration of armies over the assigned territories
        public bool IsInitialized { get; set; }
        
        public MissionObjective MissionObjective { get; set; }
        
        // This relationship is only meaningful if mission objective is DestroyEnemy
        public string TargetPlayerName { get; set; }
        public PlayerState TargetPlayer { get; set; }
        
        public int ReinforcementPoints { get; set; }
        public int UnplacedArmies { get; set; }

        public static void Configure(ModelBuilder builder)
        {
            var playerStateEntity = builder.Entity<PlayerState>();

            playerStateEntity.HasKey(ps => new {ps.MatchId, ps.PlayerName});
            playerStateEntity.Property(ps => ps.IsInitialized).HasDefaultValue(false);

            playerStateEntity
                .HasOne(ps => ps.Match)
                .WithMany(m => m.PlayerStates)
                .HasForeignKey(ps => ps.MatchId)
                .IsRequired();
            
            playerStateEntity
                .HasOne(ps => ps.Player)
                .WithMany(p => p.CurrentMatchStates)
                .HasForeignKey(ps => ps.PlayerName)
                .IsRequired();

            playerStateEntity
                .HasOne(ps => ps.TargetPlayer)
                .WithOne()
                .HasForeignKey<PlayerState>(ps => new {ps.MatchId, ps.TargetPlayerName})
                .IsRequired(false);
        }
    }
}