using RisikoOnline.Data;

namespace RisikoOnline.Api
{
    public class InvitationDto
    {
        public int Id { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public bool? Accepted { get; set; }

        public InvitationDto(Invitation invitation)
        {
            Id = invitation.Id;
            Sender = invitation.SenderName;
            Receiver = invitation.ReceiverName;
            Accepted = invitation.Accepted;
        }
    }
}