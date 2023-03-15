using Firebase.Auth;
using Firebase.Auth.Providers;
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
        public User User => throw new NotImplementedException();

        public event EventHandler<UserEventArgs> AuthStateChanged;

        public Task<UserCredential> CreateUserWithEmailAndPasswordAsync(string email, string password, string displayName = null)
        {
            throw new NotImplementedException();
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

        public Task<UserCredential> SignInWithEmailAndPasswordAsync(string email, string password)
        {
            throw new NotImplementedException();
        }

        public Task<UserCredential> SignInWithRedirectAsync(FirebaseProviderType authType, SignInRedirectDelegate redirectDelegate)
        {
            throw new NotImplementedException();
        }

        public void SignOut()
        {
            throw new NotImplementedException();
        }
    }
}
