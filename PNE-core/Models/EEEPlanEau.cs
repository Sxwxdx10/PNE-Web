using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNE_core.Models
{
    public class EEEPlanEau
    {
        [Key]
        public string Id { get; set; }

        public string IdEEE { get; set; } = null!;
        public string IdPlanEau { get; set; } = null!;

        public bool Validated { get; set; }

        [ForeignKey("IdEEE")]
        [InverseProperty("EEEPlanEau")]
        public virtual EEE EEE { get; set; } = null!;

        [ForeignKey("IdPlanEau")]
        [InverseProperty("EEEPlanEau")]
        public virtual Planeau PlanEauNavigation { get; set; } = null!;
    }
}
