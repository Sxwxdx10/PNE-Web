using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PNE_core.Models
{
    public partial class RolesUtilisateurs
    {
        [Key]
        [Column("IdRolesUtilisateurs")]
        public int IdRolesUtilisateurs { get; set; }
        public string nom_role { get; set; } = null!;
        public string IdUtilisateur { get; set; } = null!;

        [ForeignKey("nom_role")]
        [InverseProperty("RolesUtilisateurs")]
        public virtual Role Role { get; set; } = null!;

        [ForeignKey("IdUtilisateur")]
        [InverseProperty("RolesUtilisateurs")]
        public virtual Utilisateur Utilisateur { get; set; } = null!;
    }
}