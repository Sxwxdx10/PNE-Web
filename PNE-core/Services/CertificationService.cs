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
    public class CertificationService : ICertificationService
    {
        private readonly IPneDbContext _db;
        private readonly DbSet<Certification> _dbSet;

        public CertificationService(IPneDbContext db)
        {
            _db = db;
            _dbSet = _db.Certifications;
        }

        /// <summary>
        /// permet de donner une certification a un user, la certification doit exister d'avance
        /// </summary>
        /// <param name="codeCertification">code de la certification a ajouter</param>
        /// <param name="IdUtilisateur">utilisateur qui recoit la certification</param>
        /// <returns>rien si tout vas bien, InvalidOperationException sinon</returns>
        public async Task CertifyUser(string codeCertification, string IdUtilisateur)
        {
            var certification =  await _dbSet.FirstOrDefaultAsync(x => x.CodeCertification == codeCertification);
            var user = await _db.Utilisateurs.FirstOrDefaultAsync(x => x.Id == IdUtilisateur);
            if (user == null || certification == null)
                throw new InvalidOperationException("utilisateur ou certification introuvable");

            CertificationUtilisateur link = new()
            {
                CodeCertification = codeCertification,
                IdUtilisateur = IdUtilisateur,
                certification = certification,
                Utilisateur = user
            };
            _db.CertificationUtilisateurs.Add(link);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// creer une nouvelle certification
        /// </summary>
        /// <param name="entity">la certification a ajouter, sans id ni lien vers les users</param>
        /// <returns></returns>
        public async Task CreateAsync(Certification entity)
        {
            _dbSet.Add(entity);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// retirer une certification
        /// </summary>
        /// <param name="id">id de la certification a retirer</param>
        /// <returns></returns>
        public async Task DeleteAsync(string id)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(x => x.CodeCertification == id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// retourne un List<Certification> de toute les certifications dans la bd
        /// </summary>
        /// <returns>liste des certifications</returns>
        public async Task<List<Certification>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        /// <summary>
        /// chercher la certification avec l'id specifie 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Certification?> GetByIdAsync(string id)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.CodeCertification == id);
        }

        /// <summary>
        /// (incomplet) retourne la certification dans un certain DTO selon l'id specifie
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Certification?> GetForDetailAsync(string id)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.CodeCertification == id);
        }

        /// <summary>
        /// verifie si la certification existe
        /// </summary>
        /// <param name="id"></param>
        /// <returns>true si elle existe, false sinon</returns>
        public async Task<bool> IsExist(string id)
        {
            return await _dbSet.AnyAsync(x => x.CodeCertification == id);
        }

        /// <summary>
        /// update le data de la certification, prend l'id et change le reste
        /// </summary>
        /// <param name="entity">la certification avec l'id voulue et le reste du data a changer</param>
        /// <returns></returns>
        public async Task UpdateAsync(Certification entity)
        {
            _dbSet.Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
