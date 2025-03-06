using Microsoft.AspNetCore.Mvc;
using PNE_admin.Annotations;
using PNE_core.Services.Interfaces;

namespace PNE_admin.Controllers
{
    [RolesAuthentication("chercheur")]
    public class ChercheurController : Controller
    {
        private readonly IPlanEauService _eauService;
        private readonly IEmbarcationService _embarcationService;

        public ChercheurController(IPlanEauService eauService, IEmbarcationService embarcationService)
        {
            _eauService = eauService;
            _embarcationService = embarcationService;
        }

        public async Task<IActionResult> PlanEau()
        {
            ViewData["Title"] = "Plans d'eau";
            return View();
        }

        public async Task<IActionResult> DeplacementsEmbarcations()
        {
            ViewData["Title"] = "Deplacements d'embarcations";
            return View();
        }
    }
}
