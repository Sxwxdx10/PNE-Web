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
using PNE_core.DTO;
using PNE_core.Services;
using System.Data;

namespace PNE_admin.Controllers
{
    public class UtilisateursController : Controller
    {
        private readonly PneContext _context;
        private readonly IUtilisateurService _services;
        private readonly IRoleService _roleServices;
        private readonly IFirebaseAuthService _authService;

        public UtilisateursController(PneContext context, IUtilisateurService pneServices, IFirebaseAuthService authService, IRoleService roleServices)
        {
            _context = context;
            _services = pneServices;
            _authService = authService;
            _roleServices = roleServices;
        }

        // GET: Utilisateurs
        public async Task<IActionResult> Index()
        {
            //Set up la liste pour les rôles
            var roles = await _roleServices.GetAllAsync();

            // Crée une liste de rôles à afficher
            var items = roles.Select(role => new SelectListItem
            {
                Text = role.NomRole,
                Value = role.NomRole
            }).ToList();

            var multiSelectList = new MultiSelectList(items, "Value", "Text");

            // On met la liste de rôles dans un ViewBag
            ViewBag.Items = multiSelectList;

            return View(await _services.GetAllAsync());
        }

        // GET: Utilisateurs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilisateur = await _services.GetByIdAsync(id);
            if (utilisateur == null)
            {
                return NotFound();
            }

            return View(utilisateur);
        }

        // GET: Utilisateurs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Utilisateurs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SignUpUserDTO userDto)
        {
            if (ModelState.IsValid)
            {

                //Création de l'utilisateur dans FireBase
                UserBaseInfo? uInfo = await _authService.SignUp(userDto!.Email, userDto!.Password, userDto.Username);

                //Création de l'utilisateur pour l'ajouter dans la BD
                Utilisateur utilisateur = new Utilisateur();
                utilisateur.Id = uInfo.Uid;
                utilisateur.DisplayName = uInfo.Name;
                utilisateur.Email = userDto!.Email;
                utilisateur.DateCreation = DateTime.Now;

                //On ajoute le nouvel utilisateur créé dans la BD
                await _services.CreateAsync(utilisateur);
                return RedirectToAction(nameof(Index));
            }
            return View(userDto);
        }

        // GET: Utilisateurs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }


            //Set up la liste pour les rôles
            var roles = await _roleServices.GetAllAsync();

            // Crée une liste de rôles à afficher
            var items = roles.Select(role => new SelectListItem
            {
                Text = role.NomRole,
                Value = role.NomRole
            }).ToList();

            var multiSelectList = new MultiSelectList(items, "Value", "Text");

            // On met la liste de rôles dans un ViewBag
            ViewBag.Items = multiSelectList; 
            List<string> listeroles = new List<string>();

            //Prendre les rôles actuel de l'utilisateur
            var roleActuel = await _roleServices.GetUserRoles(id);
            if (roleActuel != null)
            {
               
                foreach (var role in roleActuel)
                {
                    listeroles.Add(role.NomRole);
                }
            }
            ViewBag.Roles = listeroles;

            var utilisateur = await _services.GetByIdAsync(id);
            if (utilisateur == null)
            {
                return NotFound();
            }

            return View(utilisateur);
        }

        // POST: Utilisateurs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,DisplayName,Email")] Utilisateur utilisateur)
        {
            if (id != utilisateur.Id)
            {
                return NotFound();
            }
            utilisateur.DateCreation = DateTime.Now;
            if (ModelState.IsValid)
            {
                try
                {
                    await _services.UpdateAsync(utilisateur);

                    var selectedRoles = Request.Form["RolesUtilisateurs"].ToList(); //On prend les rôles à modifier

                    //Liste des rôles de l'utilisateur courant
                    var userRole = await _roleServices.GetUserRoles(id);

                    //Nouvelle liste qui contient le nom des rôles
                    List<string> listeroles = new List<string>();
                    foreach (var role in userRole)
                    {
                        listeroles.Add(role.NomRole);
                    }

                    if (selectedRoles.Count > 0 && userRole.Count != 0)
                    {
                        for (int i = listeroles.Count - 1; i >= 0; i--)
                        {
                            foreach (var selectedRole in selectedRoles)
                            {
                                if (!listeroles.Contains(selectedRole))
                                {
                                    await _roleServices.AddUserRole(id, selectedRole);
                                    listeroles.Add(selectedRole);
                                }
                                if (!selectedRoles.Contains(listeroles[i]))
                                {
                                    await _roleServices.RemoveUserRole(id, listeroles[i]);
                                }
                            }

                        }
                    }
                    else
                    {
                        if (userRole.Count > 0)
                        {
                            foreach (var role in userRole)
                            {
                                await _roleServices.RemoveUserRole(id, role.NomRole);
                            }
                        }
                        else if (userRole.Count == 0)
                        {
                            foreach(var role in selectedRoles)
                            {
                                await _roleServices.AddUserRole(id, role);
                            }
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await UtilisateurExists(utilisateur.Id))
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
            //Set up la liste pour les rôles
            var roles = await _roleServices.GetAllAsync();

            // Crée une liste de rôles à afficher
            var items = roles.Select(role => new SelectListItem
            {
                Text = role.NomRole,
                Value = role.NomRole
            }).ToList();

            var multiSelectList = new MultiSelectList(items, "Value", "Text");

            // On met la liste de rôles dans un ViewBag
            ViewBag.Items = multiSelectList;
            return View(utilisateur);
        }

        // GET: Utilisateurs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilisateur = await _services.GetByIdAsync(id);
            if (utilisateur == null)
            {
                return NotFound();
            }

            return View(utilisateur);
        }

        // POST: Utilisateurs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _services.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> UtilisateurExists(string id)
        {
            return await _services.IsExist(id);
        }
    }
}
