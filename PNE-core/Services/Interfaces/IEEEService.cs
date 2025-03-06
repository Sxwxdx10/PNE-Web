using PNE_core.Models;

namespace PNE_core.Services.Interfaces
{
    public interface IEEEService : IPneServices<EEE>
    {
        public Task SignalerEEE(string IdEEE, string IdPlanEau);

        public Task ConfirmerEEE(string IdEEE, string IdPlanEau);

        public Task RetirerEEE(string IdEEE, string IdPlanEau);
    }
}
