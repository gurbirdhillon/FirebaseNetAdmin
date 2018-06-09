using System;
using FirebaseNetStandardAdmin.Configurations;
using FirebaseNetStandardAdmin.Configurations.ServiceAccounts;
using FirebaseNetStandardAdmin.Firebase.Auth;
using FirebaseNetStandardAdmin.HttpClients;

namespace FirebaseNetStandardAdmin.Firebase.Database
{
    public class FirebaseAdminDatabase : IFirebaseAdminDatabase, IDisposable
    {
        private readonly IFirebaseHttpClient _httpClient;

        public FirebaseAdminDatabase(IFirebaseAdminAuth auth, IServiceAccountCredentials credentials)
        {
            var firebaseConfiguration = new DefaultFirebaseConfiguration(GoogleServiceAccess.DatabaseOnly);

            var firebaseAuthority = new Uri($"https://{credentials.GetProjectId()}.{firebaseConfiguration.FirebaseHost}/", UriKind.Absolute);
            _httpClient = new FirebaseHttpClient(credentials, firebaseConfiguration, firebaseAuthority);

            auth.AddFirebaseHttpClient(_httpClient);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IFirebaseAdminRef Ref(string path) => new FirebaseAdminRef(_httpClient, path);

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _httpClient.Dispose();
            }
        }

        ~FirebaseAdminDatabase() => Dispose(false);
    }
}
