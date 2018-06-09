using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FirebaseNetStandardAdmin.HttpClients;

namespace FirebaseNetStandardAdmin.Firebase.Auth
{
    public interface IFirebaseAdminAuth : IDisposable
    {
        void Authenticate();
        Task AuthenticateAsync();
        void AddFirebaseHttpClient(IFirebaseHttpClient client);
        string CreateCustomToken(long userId, IDictionary<string, string> additionalClaims = null);
        string CreateCustomToken(string userId, IDictionary<string, string> additionalClaims = null);
    }
}
