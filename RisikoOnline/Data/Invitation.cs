using Microsoft.EntityFrameworkCore;

namespace RisikoOnline.Data
{
    public class Invitation
    {
        public int Id { get; set; }

        public string SenderName { get; set; }
        public Player Sender { get; set; }

        public string ReceiverName { get; set; }
        public Player Receiver { get; set; }

        public bool? Accepted { get; set; }

        public static void Configure(ModelBuilder builder)
        {
            var invitationEntity = builder.Entity<Invitation>();
            
            invitationEntity.HasKey(i => i.Id);

            invitationEntity
                .HasOne(i => i.Sender)
                .WithMany(p => p.OutgoingInvitations)
                .HasForeignKey(i => i.SenderName)
                .IsRequired();
            
            invitationEntity
                .HasOne(i => i.Receiver)
                .WithMany(p => p.IncomingInvitations)
                .HasForeignKey(i => i.ReceiverName)
                .IsRequired();
        }
    }
}
