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
    public class PlanEauService : IPlanEauService
    {
        private readonly IPneDbContext _db;
        private readonly DbSet<Planeau> _dbSet;

        public PlanEauService(IPneDbContext db)
        {
            _db = db;
            _dbSet = _db.Planeaus;
        }

        public async Task CreateAsync(Planeau entity)
        {
            _dbSet.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var planeau = await _dbSet.FindAsync(id);
            if (planeau != null)
            {
                _dbSet.Remove(planeau);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<Planeau>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<Planeau?> GetByIdAsync(string id)
        {
            return await _dbSet.FirstOrDefaultAsync(m => m.IdPlanEau == id);
        }

        public async Task<Planeau?> GetForDetailAsync(string id)
        {
            return await _dbSet.FirstOrDefaultAsync(m => m.IdPlanEau == id);
        }

        public async Task<bool> IsExist(string id)
        {
            return await _dbSet.AnyAsync(e => e.IdPlanEau == id);
        }

        public async Task UpdateAsync(Planeau entity)
        {
            _dbSet.Update(entity);
            await _db.SaveChangesAsync();
        }

    }
}
