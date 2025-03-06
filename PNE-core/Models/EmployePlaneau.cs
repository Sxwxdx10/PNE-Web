using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PNE_core.Models
{
    public class EmployePlaneau
    {
        [Key]
        [Column("IdEmployePlaneau")]
        public string Id { get; set; }

        public string IdUtilisateur { get; set; }

        public string IdPlaneau { get; set; }

        public bool EstGerant { get; set; }

        [ForeignKey("IdPlaneau")]
        [InverseProperty("EmployePlaneau")]
        public Planeau Planeau { get; set; } = null!;

        [ForeignKey("IdUtilisateur")]
        [InverseProperty("EmployePlaneau")]
        public Utilisateur Utilisateur { get; set; } = null!;
    }
}
