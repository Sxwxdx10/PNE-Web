using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PNE_core.Models;

public partial class Embarcationutilisateur
{
    [Key]
    [Column("id_embarcation_utilisateur")]
    public string IdEmbarcationUtilisateur { get; set; } = null!;

    public string? IdEmbarcation { get; set; }
    public string? IdUtilisateur { get; set; }

    [ForeignKey("IdEmbarcation")]
    [InverseProperty("Embarcationutilisateurs")]
    public virtual Embarcation? IdEmbarcationNavigation { get; set; }

    [ForeignKey("IdUtilisateur")]
    [InverseProperty("Embarcationutilisateurs")]
    public virtual Utilisateur? UtilisateurNavigation { get; set; }
}
