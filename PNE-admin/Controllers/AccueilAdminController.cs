using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PNE_core.Models;
using PNE_core.Services.Interfaces;
using Newtonsoft.Json;
using PNE_admin.Annotations;
using NuGet.Protocol;

namespace PNE_admin.Controllers
{
    public class AccueilAdminController : Controller
    {
        private readonly IRoleService _roleService;

        public AccueilAdminController(IRoleService roleService) 
        {
            _roleService = roleService;
        }

        [Authorize] //authorize normale pour savoir que le user a passe par la connexion
        public async Task<IActionResult> Index()
        {
            // devrait arriver directement de la connexion, verification et stockage des roles se fait ici
            // l'interface ne doit montrer que les options que chaque utilisateur a droit d'avoir acces
            var serialisedUser = HttpContext.Session.GetString("currentUser")!;
            Utilisateur utilisateur = JsonConvert.DeserializeObject<Utilisateur>(serialisedUser)!;

            var roleList = await _roleService.GetUserRoles(utilisateur.Id);
            List<string> roles = [];
            foreach(var role in roleList)
            {
                roles.Add(role.NomRole);
            }

            HttpContext.Session.SetString("userRoles", roles.ToJson());
            ViewData["Title"] = "AccueilAdmin";
            return View();
        }
    }
}
