using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PNE_DataAccess;
using PNE_core.Models;
using PNE_core.Services.Interfaces;
using PNE_core.Services;
using Newtonsoft.Json;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace PNE_admin.Controllers
{
    public class PlaneauxController : Controller
    {
        private readonly IUtilisateurService _userServices;
        private readonly IPlanEauService _eauService;

        public PlaneauxController(IUtilisateurService userServices, IPlanEauService eauService)
        {
            _userServices = userServices;
            _eauService = eauService;
        }

        // GET: Planeaux
        public async Task<IActionResult> Index()
        {
            return View(await _eauService.GetAllAsync());
        }

        // GET: Planeaux/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var planeau = await _eauService.GetByIdAsync(id);
            if (planeau == null)
            {
                return NotFound();
            }

            return View(planeau);
        }

        // GET: Planeaux/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Planeaux/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPlanEau,Nom,NiveauCouleur,Latitude,Longitude,EmplacementString")] Planeau planeau)
        {
            try
            {
                // Ajout des messages de débogage
                System.Diagnostics.Debug.WriteLine($"=== Début de la création du plan d'eau ===");
                System.Diagnostics.Debug.WriteLine($"Nom: {planeau.Nom}");
                System.Diagnostics.Debug.WriteLine($"NiveauCouleur: {planeau.NiveauCouleur}");
                System.Diagnostics.Debug.WriteLine($"Latitude: {planeau.Latitude}");
                System.Diagnostics.Debug.WriteLine($"Longitude: {planeau.Longitude}");

                // Initialiser l'ID avant la validation
                planeau.IdPlanEau = Guid.NewGuid().ToString().Substring(0, 10);
                System.Diagnostics.Debug.WriteLine($"ID généré: {planeau.IdPlanEau}");

                // Vérifier les coordonnées avant tout
                if (!planeau.Latitude.HasValue || !planeau.Longitude.HasValue)
                {
                    System.Diagnostics.Debug.WriteLine("Erreur: Coordonnées manquantes");
                    ModelState.AddModelError("", "Les coordonnées sont requises");
                    return View(planeau);
                }

                // Vérifier le niveau de couleur
                if (!planeau.NiveauCouleur.HasValue)
                {
                    System.Diagnostics.Debug.WriteLine("Erreur: Niveau de couleur non spécifié");
                    ModelState.AddModelError("NiveauCouleur", "Le niveau de couleur est requis");
                    return View(planeau);
                }

                // Initialiser EmplacementString
                try
                {
                    System.Diagnostics.Debug.WriteLine("Conversion des coordonnées en WKB");
                    var point = new Point(planeau.Longitude.Value, planeau.Latitude.Value) { SRID = 4326 };
                    var wkbWriter = new WKBWriter();
                    var bytes = wkbWriter.Write(point);
                    planeau.EmplacementString = "0x" + BitConverter.ToString(bytes).Replace("-", "");
                    System.Diagnostics.Debug.WriteLine($"EmplacementString généré: {planeau.EmplacementString}");

                    // Mettre à jour le ModelState après avoir défini EmplacementString
                    ModelState.Clear();
                    if (!TryValidateModel(planeau))
                    {
                        System.Diagnostics.Debug.WriteLine("Validation du modèle échouée après mise à jour");
                        return View(planeau);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erreur lors de la conversion des coordonnées: {ex.Message}");
                    ModelState.AddModelError("", "Erreur lors de la conversion des coordonnées");
                    return View(planeau);
                }

                if (ModelState.IsValid)
                {
                    System.Diagnostics.Debug.WriteLine("ModelState est valide");
                    var serialisedUser = HttpContext.Session.GetString("currentUser")!;
                    Utilisateur utilisateur = JsonConvert.DeserializeObject<Utilisateur>(serialisedUser)!;

                    System.Diagnostics.Debug.WriteLine("Appel de CreateAsync");
                    await _eauService.CreateAsync(planeau);
                    System.Diagnostics.Debug.WriteLine("CreateAsync terminé avec succès");

                    System.Diagnostics.Debug.WriteLine($"Liaison avec l'utilisateur {utilisateur.Id}");
                    await _userServices.LinkUserPlanEau(utilisateur.Id, planeau.IdPlanEau);
                    System.Diagnostics.Debug.WriteLine("Liaison utilisateur terminée avec succès");

                    return RedirectToAction(nameof(Index));
                }

                System.Diagnostics.Debug.WriteLine("ModelState invalide");
                foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
                {
                    System.Diagnostics.Debug.WriteLine($"Erreur de validation: {modelError.ErrorMessage}");
                }
                return View(planeau);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception lors de la création: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                ModelState.AddModelError("", "Une erreur s'est produite lors de la création : " + ex.Message);
                return View(planeau);
            }
        }

        // GET: Planeaux/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var planeau = await _eauService.GetByIdAsync(id);
            if (planeau == null)
            {
                return NotFound();
            }
            return View(planeau);
        }

        // POST: Planeaux/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("IdPlanEau,Nom,NiveauCouleur,Latitude,Longitude")] Planeau planeau)
        {
            if (id != planeau.IdPlanEau)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!planeau.NiveauCouleur.HasValue)
                    {
                        ModelState.AddModelError("NiveauCouleur", "Le niveau de couleur est requis");
                        return View(planeau);
                    }

                    await _eauService.UpdateAsync(planeau);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await PlaneauExists(planeau.IdPlanEau))
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
            return View(planeau);
        }

        // GET: Planeaux/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var planeau = await _eauService.GetByIdAsync(id);
            if (planeau == null)
            {
                return NotFound();
            }

            return View(planeau);
        }

        // POST: Planeaux/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var planeau = await _eauService.GetByIdAsync(id);
            if (planeau != null)
            {
                await _eauService.DeleteAsync(id);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> PlaneauExists(string id)
        {
            return await _eauService.IsExist(id);
        }
    }
}
