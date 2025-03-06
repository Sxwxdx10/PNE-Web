using Firebase.Auth;
using PNE_core.Services.Interfaces;
using PNE_core.Models;
using Firebase.Auth.Providers;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;

namespace PNE_core.Services
{
    public class FirebaseAuthService : IFirebaseAuthService
    {
        private readonly FirebaseAuthClient _firebaseAuth;
        private readonly IConfiguration _configuration; 

        public FirebaseAuthService(FirebaseAuthClient firebaseAuth, IConfiguration appconfig)
        {
            _firebaseAuth = firebaseAuth;
            _configuration = appconfig;
        }

        /// <summary>
        /// signup par email, utile pour quand un admin ajoute un utilisateur dans firebase
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="username"></param>
        /// <returns>un UserBaseInfo qui contient un token et ce qui sera l'id du user dans la BD</returns>
        public async Task<UserBaseInfo?> SignUp(string email, string password, string? username)
        {
            var userCredentials = await _firebaseAuth.CreateUserWithEmailAndPasswordAsync(email, password, username);

            return await InfoFormatter(userCredentials.User);
        }

        /// <summary>
        /// login un user par son email et mot de passe a firebase
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns>UserBaseInfo avec le token et l'id dans la db du user</returns>
        public async Task<UserBaseInfo?> Login(string email, string password)
        {
            var userCredentials = await _firebaseAuth.SignInWithEmailAndPasswordAsync(email, password);

            return await InfoFormatter(userCredentials.User);
        }

        /// <summary>
        /// login par google
        /// la connexion a firebase par google overide un user qui est register par email
        /// </summary>
        /// <param name="code">code de connexion google</param>
        /// <returns>UserBaseInfo avec le token et l'id dans la db du user</returns>
        public async Task<UserBaseInfo?> GoogleLogin(string code)
        {
            var token = await GoogleTokenTranslator(code);
            if (token == null)
            {
                return null;
            }
            var credentials = GoogleProvider.GetCredential(token);
            try
            {
                var userCredentials = await _firebaseAuth.SignInWithCredentialAsync(credentials);
                return await InfoFormatter(userCredentials.User);
            }
            catch (Exception e) { 
                return null;
            }
        }

        /// <summary>
        /// signout le user dans firebase, invalid le token
        /// </summary>
        public void SignOut() => _firebaseAuth.SignOut();

        /// <summary>
        /// prend le code donne par une connexion google
        /// traduit ce code en token d'acces pour firebase
        /// </summary>
        /// <param name="code">code de connexion google</param>
        /// <returns>token d'access pour firebase</returns>
        private async Task<string?> GoogleTokenTranslator(string code)
        {
            try
            {
                // Exchange the authorization code for an ID token
                var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = new ClientSecrets
                    {
                        ClientId = _configuration["GOOGLE_ID"],
                        ClientSecret = _configuration["GOOGLE_SECRET"]
                    },
                    Scopes = ["email", "profile"], // Scopes requested during authentication
                });
                var result = await flow.ExchangeCodeForTokenAsync(null, code, "postmessage", CancellationToken.None);
                // Extract the Access token from the result
                // Access token lets us identify wich google user is trying to log in
                return result.AccessToken;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// change le retour d'info de firebase en prenant seulement les infos voulu
        /// methode utile seulement dans ce service
        /// </summary>
        /// <param name="u">user firebase</param>
        /// <returns>UserBaseInfo avec le token et l'id dans la db du user</returns>
        private static async Task<UserBaseInfo?> InfoFormatter(User u)
        {
            if (u == null) return null;
            string username = u.Info.DisplayName == "" ? u.Info.Email : u.Info.DisplayName;
            var uid = u.Info.Uid;
            var email = u.Info.Email;
            return new UserBaseInfo(username, await u.GetIdTokenAsync(), uid, email);
        }
    }
}
