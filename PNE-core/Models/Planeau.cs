using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;
using PNE_core.Enums;

namespace PNE_core.Models;

public partial class Planeau
{
    public string? IdPlanEau { get; set; }

    public Point? Emplacement { get; set; }

    public string Nom { get; set; } = null!;

    public virtual Niveau? NiveauCouleur { get; set; }

    [InverseProperty("IdPlanEauNavigation")]
    public virtual ICollection<Miseaeau>? Miseaeaus { get; set; }

    [InverseProperty("IdPlanEauNavigation")]
    public virtual ICollection<Notedossier>? Notedossiers { get; set; }

    [InverseProperty("Planeau")]
    public virtual ICollection<EmployePlaneau> EmployePlaneau { get; set; } = new List<EmployePlaneau>();
    
    [InverseProperty("PlanEauNavigation")]
    public virtual ICollection<EEEPlanEau>? EEEPlanEau { get; set; }
}
