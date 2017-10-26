using System;
using System.Threading.Tasks;
using FirebaseNetAdmin.HttpClients;

namespace FirebaseNetAdmin.Firebase.Auth
{
    public interface IFirebaseAdminAuth : IDisposable
    {
        void Authenticate();
        Task AuthenticateAsync();
        void AddFirebaseHttpClient(IFirebaseHttpClient client);
        string CreateCustomToken(long userId);
    }
}
