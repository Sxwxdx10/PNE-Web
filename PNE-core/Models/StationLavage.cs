using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PNE_core.Enums;

namespace PNE_core.Models
{
    public class StationLavage
    {
        [Key]
        public string? Id { get; set; }

        public string Nom { get; set; } = null!;

        public Point? Position { get; set; }

        public Planeau? planeau { get; set; }

        public bool? PeutDecontaminer { get; set; }
        public bool? HautePression { get; set; }
        public bool? BassePressionetAttaches { get; set; }
        public bool? EauChaude { get; set; }

        public StationPersonnelStatus StationPersonnelStatus { get; set; }
    }
}
