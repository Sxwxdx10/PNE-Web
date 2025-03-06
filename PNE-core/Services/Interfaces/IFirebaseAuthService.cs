using Firebase.Auth;
using Google.Apis.Auth;
using PNE_core.Models;

namespace PNE_core.Services.Interfaces
{
    public interface IFirebaseAuthService
    {
        Task<UserBaseInfo?> SignUp(string email, string password, string username);

        Task<UserBaseInfo?> Login(string email, string password);

        Task<UserBaseInfo?> GoogleLogin(string idToken);

        void SignOut();
    }
}