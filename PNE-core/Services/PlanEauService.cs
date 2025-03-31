using Microsoft.EntityFrameworkCore;
using PNE_core.Models;
using PNE_core.Services.Interfaces;
using PNE_core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

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
            if (entity.Latitude.HasValue && entity.Longitude.HasValue)
            {
                var point = new Point(entity.Longitude.Value, entity.Latitude.Value) { SRID = 4326 };
                var wkbWriter = new WKBWriter();
                var bytes = wkbWriter.Write(point);
                entity.EmplacementString = "0x" + BitConverter.ToString(bytes).Replace("-", "");
            }
            
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
            return await _dbSet
                .Include(p => p.EEEPlanEau)
                    .ThenInclude(e => e.EEE)
                .FirstOrDefaultAsync(m => m.IdPlanEau == id);
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
            var existingEntity = await _dbSet.FindAsync(entity.IdPlanEau);
            if (existingEntity != null)
            {
                // Mettre à jour les propriétés de base
                existingEntity.Nom = entity.Nom;
                existingEntity.NiveauCouleur = entity.NiveauCouleur;

                await _db.SaveChangesAsync();
            }
        }
    }
}
