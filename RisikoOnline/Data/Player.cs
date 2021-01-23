using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace RisikoOnline.Data
{
    public class Player
    {
        public string Name { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }

        public List<Invitation> OutgoingInvitations { get; set; }
        public List<Invitation> IncomingInvitations { get; set; }
        
        public List<PlayerState> CurrentMatchStates { get; set; }
        
        public List<MatchHistoryRecord> History { get; set; }

        public static void Configure(ModelBuilder builder)
        {
            var playerEntity = builder.Entity<Player>();
            
            playerEntity.HasKey(p => p.Name);
            playerEntity.Property(p => p.PasswordHash).IsRequired();
            playerEntity.Property(p => p.PasswordSalt).IsRequired();
        }
    }
}
