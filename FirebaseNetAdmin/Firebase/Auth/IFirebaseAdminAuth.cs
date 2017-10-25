namespace FirebaseNetAdmin.Firebase.Auth
{
    using FirebaseNetAdmin.HttpClients;
    using System;
    using System.Threading.Tasks;

    public interface IFirebaseAdminAuth : IDisposable
    {
        void Authenticate();
        Task AuthenticateAsync();
        void AddFirebaseHttpClient(IFirebaseHttpClient client);
        string CreateCustomToken(long userId);
    }
}