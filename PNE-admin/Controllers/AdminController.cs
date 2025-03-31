using FirebaseAdmin.Auth.Multitenancy;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using PNE_admin.Annotations;
using PNE_core.DTO;
using PNE_core.Models;
using PNE_core.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace PNE_admin.Controllers
{
    [RolesAuthentication("admin")]
    public class AdminController : Controller
    {
        private readonly IFirebaseAuthService _authService;
        private readonly IUtilisateurService _userServices;
        private readonly IRoleService _roleService;
        private readonly IPlanEauService _eauService;
        private readonly ICertificationService _certificationService;
        private readonly IPneDbContext _db;
        private readonly IEEEService _eeeService;

        public AdminController(IFirebaseAuthService authService, IUtilisateurService userService, 
            IPlanEauService eauService, ICertificationService certificationService, IRoleService roleService,
            IPneDbContext db, IEEEService eeeService)
        {
            _authService = authService;
            _userServices = userService;
            _eauService = eauService;
            _certificationService = certificationService;
            _roleService = roleService;
            _db = db;
            _eeeService = eeeService;
        }

        /// <summary>
        /// inscription de gerants de plans d'eau.
        /// </summary>
        public async Task<IActionResult> InscriptionGerant()
        {
            ViewData["Title"] = "Inscription";
            ViewBag.PlanEaux = await _eauService.GetAllAsync();
            return View();
        }

        /// <summary>
        /// retour de l'inscription, ajouter le user en BD et a firebase puis lui assigner le bon role
        /// </summary>
        /// <param name="user">utilisateur cree par l'admin</param>
        [HttpPost]
        public async Task<IActionResult> InscriptionGerant(SignUpUserDTO userDTO, List<string> SelectedPlanEaux)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.PlanEaux = await _eauService.GetAllAsync();
                return View();
            }

            //ajout du user dans firebase, get back le uid
            UserBaseInfo? uInfo = await _authService.SignUp(userDTO.Email, userDTO.Password, userDTO.Username);
            if (uInfo is not null)
            {
                //cree le user dans la bd avec le uid de firebase
                Utilisateur user = new()
                {
                    Id = uInfo.Uid,
                    Email = uInfo.Email!,
                    DisplayName = uInfo.Name
                };
                await _userServices.CreateAsync(user);

                //assigner le role "gerant" au user
                await _roleService.AddUserRole(uInfo.Uid, "gerant");

                // Lier l'utilisateur aux plans d'eau sélectionnés
                foreach (var planEauId in SelectedPlanEaux)
                {
                    await _userServices.LinkUserPlanEau(uInfo.Uid, planEauId);
                }

                return RedirectToAction("Index", "AccueilAdmin");
            }

            ViewBag.PlanEaux = await _eauService.GetAllAsync();
            return View();
        }

        /// <summary>
        /// ! les EEE ne sont pas encore implemente, donc ceci ne sert a rien avant que la partie des EEE pour les gerants de plans d'eau soit fait !
        /// 
        /// Les Administrateurs veulent pouvoir confirmer les EEE/EAE des plans d'eau
        /// </summary>
        public async Task<IActionResult> ConfirmationEEE()
        {
            ViewData["Title"] = "Confirmation EEEs";
            var eeePlanEaux = await _db.EEEPlanEaus
                .Include(e => e.EEE)
                    .ThenInclude(e => e.Signaleur)
                .Include(e => e.PlanEauNavigation)
                .OrderByDescending(e => !e.Validated)
                .ToListAsync();
            return View(eeePlanEaux);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmerEEE(string IdEEE, string IdPlanEau)
        {
            try
            {
                await _eeeService.ConfirmerEEE(IdEEE, IdPlanEau);
                TempData["SuccessMessage"] = "EEE confirmée avec succès.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(ConfirmationEEE));
        }

        [HttpPost]
        public async Task<IActionResult> RetirerEEE(string IdEEE, string IdPlanEau)
        {
            try
            {
                await _eeeService.RetirerEEE(IdEEE, IdPlanEau);
                TempData["SuccessMessage"] = "EEE retirée avec succès.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(ConfirmationEEE));
        }

        /// <summary>
        /// un admin veut pouvoir voir les plans d'eau et confirmer un niveau (vert, jaune, rouge) dependament des EEE qu'il a evalue
        /// il veut aussi pouvoir evaluer les stations de lavages du plan d'eau
        /// </summary>
        public async Task<IActionResult> Planseau()
        {
            ViewData["Title"] = "Plans d'eau";
            return RedirectToAction("Index", "Planeaux"); ;
        }

        /// <summary>
        /// ici je crois que l'on veut pouvoir creer des formations et aider les gerants a donner ces formations
        /// a confirmer avec le client
        /// </summary>
        public async Task<IActionResult> FormationsCertifications()
        {
            ViewData["Title"] = "Formations et certifications";
            return View();
        }
    }
}
