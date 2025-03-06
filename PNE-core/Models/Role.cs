using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PNE_core.Models;

public partial class Role
{
    [Key]
    public string NomRole { get; set; } = null!;

    public string Description { get; set; } = null!;

    [InverseProperty("Role")]
    public virtual ICollection<RolesUtilisateurs> RolesUtilisateurs { get; set; } = new HashSet<RolesUtilisateurs>();
}
