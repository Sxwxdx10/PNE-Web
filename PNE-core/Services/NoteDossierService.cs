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
    public class NoteDossierService : INoteDossierService
    {
        private readonly IPneDbContext _db;
        private readonly DbSet<Notedossier> _dbSet;

        public NoteDossierService(IPneDbContext db)
        {
            _db = db;
            _dbSet = _db.Notedossiers;
        }

        public async Task CreateAsync(Notedossier entity)
        {
            _dbSet.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var note = await _dbSet.FirstOrDefaultAsync(i => i.Idnote == id);
            if (note != null) {
                _dbSet.Remove(note);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<Notedossier>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<Notedossier?> GetByIdAsync(string id)
        {
            return await _dbSet.FirstOrDefaultAsync(m => m.Idnote == id);
        }

        public async Task<List<Notedossier>> GetNotesByEmbarcationIdAsync(string id)
        {
            return await _dbSet
             .Where(n => n.IdEmbarcation == id) // Filter by IdEmbarcationUtilisateur
             .ToListAsync();
        }

        public async Task<Notedossier?> GetForDetailAsync(string id)
        {
            return await _dbSet.FirstOrDefaultAsync(i => i.Idnote == id);
        }

        public async Task<bool> IsExist(string id)
        {
            return await _dbSet.AnyAsync(i => i.Idnote == id);
        }

        public async Task UpdateAsync(Notedossier entity)
        {
            _dbSet.Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
