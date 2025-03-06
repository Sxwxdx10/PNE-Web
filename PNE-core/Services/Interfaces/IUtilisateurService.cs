using PNE_core.Models;

namespace PNE_core.Services.Interfaces
{
    public interface IUtilisateurService : IPneServices<Utilisateur>
    {
        Task LinkUserPlanEau(string idUtilisateur, string IdPlaneau);

        Task RemoveUserPlanEau(string idUtilisateur, string IdPlaneau);

        Task<List<Planeau>> GetUserPlanEau(string idUtilisateur, bool Gerant);
    }
}

