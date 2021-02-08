using System.ComponentModel.DataAnnotations;

namespace RisikoOnline.Api
{
    public class Credentials
    {
        [Required] public string Name { get; set; }
        [Required] public string Password { get; set; }
    }
}