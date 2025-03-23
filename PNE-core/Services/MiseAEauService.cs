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
    public class MiseAEauService : IMiseAEauService
    {
        private readonly IPneDbContext _db;
        private readonly DbSet<Miseaeau> _dbSet;
        public MiseAEauService(IPneDbContext db)
        {
            _db = db;
            _dbSet = _db.Miseaeaus;
        }

        public async Task CreateAsync(Miseaeau entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _dbSet.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var miseAEau = await _dbSet.FindAsync(id);
            if (miseAEau is not null)
            {
                _dbSet.Remove(miseAEau);
            }
            await _db.SaveChangesAsync();
        }

        public async Task<List<Miseaeau>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<Miseaeau?> GetByIdAsync(string id)
        {
            return await _dbSet.FirstOrDefaultAsync(m => m.IdMiseEau == id);
        }

        public async Task<Miseaeau?> GetForDetailAsync(string id)
        {
            return await _dbSet.FirstOrDefaultAsync(m => m.IdMiseEau == id);
        }

        public async Task<bool> IsExist(string id)
        {
            return await _dbSet.AnyAsync(e => e.IdMiseEau == id);
        }

        public async Task UpdateAsync(Miseaeau entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var existingMiseAEau = await _dbSet.FindAsync(entity.IdMiseEau);
            if (existingMiseAEau != null)
            {
                existingMiseAEau.Date = entity.Date;
                existingMiseAEau.DureeEnJours = entity.DureeEnJours;  // ✅ Update DureeEnJours
                existingMiseAEau.IdPlanEau = entity.IdPlanEau;
                existingMiseAEau.IdEmbarcation = entity.IdEmbarcation;

                await _db.SaveChangesAsync();
            }
            await _db.SaveChangesAsync();
        }
    }
}
