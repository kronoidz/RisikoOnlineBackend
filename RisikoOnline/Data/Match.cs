using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RisikoOnline.Data
{
    public class Match
    {
        public int Id { get; set; }

        public List<PlayerState> PlayerStates { get; set; }

        // Initialized only when PlayerState.IsInitialized == true for all PlayerStates
        public string CurrentPlayerName { get; set; }
        public PlayerState CurrentPlayerState { get; set; }
        
        public List<TerritoryOwnership> Ownerships { get; set; }

        public static void Configure(ModelBuilder builder)
        {
            var matchEntity = builder.Entity<Match>();

            matchEntity.HasKey(m => m.Id);
            matchEntity.Property(m => m.Id).ValueGeneratedOnAdd();

            matchEntity
                .HasOne(m => m.CurrentPlayerState)
                .WithOne()
                .HasForeignKey<Match>(m => new {m.Id, m.CurrentPlayerName})
                .IsRequired(false);
        }
    }
}