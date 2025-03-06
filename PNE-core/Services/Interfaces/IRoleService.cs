using PNE_core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNE_core.Services.Interfaces
{
    public interface IRoleService
    {
        Task AddUserRole(string idUtilisateur, string nomRole);

        Task RemoveUserRole(string idUtilisateur, string nomRole);

        Task<List<Role>?> GetUserRoles(string idUtilisateur);
        Task<List<Role>?> GetAllAsync();
    }
}