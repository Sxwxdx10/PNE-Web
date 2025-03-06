using Microsoft.EntityFrameworkCore;
using PNE_core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNE_core.Services.Interfaces
{
    public interface IPneDbContext
    {
        #region tables
        public  DbSet<Certification> Certifications { get; set; }

        public  DbSet<Embarcation> Embarcations { get; set; }

        public  DbSet<Lavage> Lavages { get; set; }

        public  DbSet<Miseaeau> Miseaeaus { get; set; }

        public  DbSet<Notedossier> Notedossiers { get; set; }

        public  DbSet<Planeau> Planeaus { get; set; }

        public  DbSet<Role> Roles { get; set; }

        public  DbSet<Utilisateur> Utilisateurs { get; set; }

        public  DbSet<EEE> EEEs { get; set; }
        #endregion
        #region tables de liaison

        public  DbSet<Embarcationutilisateur> Embarcationutilisateurs { get; set; }

        public  DbSet<RolesUtilisateurs> RolesUtilisateurs { get; set; }

        public  DbSet<CertificationUtilisateur> CertificationUtilisateurs { get; set; }

        public  DbSet<EEEPlanEau> EEEPlanEaus { get; set; }

        #endregion

        public DbSet<EmployePlaneau> EmployePlaneaus { get; set; }

        public DbSet<StationLavage> StationLavages { get; set; }

        public void Add(object entity);

        public Task SaveChangesAsync();

        public void Update(object entity);
    }
}