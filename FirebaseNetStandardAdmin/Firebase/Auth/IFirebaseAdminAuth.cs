using System;
using System.Threading.Tasks;
using FirebaseNetStandardAdmin.HttpClients;

namespace FirebaseNetStandardAdmin.Firebase.Auth
{
    public interface IFirebaseAdminAuth : IDisposable
    {
        void Authenticate();
        Task AuthenticateAsync();
        void AddFirebaseHttpClient(IFirebaseHttpClient client);
        string CreateCustomToken(long userId);
    }
}
