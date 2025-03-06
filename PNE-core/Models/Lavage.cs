using PNE_core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PNE_core.Models;

public partial class Lavage
{
    public string? IdEmbarcation { get; set; }

    public DateTime Date { get; set; }

    public bool SelfServe { get; set; }

    public string IdLavage { get; set; } = null!;

    public virtual TypeLavage TypeLavage { get; set; }

    [ForeignKey("IdEmbarcation")]
    public virtual Embarcation? EmbarcationNavigation { get; set; }
}
