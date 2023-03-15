using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using FlyShoes.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.DAL.Implements
{
    public class FirebaseService : IFirebaseAuthClient
    {
        FirebaseAuthClient _firebaseProvider;
        private readonly static string API_KEY = "AIzaSyCEtfd42e7DeEwY_TK5JNUJWjqXbymIaP0";

        public FirebaseService()
        {
            var config = new FirebaseAuthConfig();
            config.ApiKey = API_KEY;
            config.AuthDomain = "fly-shoes-store.firebaseapp.com";
            config.Providers = new FirebaseAuthProvider[]
            {
                new GoogleProvider().AddScopes("email"),
                new EmailProvider()
            };
            config.UserRepository = new FileUserRepository("LoginData");

            _firebaseProvider = new FirebaseAuthClient(config);
        }
        public User User => _firebaseProvider.User;

        public event EventHandler<UserEventArgs> AuthStateChanged;

        public async Task<UserCredential> CreateUserWithEmailAndPasswordAsync(string email, string password, string displayName = null)
        {
            return await _firebaseProvider.CreateUserWithEmailAndPasswordAsync(email, password, displayName);
        }

        public Task<FetchUserProvidersResult> FetchSignInMethodsForEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task ResetEmailPasswordAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<UserCredential> SignInAnonymouslyAsync()
        {
            throw new NotImplementedException();
        }

        public Task<UserCredential> SignInWithCredentialAsync(AuthCredential credential)
        {
            throw new NotImplementedException();
        }

        public async Task<UserCredential> SignInWithEmailAndPasswordAsync(string email, string password)
        {
            return await _firebaseProvider.SignInWithEmailAndPasswordAsync(email,password);
        }

        public Task<UserCredential> SignInWithRedirectAsync(FirebaseProviderType authType, SignInRedirectDelegate redirectDelegate)
        {
            throw new NotImplementedException();
        }

        public void SignOut()
        {
            _firebaseProvider.SignOut();
        }
    }
}
