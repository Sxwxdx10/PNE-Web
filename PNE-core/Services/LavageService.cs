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

    public class LavageService : ILavageService
    {
        private readonly IPneDbContext _db;
        private readonly DbSet<Lavage> _dbSet;
        public LavageService(IPneDbContext db)
        {
            _db = db;
            _dbSet = _db.Lavages;
        }

        public async Task CreateAsync(Lavage entity)
        {
            _dbSet.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var lavage = await _dbSet.FindAsync(id);
            if (lavage != null)
            {
                _dbSet.Remove(lavage);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<Lavage>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<Lavage?> GetByIdAsync(string id)
        {
            return await _dbSet.FirstOrDefaultAsync(m => m.IdLavage == id);
        }

        public async Task<Lavage?> GetForDetailAsync(string id)
        {
            return await _dbSet.FirstOrDefaultAsync(m => m.IdLavage == id);
        }

        public async Task<bool> IsExist(string id)
        {
            return await _dbSet.AnyAsync(e => e.IdLavage == id);
        }

        public async Task UpdateAsync(Lavage entity)
        {
            _dbSet.Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
