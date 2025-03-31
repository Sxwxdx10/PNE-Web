using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using PNE_admin.Annotations;
using PNE_core.Models;
using PNE_core.Services.Interfaces;
using PNE_core.Models;
using Newtonsoft.Json;
using NetTopologySuite.Index.HPRtree;
using PNE_core.Enums;
using PNE_core.DTO;
using PNE_core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace PNE_admin.Controllers
{
    [RolesAuthentication("gerant")]
    public class GerantController : Controller
    {
        //pour ajouter ses employes
        private readonly IFirebaseAuthService _authService;
        private readonly IUtilisateurService _userServices;
        private readonly IRoleService _roleService;

        private readonly IPlanEauService _eauService;
        private readonly ICertificationService _certificationService;
        private readonly IMiseAEauService _miseAEauService;
        private readonly IStationLavageService _stationLavageService;
        private readonly IEEEService _eeeService;

        public GerantController(IFirebaseAuthService authService, IUtilisateurService userService, IRoleService roleService, IPlanEauService eauService, ICertificationService certificationService, IMiseAEauService miseAEauService, IStationLavageService stationLavageService, IEEEService eeeService)
        {
            _authService = authService;
            _userServices = userService;
            _eauService = eauService;
            _certificationService = certificationService;
            _miseAEauService = miseAEauService;
            _roleService = roleService;
            _stationLavageService = stationLavageService;
            _eeeService = eeeService;
        }

        /// <summary>
        /// inscription de comptes employes a son plan d'eau
        /// </summary>
        public async Task<IActionResult> InscriptionEmployes()
        {
            ViewData["Title"] = "Inscription";
            return View();
        }

        /// <summary>
        /// Retour de l'inscription, création de l'utilisateur et assignation du role
        /// </summary>
        /// <param name="user">utilisateur le gérant</param>
        [Authorize(Roles =Roles.Gerant)]
        [HttpPost]
        public async Task<IActionResult> InscriptionEmployes(SignUpUserDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            //ajout de l'employer dans firebase, get back le uid
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

                //assigner le role "employe" au user
                await _roleService.AddUserRole(uInfo.Uid, "employe");

                return RedirectToAction("Index", "AccueilAdmin");
            }

            return BadRequest();
        }

        /// <summary>
        /// carte ou l'on peut voir son plan d'eau avec la possibilite de voir et ajouter des stations de lavage.
        /// </summary>
        public async Task<IActionResult> GestionPlanEau()
        {
            var serialisedUser = HttpContext.Session.GetString("currentUser")!;
            Utilisateur utilisateur = JsonConvert.DeserializeObject<Utilisateur>(serialisedUser)!;

            try
            {
                var plansDeau = await _userServices.GetUserPlanEau(utilisateur.Id, true);
                ViewBag.PlanEaux = plansDeau;

                // Récupérer les stations de lavage pour chaque plan d'eau
                var stations = new List<PNE_core.Models.StationLavage>();
                foreach (var planEau in plansDeau)
                {
                    var stationsDuPlan = await _stationLavageService.GetAllAsync();
                    stations.AddRange(stationsDuPlan.Where(s => s.planeau?.IdPlanEau == planEau.IdPlanEau));
                }
                ViewBag.Stations = stations;

                //si plansDeau a eu un plan d'eau, alors on retourne un point vers celui-ci
                ViewData["Title"] = "Mon plan d'eau";
                return View(plansDeau[0]);
            }
            catch (InvalidOperationException e)
            {
                //return une autre view pour ajouter son plan d'eau car il n'y en a pas de definie
                //(en ce moment, juste retourner la vue d'un lac pour des fins de test)
                return View(new Planeau
                {
                    Emplacement = new NetTopologySuite.Geometries.Point(new NetTopologySuite.Geometries.Coordinate(48.587, -72.04))
                }); 
            }
        }
        
         public async Task<IActionResult> MiseAEau()
         {
            var serialisedUser = HttpContext.Session.GetString("currentUser")!;
            Utilisateur utilisateur = JsonConvert.DeserializeObject<Utilisateur>(serialisedUser)!;

            try
            {
                var planeaux = await _userServices.GetUserPlanEau(utilisateur.Id, true);
                ViewData["planeau"] = new SelectList(planeaux, "IdPlanEau", "Nom");
                ViewData["Title"] = "Mise a l'eau";
                return View();
            }
            catch (InvalidOperationException e) {
                TempData["Erreur"] = "Vous n'avez pas de plans d'eau";
                return RedirectToAction("Index", "AccueilAdmin");
            }
         }


        /// <summary>
        /// Effectuer des mises a l'eau
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MiseAEau(string idEmbarcation, string IdPlanEau, int dureeEnJours)
        {
            TempData["ErreurMiseEau"] = null;
            Miseaeau miseAEau = new Miseaeau();
            miseAEau.IdEmbarcation = idEmbarcation;
            miseAEau.IdPlanEau = IdPlanEau;
            miseAEau.IdMiseEau = Guid.NewGuid().ToString();
            miseAEau.Date = DateTime.Now;
            miseAEau.DureeSejourEnJours = dureeEnJours;
            if (miseAEau != null)
            {
                try
                {
                    await _miseAEauService.CreateAsync(miseAEau);
                }
                catch (Exception ex)
                {
                    if (IdPlanEau == null) 
                        TempData["ErreurMiseEau"] = "Sélectionnez un plan d'eau";
                    else
                        TempData["ErreurMiseEau"] = "Cette embarcation n'existe pas";
                    return RedirectToAction("MiseAEau");
                }
            }    
            ViewData["Title"] = "Mise a l'eau";
            return RedirectToAction("Index", "AccueilAdmin");
        }

        /// <summary>
        /// Page pour signaler a un administrateur de la CREE que son plan d'eau a une EEE
        /// L'EEE sera en attente de confirmation
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> SignalisationEEE()
        {
            var serialisedUser = HttpContext.Session.GetString("currentUser")!;
            Utilisateur utilisateur = JsonConvert.DeserializeObject<Utilisateur>(serialisedUser)!;

            try
            {
                var plansEau = await _userServices.GetUserPlanEau(utilisateur.Id, true);
                ViewData["Title"] = "Signaler une EEE";
                ViewData["PlanEaux"] = new SelectList(plansEau, "IdPlanEau", "Nom");
                return View("SelectPlanEau");
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = "Vous n'avez pas de plans d'eau assignés";
                return RedirectToAction("Index", "AccueilAdmin");
            }
        }

        [HttpGet]
        [Route("Gerant/SignalisationEEE/{id}")]
        public async Task<IActionResult> SignalisationEEEDetail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "Veuillez sélectionner un plan d'eau";
                return RedirectToAction("SignalisationEEE");
            }

            var serialisedUser = HttpContext.Session.GetString("currentUser")!;
            Utilisateur utilisateur = JsonConvert.DeserializeObject<Utilisateur>(serialisedUser)!;

            try
            {
                var plansEau = await _userServices.GetUserPlanEau(utilisateur.Id, true);
                var planEau = plansEau.FirstOrDefault(p => p.IdPlanEau == id);

                if (planEau == null)
                {
                    TempData["ErrorMessage"] = "Plan d'eau non trouvé ou non autorisé";
                    return RedirectToAction("SignalisationEEE");
                }

                // Inclure les EEE associées au plan d'eau avec leurs relations
                planEau = await _eauService.GetByIdAsync(id);
                if (planEau == null)
                {
                    TempData["ErrorMessage"] = "Plan d'eau non trouvé";
                    return RedirectToAction("SignalisationEEE");
                }

                ViewBag.EEEs = await _eeeService.GetAllAsync();
                ViewData["Title"] = "Signaler une EEE";
                return View("SignalisationEEE", planEau);
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = "Vous n'avez pas accès à ce plan d'eau";
                return RedirectToAction("Index", "AccueilAdmin");
            }
        }

        /// <summary>
        /// quelques statistiques sur le plan d'eau
        /// ex : lac d'ou les bateaus viennent, utilisation des stations de lavages... (de son plan d'eau)
        /// </summary>
        public async Task<IActionResult> Stats()
        {
            ViewData["Title"] = "Statistiques";
            return View();
        }
        /// <summary>
        /// Creation plan d'eau
        /// 
        /// </summary>
        public async Task<IActionResult> NouveauPlan()
        {
            ViewData["Title"] = "Nouveau Plan d'eau";
            return View("Planeaux/Create");
        }

        [HttpPost]
        public async Task<IActionResult> SignalerEEE(string Name, string Description, Niveau NiveauCouleur, string IdPlanEau)
        {
            try
            {
                var serialisedUser = HttpContext.Session.GetString("currentUser")!;
                Utilisateur utilisateur = JsonConvert.DeserializeObject<Utilisateur>(serialisedUser)!;

                // Créer une nouvelle EEE
                var eee = new EEE
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = Name,
                    Description = Description,
                    NiveauCouleur = NiveauCouleur,
                    IdSignaleur = utilisateur.Id
                };

                // Sauvegarder l'EEE
                await _eeeService.CreateAsync(eee);

                // Associer l'EEE au plan d'eau
                await _eeeService.SignalerEEE(eee.Id, IdPlanEau);
                TempData["SuccessMessage"] = "EEE créée et signalée avec succès.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(SignalisationEEEDetail), new { id = IdPlanEau });
        }
    }
}
