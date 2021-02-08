using System.ComponentModel.DataAnnotations;

namespace RisikoOnline.Api
{
    public class PostInvitationRequest
    {
        [Required] public string Receiver { get; set; }
    }
}