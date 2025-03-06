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
    public class StationLavageService : IStationLavageService
    {
        private readonly IPneDbContext _db;
        private readonly DbSet<StationLavage> _dbSet;

        public StationLavageService(IPneDbContext db)
        {
            _db = db;
            _dbSet = _db.StationLavages;
        }

        public async Task CreateAsync(StationLavage entity)
        {
            _dbSet.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var station = await _dbSet.FindAsync(id);
            if (station != null)
            {
                _dbSet.Remove(station);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<StationLavage>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<StationLavage?> GetByIdAsync(string id)
        {
            return await _dbSet.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<StationLavage?> GetForDetailAsync(string id)
        {
            return await _dbSet.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<bool> IsExist(string id)
        {
            return await _dbSet.AnyAsync(e => e.Id == id);
        }

        public async Task UpdateAsync(StationLavage entity)
        {
            _dbSet.Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
