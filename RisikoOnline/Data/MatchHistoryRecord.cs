using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RisikoOnline.Data
{
    public class MatchHistoryRecord
    {
        public int Id { get; set; }

        public string PlayerName { get; set; }
        public Player Player { get; set; }
        
        public bool IsWinner { get; set; }

        public static void Configure(ModelBuilder builder)
        {
            var entity = builder.Entity<MatchHistoryRecord>();
            
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.PlayerName);
            
            entity
                .HasOne(e => e.Player)
                .WithMany(p => p.History)
                .HasForeignKey(e => e.PlayerName)
                .IsRequired();
        }
    }
}