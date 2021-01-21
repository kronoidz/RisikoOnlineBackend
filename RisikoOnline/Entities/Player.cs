using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RisikoOnline.Entities
{
    public class Player
    {
        [Key] public string Name { get; set; }
        [Required] public string PasswordHash { get; set; }
        [Required] public string PasswordSalt { get; set; }

        [InverseProperty("Sender")]
        public List<Invitation> OutgoingInvitations { get; set; }
        
        [InverseProperty("Receiver")]
        public List<Invitation> IncomingInvitations { get; set; }
    }
}
