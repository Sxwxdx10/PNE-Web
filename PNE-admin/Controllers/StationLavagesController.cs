using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PNE_core.Models;
using PNE_core.Services.Interfaces;
using PNE_DataAccess;
using NetTopologySuite.Geometries;
using Npgsql;
using Microsoft.Extensions.Logging;

namespace PNE_admin.Controllers
{
    public class StationLavagesController : Controller
    {
        private readonly PneContext _context;
        private readonly IStationLavageService _services;
        private readonly IPlanEauService _planEauService;
        private readonly ILogger<StationLavagesController> _logger;

        public StationLavagesController(
            PneContext context, 
            IStationLavageService pneServices, 
            IPlanEauService planEauService,
            ILogger<StationLavagesController> logger)
        {
            _context = context;
            _services = pneServices;
            _planEauService = planEauService;
            _logger = logger;
        }

        // GET: StationLavagesController
        public async Task<IActionResult> Index()
        {
            return View(await _services.GetAllAsync());
        }

        // GET: StationLavagesController/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var station = await _services.GetByIdAsync(id);
            if (station == null)
            {
                return NotFound();
            }

            return View(station);
        }

        // GET: StationLavagesController/Create
        public async Task<IActionResult> Create(string idPlanEau)
        {
            if (string.IsNullOrEmpty(idPlanEau))
            {
                return NotFound();
            }

            var planEau = await _planEauService.GetForDetailAsync(idPlanEau);
            if (planEau == null)
            {
                return NotFound();
            }

            ViewBag.PlanEau = planEau;
            return View();
        }

        // POST: StationLavagesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nom,PositionString,PeutDecontaminer,HautePression,BassePressionetAttaches,EauChaude")] StationLavage station, string idPlanEau)
        {
            try
            {
                // Générer un ID unique dans le même format que les plans d'eau
                station.Id = $"S{DateTime.Now:yyMMdd}{new Random().Next(10, 99)}";
                _logger.LogInformation($"=== Début de la création de la station de lavage ===");
                _logger.LogInformation($"Id généré: {station.Id}");
                _logger.LogInformation($"Nom: {station.Nom}");
                _logger.LogInformation($"Position: {station.PositionString}");
                _logger.LogInformation($"IdPlanEau: {idPlanEau}");
                _logger.LogInformation($"PeutDecontaminer: {station.PeutDecontaminer}");
                _logger.LogInformation($"HautePression: {station.HautePression}");
                _logger.LogInformation($"BassePressionetAttaches: {station.BassePressionetAttaches}");
                _logger.LogInformation($"EauChaude: {station.EauChaude}");

                // Récupérer le plan d'eau
                var planEau = await _planEauService.GetForDetailAsync(idPlanEau);
                if (planEau == null)
                {
                    _logger.LogError("Erreur: Plan d'eau non trouvé");
                    return NotFound("Plan d'eau non trouvé");
                }

                // Assigner le plan d'eau à la station
                station.planeau = planEau;
                station.StationPersonnelStatus = PNE_core.Enums.StationPersonnelStatus.Aucun;

                // Effacer et revalider le ModelState après avoir défini l'Id
                ModelState.Clear();
                if (!TryValidateModel(station))
                {
                    _logger.LogError("Validation du modèle échouée après mise à jour");
                    foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        _logger.LogError($"Erreur de validation: {modelError.ErrorMessage}");
                    }
                    ViewBag.PlanEau = planEau;
                    return View(station);
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _logger.LogInformation("Appel de CreateAsync");
                        await _services.CreateAsync(station);
                        _logger.LogInformation("CreateAsync terminé avec succès");
                        return RedirectToAction("GestionPlanEau", "Gerant", new { idPlanEau = planEau.IdPlanEau });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Erreur lors de la création: {ex.Message}");
                        _logger.LogError($"StackTrace: {ex.StackTrace}");
                        ModelState.AddModelError("", $"Erreur lors de la création: {ex.Message}");
                        ViewBag.PlanEau = planEau;
                        return View(station);
                    }
                }

                _logger.LogError("ModelState invalide");
                foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogError($"Erreur de validation: {modelError.ErrorMessage}");
                }
                ViewBag.PlanEau = planEau;
                return View(station);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception lors de la création: {ex.Message}");
                _logger.LogError($"StackTrace: {ex.StackTrace}");
                ModelState.AddModelError("", "Une erreur s'est produite lors de la création : " + ex.Message);
                return View(station);
            }
        }

        // GET: StationLavagesController/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var station = await _services.GetByIdAsync(id);
            if (station == null)
            {
                return NotFound();
            }
            return View(station);
        }

        // POST: StationLavagesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Nom,Position,PeutDecontaminer,HautePression,BassePressionetAttaches,EauChaude")] StationLavage station)
        {
            if (id != station.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _services.UpdateAsync(station);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _services.IsExist(station.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(station);
        }

        // GET: StationLavagesController/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var station = await _context.StationLavages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (station == null)
            {
                return NotFound();
            }

            return View(station);
        }

        // POST: StationLavagesController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var station = await _context.StationLavages.FindAsync(id);
            if (station != null)
            {
                _context.StationLavages.Remove(station);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StationExists(string id)
        {
            return _context.StationLavages.Any(e => e.Id == id);
        }
    }
}
