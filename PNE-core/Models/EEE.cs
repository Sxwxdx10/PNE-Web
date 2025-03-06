using PNE_core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNE_core.Models
{
    public class EEE
    {
        [Key]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Niveau NiveauCouleur { get; set; }

        [InverseProperty("EEE")]
        public virtual ICollection<EEEPlanEau> EEEPlanEau { get; set; }
    }
}
