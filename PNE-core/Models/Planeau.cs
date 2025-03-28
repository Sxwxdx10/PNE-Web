using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using PNE_core.Enums;

namespace PNE_core.Models;

public partial class Planeau
{
    [Key]
    public string IdPlanEau { get; set; }
    
    public string Nom { get; set; } = null!;

    [Column("niveau_couleur")]
    public virtual Niveau? NiveauCouleur { get; set; }

    // Stockage en base de données
    public string EmplacementString { get; set; }

    // Propriétés pour la saisie des coordonnées
    [NotMapped]
    public double? Latitude { get; set; }

    [NotMapped]
    public double? Longitude { get; set; }

    // Propriété calculée pour l'utilisation dans le code
    [NotMapped]
    public Point Emplacement
    {
        get
        {
            if (string.IsNullOrEmpty(EmplacementString)) return null;
            try
            {
                var wkbReader = new WKBReader();
                var hexString = EmplacementString.Replace("0x", "");
                var bytes = new byte[hexString.Length / 2];
                for (int i = 0; i < hexString.Length; i += 2)
                {
                    bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
                }
                var geometry = wkbReader.Read(bytes);
                return geometry as Point;
            }
            catch
            {
                // Si le format n'est pas WKB, essayer le format x,y
                var coords = EmplacementString.Split(',');
                if (coords.Length == 2 && 
                    double.TryParse(coords[0], out double x) && 
                    double.TryParse(coords[1], out double y))
                {
                    return new Point(x, y) { SRID = 4326 };
                }
                return null;
            }
        }
        set
        {
            if (value == null)
            {
                EmplacementString = null;
            }
            else
            {
                var wkbWriter = new WKBWriter();
                var bytes = wkbWriter.Write(value);
                EmplacementString = "0x" + BitConverter.ToString(bytes).Replace("-", "");
            }
        }
    }

    [InverseProperty("IdPlanEauNavigation")]
    public virtual ICollection<Miseaeau>? Miseaeaus { get; set; }

    [InverseProperty("IdPlanEauNavigation")]
    public virtual ICollection<Notedossier>? Notedossiers { get; set; }

    [InverseProperty("Planeau")]
    public virtual ICollection<EmployePlaneau> EmployePlaneau { get; set; } = new List<EmployePlaneau>();
    
    [InverseProperty("PlanEauNavigation")]
    public virtual ICollection<EEEPlanEau>? EEEPlanEau { get; set; }
}
