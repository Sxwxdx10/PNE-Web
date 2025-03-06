using Microsoft.EntityFrameworkCore;
using PNE_core.Models;
using PNE_core.Services.Interfaces;

namespace PNE_core.Services
{
    public class EmbarcationService : IEmbarcationService
    {
        private readonly IPneDbContext _db;
        private readonly DbSet<Embarcation> _dbSet;

        public EmbarcationService(IPneDbContext db)
        {
            _db = db;
            _dbSet = _db.Embarcations;
        }

        public async Task AddUtilisateurAsync(string embarcationId, string utilisateurId)
        {
            var embarcation = await _dbSet.FindAsync(embarcationId);
            var user = await _db.Utilisateurs.FindAsync(utilisateurId);
            if (embarcation == null || user == null)
                throw new InvalidOperationException("embarcation ou user introuvable");

            Embarcationutilisateur link = new()
            {
                IdEmbarcation = embarcationId,
                IdUtilisateur = utilisateurId,
            };
            _db.Embarcationutilisateurs.Add(link);

            await _db.SaveChangesAsync();
        }

        public async Task CreateAsync(Embarcation entity)
        {
            _dbSet.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var embarcation = await _dbSet.FindAsync(id);
            if (embarcation != null)
            {
                _dbSet.Remove(embarcation);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<Embarcation>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<Embarcation?> GetByIdAsync(string id)
        {
            return await _dbSet.FirstOrDefaultAsync(m => m.IdEmbarcation == id);
        }

        public async Task<Embarcation?> GetForDetailAsync(string id)
        {
            return await _dbSet.FirstOrDefaultAsync(m => m.IdEmbarcation == id);
        }

        public async Task<bool> IsExist(string id)
        {
            return await _dbSet.AnyAsync(e => e.IdEmbarcation == id);
        }

        public async Task UpdateAsync(Embarcation entity)
        {
            _dbSet.Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
