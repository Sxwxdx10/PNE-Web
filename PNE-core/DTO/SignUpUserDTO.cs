using System.ComponentModel.DataAnnotations;

namespace PNE_core.DTO
{
    public class SignUpUserDTO
    {
        [Required]
        [Display(Name = "Nom d'utilisateur")]
        public required string Username { get; set; }

        [Required, EmailAddress]
        [Display(Name = "Adresse Email")]
        public required string Email { get; set; }

        [Required]
        [Display(Name = "Mot de passe")]
        public required string Password { get; set; }

        [Required, Compare(nameof(Password), ErrorMessage = "Les mots de passes ne sont pas identiques")]
        [Display(Name = "Confirmer votre mot de passe")]
        public required string ConfirmPassword { get; set; }
    }
}
