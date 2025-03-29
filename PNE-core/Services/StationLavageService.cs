using Microsoft.EntityFrameworkCore;
using PNE_core.Models;
using PNE_core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PNE_core.Services
{
    public class StationLavageService : IStationLavageService
    {
        private readonly IPneDbContext _db;
        private readonly DbSet<StationLavage> _dbSet;
        private readonly ILogger<StationLavageService> _logger;

        public StationLavageService(IPneDbContext db, ILogger<StationLavageService> logger)
        {
            _db = db;
            _dbSet = _db.StationLavages;
            _logger = logger;
        }

        public async Task CreateAsync(StationLavage entity)
        {
            try
            {
                _logger.LogInformation($"Service - Création de la station : Id={entity.Id}, Position={entity.PositionString}");
                _logger.LogInformation($"Service - Plan d'eau associé : {entity.planeau?.IdPlanEau} - {entity.planeau?.Nom}");

                // Ajouter directement l'entité avec sa relation
                _dbSet.Add(entity);
                await _db.SaveChangesAsync();
                _logger.LogInformation("Service - Station créée avec succès");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service - Erreur lors de la création de la station");
                throw;
            }
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
            return await _dbSet.Include(s => s.planeau).ToListAsync();
        }

        public async Task<StationLavage?> GetByIdAsync(string id)
        {
            return await _dbSet.Include(s => s.planeau).FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<StationLavage?> GetForDetailAsync(string id)
        {
            return await _dbSet.Include(s => s.planeau).FirstOrDefaultAsync(m => m.Id == id);
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
