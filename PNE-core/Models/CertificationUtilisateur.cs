using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNE_core.Models
{
    public class CertificationUtilisateur
    {
        [Key]
        [Column("IdCertificationUtilisateur")]
        public int IdCertificationUtilisateur { get; set; }
        public string CodeCertification { get; set; } = null!;
        public string IdUtilisateur { get; set; } = null!;

        [ForeignKey("CodeCertification")]
        [InverseProperty("CertificationUtilisateur")]
        public Certification certification { get; set; } = null!;

        [ForeignKey("IdUtilisateur")]
        [InverseProperty("CertificationUtilisateur")]
        public Utilisateur Utilisateur { get; set; } = null!;
    }
}
