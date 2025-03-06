using PNE_core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNE_core.Services.Interfaces
{
    public interface IEmbarcationService : IPneServices<Embarcation>
    {
        public Task AddUtilisateurAsync(string embarcationId, string utilisateurId);
    }
}
