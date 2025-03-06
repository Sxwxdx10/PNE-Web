using Microsoft.EntityFrameworkCore;
using PNE_core.Enums;
using PNE_core.Models;
using PNE_core.Services.Interfaces;

namespace PNE_core.Services
{
    public class EEEService : IEEEService
    {
        private readonly IPneDbContext _db;
        private readonly DbSet<EEE> _dbSet;

        public EEEService(IPneDbContext db)
        {
            _db = db;
            _dbSet = db.EEEs;
        }

        public async Task SignalerEEE(string IdEEE, string IdPlanEau)
        {
            var EEE = await _dbSet.FirstOrDefaultAsync(x => x.Id == IdEEE);
            var PlanEau = await _db.Planeaus.FirstOrDefaultAsync(x => x.IdPlanEau == IdPlanEau);

            if (EEE == null || PlanEau == null)
                throw new InvalidOperationException("EEE ou Plan d'eau invalid");

            bool exists = await _db.EEEPlanEaus.Where(u => u.IdEEE == IdEEE)
                    .AnyAsync(r => r.IdPlanEau == IdPlanEau);

            if (exists)
                throw new InvalidOperationException("EEE deja signale dans ce plan d'eau");

            var signal = new EEEPlanEau()
            {
                IdEEE = IdEEE,
                IdPlanEau = IdPlanEau,
                Validated = false
            };
            _db.EEEPlanEaus.Add(signal);
            await _db.SaveChangesAsync();
        }

        public async Task ConfirmerEEE(string IdEEE, string IdPlanEau)
        {
            var EEE = await _dbSet.FirstOrDefaultAsync(x => x.Id == IdEEE);
            var PlanEau = await _db.Planeaus.FirstOrDefaultAsync(x => x.IdPlanEau == IdPlanEau);
            //EEE et planEau devrait toujours existe car la signalisation de l'EEE a besoin de les avoirs valides
            if (EEE == null || PlanEau == null)
                throw new InvalidOperationException("EEE ou Plan d'eau invalid");

            var signal = await _db.EEEPlanEaus.Where(u => u.IdEEE == IdEEE)
                .FirstOrDefaultAsync(r => r.IdPlanEau == IdPlanEau);
            if (signal == null)
                throw new InvalidOperationException("signalisation de l'EEE pas trouvé");
            if (signal.Validated)
                throw new InvalidOperationException("Cette EEE a deja été confirmé");

            signal.Validated = true;
            _db.EEEPlanEaus.Update(signal);

            //update la couleur
            if (EEE.NiveauCouleur > PlanEau.NiveauCouleur)
            {
                PlanEau.NiveauCouleur = EEE.NiveauCouleur;
                _db.Planeaus.Update(PlanEau);
            }

            await _db.SaveChangesAsync();
        }

        public async Task RetirerEEE(string IdEEE, string IdPlanEau)
        {
            //valider le plan deau pour utilisation ulterieur
            var PlanEau = await _db.Planeaus.FirstOrDefaultAsync(x => x.IdPlanEau == IdPlanEau);
            if (PlanEau is null)
                throw new InvalidOperationException("Plan d'eau invalid");

            var signal = await _db.EEEPlanEaus.Where(u => u.IdEEE == IdEEE)
                .FirstOrDefaultAsync(r => r.IdPlanEau == IdPlanEau);
            if (signal == null)
                throw new InvalidOperationException("signalisation de l'EEE pas trouvé");

            _db.EEEPlanEaus.Remove(signal);

            //update la couleur si necessaire
            var liensRestant = await _db.EEEPlanEaus.Where(x => x.IdPlanEau == IdPlanEau)
                .Where(x => x.IdEEE != IdEEE).Include(x => x.EEE).ToListAsync();
            Niveau niveau = Niveau.Vert;
            foreach (var lien in liensRestant)
            {
                if (lien.EEE.NiveauCouleur > niveau)
                    niveau = lien.EEE.NiveauCouleur;
            }
            if (PlanEau.NiveauCouleur != niveau)
            {
                PlanEau.NiveauCouleur = niveau;
                _db.Planeaus.Update(PlanEau);
            }

            await _db.SaveChangesAsync();
        }

        public async Task CreateAsync(EEE entity)
        {
            _dbSet.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var EEE = await _dbSet.FindAsync(id);
            if (EEE != null)
            {
                _dbSet.Remove(EEE);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<EEE>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<EEE?> GetByIdAsync(string id)
        {
            return await _dbSet.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<EEE?> GetForDetailAsync(string id)
        {
            return await _dbSet.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<bool> IsExist(string id)
        {
            return await _dbSet.AnyAsync(e => e.Id == id);
        }

        public async Task UpdateAsync(EEE entity)
        {
            _dbSet.Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
