using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PNE_core.Models;

public partial class Embarcation
{
    public string IdEmbarcation { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Marque { get; set; } = null!;

    public int Longueur { get; set; }

    public string Photo { get; set; } = null!;

    public string codeQR { get; set; }

    [InverseProperty("IdEmbarcationNavigation")]
    public virtual ICollection<Embarcationutilisateur> Embarcationutilisateurs { get; set; } = new List<Embarcationutilisateur>();

    [InverseProperty("EmbarcationNavigation")]
    public virtual ICollection<Lavage> Lavages { get; set; } = new List<Lavage>();

    [InverseProperty("EmbarcationNavigation")]
    public virtual ICollection<Miseaeau> Miseaeaus { get; set; } = new List<Miseaeau>();

    public virtual ICollection<Notedossier> Notedossiers { get; set; } = new List<Notedossier>();
}
