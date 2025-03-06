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

namespace PNE_admin.Controllers
{
    public class NotedossiersController : Controller
    {
        private readonly PneContext _context;
        private readonly INoteDossierService _services;
        private readonly IUtilisateurService _userservices;

        public NotedossiersController(PneContext context, INoteDossierService pneServices, IUtilisateurService userservices)
        {
            _context = context;
            _services = pneServices;
            _userservices = userservices;
        }


        // GET: Notedossiers
        public async Task<IActionResult> Index(string id)
        {
            id = GetId(id); //Récupère l'Id de l'objet et le stocke pour le réutiliser
            if (id == null)
            {
                return NotFound();
            }
            return View(await _services.GetNotesByEmbarcationIdAsync(id));

        }

        // GET: Notedossiers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var note = await _services.GetByIdAsync(id);
            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        // GET: Notedossiers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Notedossiers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Note,Idnote")] Notedossier notedossier)
        {
            //On prend l'Id de la session. S'il n'y en a pas, alors le ModelState ne sera pas valide
            notedossier.IdEmbarcation = HttpContext.Session.GetString("IdEmbarcation");
            notedossier.Date = DateTime.Now;
            notedossier.Idnote = Guid.NewGuid().ToString(); //Ajouter un id à la note
            if (ModelState.IsValid)
            {
                await _services.CreateAsync(notedossier);
                return RedirectToAction(nameof(Index));
            }
            return View(notedossier);
        }

        // GET: Notedossiers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notedossier = await _services.GetByIdAsync(id);
            if (notedossier == null)
            {
                return NotFound();
            }
            return View(notedossier);
        }

        // POST: Notedossiers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Note,Idnote")] Notedossier notedossier)
        {
            if (id != notedossier.Idnote)
            {
                return NotFound();
            }
            //On prend l'Id de la session. S'il n'y en a pas, alors le ModelState ne sera pas valide
            notedossier.IdEmbarcation = HttpContext.Session.GetString("IdEmbarcation");
            notedossier.Date = DateTime.Now;
            if (ModelState.IsValid)
            {
                try
                {
                    await _services.UpdateAsync(notedossier);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await NotedossierExists(notedossier.Idnote))
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
            return View(notedossier);
        }

        // GET: Notedossiers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notedossier = await _services.GetByIdAsync(id);
            if (notedossier == null)
            {
                return NotFound();
            }

            return View(notedossier);
        }

        // POST: Notedossiers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _services.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> NotedossierExists(string id)
        {
            return await _services.IsExist(id);
        }

        //Méthode pour prendre l'Id de la chose sur laquelle on veut ajouter une note sur laquelle on veut ajouter une note
        private string GetId(string IdEmbarcation)
        {
            //Vérifier si un Id a déjà été enregistré dans la session actuelle si non, on met l'Id reçu en paramètre comme Id de session. Si oui, on le prend et on le retourne comme id
            var id = HttpContext.Session.GetString("IdEmbarcation");
            if (id == null || IdEmbarcation != null)
            {
                HttpContext.Session.SetString("IdEmbarcation", IdEmbarcation);
            }
            else if (id != null || IdEmbarcation == null)
            {
                IdEmbarcation = id;
            }
            return IdEmbarcation;
        }
    }
}
