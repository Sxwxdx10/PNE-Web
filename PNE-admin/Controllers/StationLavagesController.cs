using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PNE_core.Models;
using PNE_core.Services.Interfaces;
using PNE_DataAccess;
using NetTopologySuite.Geometries;
using Npgsql;

namespace PNE_admin.Controllers
{
    public class StationLavagesController : Controller
    {
        private readonly PneContext _context;
        private readonly IStationLavageService _services;

        public StationLavagesController(PneContext context, IStationLavageService pneServices)
        {
            _context = context;
            _services = pneServices;
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
        public IActionResult Create()
        {
            return View();
        }

        // POST: StationLavagesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nom")] StationLavage station)
        {
            station.Id = Guid.NewGuid().ToString();
            station.Position = new Point(1d, 1d); //À changer lorsqu'on laisse le user mettre la position lui-même
            if (ModelState.IsValid)
            {
                station.PeutDecontaminer = false;
                station.HautePression = false;
                station.EauChaude = false;
                station.BassePressionetAttaches = false;
                station.StationPersonnelStatus = 0;
                await _services.CreateAsync(station);
                return RedirectToAction(nameof(Index));
            }
            return View(station);
        }

        // GET: StationLavagesController/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var station = await _context.StationLavages.FindAsync(id);
            if (station == null)
            {
                return NotFound();
            }
            return View(station);
        }

        // POST: StationLavagesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Nom")] StationLavage station)
        {
            if (id != station.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                station.Position = new Point(1d, 1d); //À changer lorsqu'on laisse le user mettre la position lui-même

                try
                {
                    _context.Update(station);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StationExists(station.Id))
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
