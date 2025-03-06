using Microsoft.AspNetCore.Mvc;
using PNE_core.DTO;
using PNE_core.Models;
using PNE_core.Services.Interfaces;
using Newtonsoft.Json;

namespace PNE_admin.Controllers
{
    public class AuthentificationController : Controller
    {
        private readonly IFirebaseAuthService _authService;
        private readonly IUtilisateurService _userServices;
        private readonly IConfiguration _config;

        public AuthentificationController(IFirebaseAuthService authService, IUtilisateurService userService, IConfiguration config)
        {
            _authService = authService;
            _userServices = userService;
            _config = config;
        }

        public ActionResult Login()
        {
            ViewData["Title"] = "Connexion";
            ViewData["GoogleClientId"] = _config["GOOGLE_ID"];
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            UserBaseInfo? uInfo = await _authService.Login(userDto!.Email, userDto!.Password);
            if (uInfo is not null)
            {
                await setVariablesAfterConnexion(uInfo);

                return RedirectToAction("Index", "AccueilAdmin");
            }
            return BadRequest();
        }

        [HttpPost("auth/googleLogin")]
        public async Task<IActionResult> GoogleLogin([FromBody] string code)
        {
            UserBaseInfo? uInfo = await _authService.GoogleLogin(code);
            if (uInfo is not null)
            {
                await setVariablesAfterConnexion(uInfo);
                //redirect gere par la page de connexion
                return Ok();
            }

            //ne devrait pas arriver; google doit prendre tout les incoming users
            return BadRequest($"Authentification au server refuse");
        }

        public ActionResult logout()
        {
            _authService.SignOut();
            HttpContext.Session.Remove("token");
            HttpContext.Session.Remove("username");
            HttpContext.Session.Remove("currentUser");
            HttpContext.Session.Remove("userRoles");
            ViewData["Title"] = "Deconnexion reussi";
            return View();
        }

        private async Task setVariablesAfterConnexion(UserBaseInfo uInfo)
        {
            uInfo.Name = uInfo.Name ?? uInfo.Email.Split('@')[0];
            //set token et username pour utilisation dans le frontEnd de l'app
            HttpContext.Session.SetString("token", uInfo.Token);
            HttpContext.Session.SetString("username", uInfo.Name);

            //gestion de la partie user dans la BD
            Utilisateur? utilisateur = await _userServices.GetByIdAsync(uInfo.Uid);
            if (utilisateur is null)
            {
                //Création de l'utilisateur pour l'ajouter dans la BD
                utilisateur = new Utilisateur();
                utilisateur.Id = uInfo.Uid;
                utilisateur.DisplayName = uInfo.Name;
                utilisateur.DateCreation = DateTime.Now;
                utilisateur.Email = uInfo.Email!;

                //On ajoute le nouvel utilisateur créé dans la BD
                await _userServices.CreateAsync(utilisateur);
            }
            HttpContext.Session.SetString("currentUser", JsonConvert.SerializeObject(utilisateur));
        }
    }
}
