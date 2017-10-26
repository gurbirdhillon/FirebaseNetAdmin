using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirebaseNetAdmin.Exceptions;
using FirebaseNetAdmin.HttpClients;

namespace FirebaseNetAdmin.Firebase.Auth
{
    public class FirebaseAdminAuth : IFirebaseAdminAuth
    {
        private readonly IList<IFirebaseHttpClient> _httpClients = new List<IFirebaseHttpClient>();

        public void AddFirebaseHttpClient(IFirebaseHttpClient client)
        {
            if (client != null)
            {
                _httpClients.Add(client);
            }
        }

        public void Authenticate() => AuthenticateAsync().Wait();

        public async Task AuthenticateAsync() => await Task.WhenAll(_httpClients.Select(client => client.Send2LOTokenRequestAsync()));

        public string CreateCustomToken(long userId)
        {
            if (_httpClients == null || _httpClients.Count == 0)
                throw new FirebaseHttpException("No initialzied clients were found");

            return _httpClients.First().CreateCustomToken(userId);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            foreach (var client in _httpClients)
                client.Dispose();
        }

        ~FirebaseAdminAuth() => Dispose(false);
    }
}
