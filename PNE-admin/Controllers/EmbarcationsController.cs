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
using PNE_DataAccess.Services;
using PNE_DataAccess.Services.Neon;

namespace PNE_admin.Controllers
{
    public class EmbarcationsController : Controller
    {
        private readonly PneContext _context;
        private readonly IEmbarcationService _services;
        private readonly INoteDossierService _noteDossierService;
        private readonly EmbarcationNeonService _neonService;

        public EmbarcationsController(PneContext context, IEmbarcationService pneServices, INoteDossierService noteDossierService, EmbarcationNeonService neonService)
        {
            _context = context;
            _services = pneServices;
            _noteDossierService = noteDossierService;
            _neonService = neonService;

        }

        // GET: Embarcations
        public async Task<IActionResult> Index()
        {
            var embarcations = await _neonService.GetAllAsync();
            return View(embarcations);
        }

        // GET: Embarcations/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var embarcation = await _services.GetByIdAsync(id);
            if (embarcation == null)
            {
                return NotFound();
            }

            return View(embarcation);
        }

        // GET: Embarcations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Embarcations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdEmbarcation,Description,Marque,Longueur,Photo,codeQR")] Embarcation embarcation)
        {
            if (ModelState.IsValid)
            {
                await _services.CreateAsync(embarcation);
                return RedirectToAction(nameof(Index));
            }
            return View(embarcation);
        }

        // GET: Embarcations/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var embarcation = await _services.GetByIdAsync(id);
            if (embarcation == null)
            {
                return NotFound();
            }
            return View(embarcation);
        }

        // POST: Embarcations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("IdEmbarcation,Description,Marque,Longueur,Photo")] Embarcation embarcation)
        {
            if (id != embarcation.IdEmbarcation)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _services.UpdateAsync(embarcation);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await EmbarcationExists(embarcation.IdEmbarcation))
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
            return View(embarcation);
        }

        // GET: Embarcations/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var embarcation = await _services.GetByIdAsync(id);
            if (embarcation == null)
            {
                return NotFound();
            }

            return View(embarcation);
        }

        // POST: Embarcations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _services.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> EmbarcationExists(string id)
        {
            return await _services.IsExist(id);
        }


       // public async Task<IActionResult> AddNote(string id)
       // {
       //     return View("~/Views/NoteDossiers/Index.cshtml");
       // }

    }
}
