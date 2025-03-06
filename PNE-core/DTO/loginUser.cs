using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;

namespace PNE_core.DTO
{
    public class LoginDto
    {
        [Required, EmailAddress]
        [Display(Name = "Adresse Email")]
        public required string Email { get; set; }
        [Required]
        [Display(Name = "Mot de passe")]
        public required string Password { get; set; }
    }
}
