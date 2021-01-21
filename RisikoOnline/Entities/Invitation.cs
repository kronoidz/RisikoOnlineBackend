using System.ComponentModel.DataAnnotations;

namespace RisikoOnline.Entities
{
    public class Invitation
    {
        public int Id { get; set; }

        public string SenderName { get; set; }
        public Player Sender { get; set; }

        public string ReceiverName { get; set; }
        public Player Receiver { get; set; }

        public bool? Accepted { get; set; }
    }
}
