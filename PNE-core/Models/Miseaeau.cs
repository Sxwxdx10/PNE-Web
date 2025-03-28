using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PNE_core.Models;

public partial class Miseaeau
{
    [Key]
    public string IdMiseEau { get; set; } = null!;

    public DateTime Date { get; set; }

    public int? DureeSejourEnJours { get; set; } 

    public string? IdPlanEau { get; set; }

    public string IdEmbarcation { get; set; } = null!;

    [InverseProperty("Miseaeaus")]
    [ForeignKey("IdEmbarcation")]
    public virtual Embarcation? EmbarcationNavigation { get; set; }

    [InverseProperty("Miseaeaus")]
    [ForeignKey("IdPlanEau")]
    public virtual Planeau? IdPlanEauNavigation { get; set; }
}