using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PNE_core.Models;

public partial class Certification
{
    [Key]
    public string CodeCertification { get; set; } = null!;

    public string NomFormation { get; set; } = null!;

    [InverseProperty("certification")]
    public virtual ICollection<CertificationUtilisateur> CertificationUtilisateur { get; set; } = new List<CertificationUtilisateur>();
}