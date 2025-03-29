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
        public string Id { get; set; }

        public string Nom { get; set; } = null!;

        public StationPersonnelStatus StationPersonnelStatus { get; set; }

        // Stockage en base de données
        public string PositionString { get; set; }

        // Propriété calculée pour l'utilisation dans le code
        [NotMapped]
        public Point Position
        {
            get
            {
                if (string.IsNullOrEmpty(PositionString)) return null;
                var coords = PositionString.Split(',');
                if (coords.Length != 2) return null;
                if (double.TryParse(coords[0], out double x) && double.TryParse(coords[1], out double y))
                {
                    return new Point(x, y) { SRID = 4326 };
                }
                return null;
            }
            set
            {
                if (value == null)
                    PositionString = null;
                else
                    PositionString = $"{value.X},{value.Y}";
            }
        }

        public Planeau? planeau { get; set; }

        public bool PeutDecontaminer { get; set; } = false;
        public bool HautePression { get; set; } = false;
        public bool BassePressionetAttaches { get; set; } = false;
        public bool EauChaude { get; set; } = false;
    }
}
