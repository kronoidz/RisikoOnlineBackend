using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace RisikoOnline.Entities
{
    public class Player
    {
        [Key] public string Name { get; set; }
        [Required] public string PasswordHash { get; set; }
        [Required] public string PasswordSalt { get; set; }

        public static void Configure(ModelBuilder builder)
        {
            builder.Entity<Player>();
        }
    }
}
