using Microsoft.EntityFrameworkCore;
using PNE_core.Models;
using PNE_core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNE_core.Services
{
    public class RoleService : IRoleService
    {
        private readonly IPneDbContext _db;
        private readonly DbSet<Role> _dbSet;

        public RoleService(IPneDbContext db)
        {
            _db = db;
            _dbSet = _db.Roles;
        }

        public async Task AddUserRole(string idUtilisateur, string nomRole)
        {
            var user = await _db.Utilisateurs.FindAsync(idUtilisateur);
            var role = await _db.Roles.FindAsync(nomRole);

            if (role == null || user == null)
                throw new InvalidOperationException("utilisateur ou role introuvable");
            
            var link = new RolesUtilisateurs()
            {
                IdUtilisateur = idUtilisateur,
                nom_role = nomRole
            };
            _db.RolesUtilisateurs.Add(link);
            await _db.SaveChangesAsync();
        }

        public async Task RemoveUserRole(string idUtilisateur, string nomRole)
        {
            var user = await _db.Utilisateurs.FindAsync(idUtilisateur);
            var role = await _db.Roles.FindAsync(nomRole);

            if (role == null && user == null)
                throw new InvalidOperationException("utilisateur ou role introuvable");

            var link = await _db.RolesUtilisateurs.Where(u => u.IdUtilisateur == idUtilisateur)
                .FirstOrDefaultAsync(r => r.nom_role == nomRole);
            if (link == null)
                throw new InvalidOperationException("cet utilisateur n'a pas ce role");
            
            _db.RolesUtilisateurs.Remove(link);
            await _db.SaveChangesAsync();
        }

        public async Task<List<Role>?> GetUserRoles(string idUtilisateur)
        {
            var user = await _db.Utilisateurs.FindAsync(idUtilisateur);
            if (user != null)
            {
                var links = await _db.RolesUtilisateurs.Where(x => x.IdUtilisateur == idUtilisateur).Include(x => x.Role)
                    .ToListAsync();

                List<Role> roles = [];
                foreach (var link in links) {
                    roles.Add(link.Role);
                }
                return roles;
            }
            return null;
        }
        public async Task<List<Role>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
    }
}
