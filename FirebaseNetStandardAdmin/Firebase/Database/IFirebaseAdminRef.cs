using System.Collections.Generic;
using FirebaseNetStandardAdmin.HttpClients;

namespace FirebaseNetStandardAdmin.Firebase.Database
{
    public interface IFirebaseAdminRef
    {
        IFirebaseHttpClient HttpClient { get; }

        string Path { get; }

        void Add(string key, string value);

        void AddBool(string key, bool value);

        void AddString(string key, string value);

        IList<KeyValuePair<string, string>> GetQueryStore();
    }
}
