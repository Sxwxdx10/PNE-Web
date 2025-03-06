using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PNE_core.Models;

[Table("Utilisateur")]
public partial class Utilisateur
{
    [Key]
    [Column("id")]
    public string Id { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string DisplayName { get; set; } = null!;
    [Display(Name = "Date de creation")]
    public DateTime DateCreation { get; set; }

    [InverseProperty("UtilisateurNavigation")]
    public virtual ICollection<Embarcationutilisateur> Embarcationutilisateurs { get; set; } = new List<Embarcationutilisateur>();

    [InverseProperty("Utilisateur")]
    public virtual ICollection<CertificationUtilisateur> CertificationUtilisateur { get; set; } = new List<CertificationUtilisateur>();

    public virtual ICollection<EmployePlaneau> EmployePlaneau { get; set; } = new List<EmployePlaneau>();

    [InverseProperty("Utilisateur")]
    public virtual ICollection<RolesUtilisateurs> RolesUtilisateurs { get; set; } = new HashSet<RolesUtilisateurs>();
}