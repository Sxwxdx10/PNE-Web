using Microsoft.EntityFrameworkCore;
using PNE_core.Models;
using PNE_core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNE_core.Services
{
    public class UtilisateursService : IUtilisateurService
    {
        private readonly IPneDbContext _db;
        private readonly DbSet<Utilisateur> _dbSet;

        public UtilisateursService(IPneDbContext db)
        {
            _db = db;
            _dbSet = _db.Utilisateurs;
        }

        public async Task LinkUserPlanEau(string idUtilisateur, string IdPlaneau)
        {
            Utilisateur? user = await _dbSet.Include(m => m.RolesUtilisateurs).FirstOrDefaultAsync(m => m.Id == idUtilisateur);
            Planeau? planeau = await _db.Planeaus.FirstOrDefaultAsync(p => p.IdPlanEau == IdPlaneau);

            if (planeau != null && user != null) {
                bool exists = await _db.EmployePlaneaus.Where(p => p.IdUtilisateur == idUtilisateur)
                    .AnyAsync(m => m.IdPlaneau == IdPlaneau);
                if (exists)
                {
                    return;
                }
                else
                {
                    bool gerant = false;
                    foreach(var role in user.RolesUtilisateurs)
                    {
                        if(role.nom_role == "gerant")
                        {
                            gerant = true;
                            break;
                        }
                    }

                    EmployePlaneau employePlaneau = new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        EstGerant = gerant,
                        IdPlaneau = IdPlaneau,
                        IdUtilisateur = idUtilisateur
                    };
                    _db.EmployePlaneaus.Add(employePlaneau);
                    await _db.SaveChangesAsync();
                }
            }
        }

        public async Task RemoveUserPlanEau(string idUtilisateur, string IdPlaneau)
        {
            Utilisateur? user = await _dbSet.Include(m => m.RolesUtilisateurs).FirstOrDefaultAsync(m => m.Id == idUtilisateur);
            Planeau? planeau = await _db.Planeaus.FirstOrDefaultAsync(p => p.IdPlanEau == IdPlaneau);

            if (planeau != null && user != null)
            {
                EmployePlaneau? employePlaneau = await _db.EmployePlaneaus.Where(p => p.IdUtilisateur == idUtilisateur)
                    .FirstOrDefaultAsync(m => m.IdPlaneau == IdPlaneau);
                if(employePlaneau != null)
                {
                    _db.EmployePlaneaus.Remove(employePlaneau);
                    await _db.SaveChangesAsync();
                }
            }
        }

        public async Task<List<Planeau>> GetUserPlanEau(string idUtilisateur, bool Gerant)
        {
            Utilisateur? user = await _dbSet.Include(m => m.RolesUtilisateurs).FirstOrDefaultAsync(m => m.Id == idUtilisateur);
            if (user is null)
                throw new InvalidOperationException("utilisateur introuvable");

            var PlanEauLinks = await _db.EmployePlaneaus.Where(x => x.IdUtilisateur == idUtilisateur)
                .Where(x => x.EstGerant == Gerant)
                .Include(x => x.Planeau)
                .ToListAsync();

            if (PlanEauLinks.Count == 0)
                throw new InvalidOperationException("pas de plans d'eau lie");

            List<Planeau> PlansEau = new List<Planeau>();
            foreach (var plan in PlanEauLinks)
                    PlansEau.Add(plan.Planeau);

            return PlansEau;
        }

        public async Task CreateAsync(Utilisateur entity)
        {
            _dbSet.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var utilisateur = await _dbSet.FindAsync(id);
            if (utilisateur != null)
            {
                _dbSet.Remove(utilisateur);
            }

            await _db.SaveChangesAsync();
        }

        public async Task<List<Utilisateur>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<Utilisateur?> GetByIdAsync(string id)
        {
            return await _dbSet.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Utilisateur?> GetForDetailAsync(string id)
        {
            return await _dbSet.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<bool> IsExist(string id)
        {
            return await _dbSet.AnyAsync(e => e.Id == id);
        }

        public async Task UpdateAsync(Utilisateur entity)
        {
            _dbSet.Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
