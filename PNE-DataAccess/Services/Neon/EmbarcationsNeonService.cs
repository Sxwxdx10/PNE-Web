using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PNE_core.Models;

namespace PNE_DataAccess.Services.Neon
{
    public class EmbarcationNeonService
    {
        private readonly NeonContext _neonDb;

        public EmbarcationNeonService(NeonContext neonDb)
        {
            _neonDb = neonDb;
        }

        public async Task<List<Embarcation>> GetAllAsync()
        {
            return await _neonDb.Embarcations.ToListAsync();
        }
    }
}
