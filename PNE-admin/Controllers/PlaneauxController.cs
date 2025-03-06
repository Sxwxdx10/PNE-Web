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
        public async Task<IActionResult> Create([Bind("Nom")] Planeau planeau)
        {
            if (ModelState.IsValid)
            {
                var serialisedUser = HttpContext.Session.GetString("currentUser")!;
                Utilisateur utilisateur = JsonConvert.DeserializeObject<Utilisateur>(serialisedUser)!;
                planeau.IdPlanEau = Guid.NewGuid().ToString().Substring(0, 10);
                planeau.Emplacement = new NetTopologySuite.Geometries.Point(new NetTopologySuite.Geometries.Coordinate(48.587, -72.04));
                planeau.NiveauCouleur = 0;

                await _eauService.CreateAsync(planeau);
                await _userServices.LinkUserPlanEau(utilisateur.Id, planeau.IdPlanEau);
                return RedirectToAction(nameof(Index));
            }
            return View(planeau);
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
        public async Task<IActionResult> Edit(string id, [Bind("IdPlanEau,Nom")] Planeau planeau)
        {
            if (id != planeau.IdPlanEau)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    planeau.Emplacement = new NetTopologySuite.Geometries.Point(new NetTopologySuite.Geometries.Coordinate(48.587, -72.04));
                    planeau.NiveauCouleur = 0;
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
