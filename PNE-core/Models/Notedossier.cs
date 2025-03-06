using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PNE_core.Models;

public partial class Notedossier
{
    public string? IdEmbarcation { get; set; }

    public string? IdPlanEau { get; set; }

    public DateTime Date { get; set; }

    public string Note { get; set; } = null!;

    public string? Idnote { get; set; } = null!;

    [InverseProperty("Notedossiers")]
    [ForeignKey("IdEmbarcation")]
    public virtual Embarcation? IdEmbarcationNavigation { get; set; }

    [InverseProperty("Notedossiers")]
    [ForeignKey("IdPlanEau")]
    public virtual Planeau? IdPlanEauNavigation { get; set; }
}