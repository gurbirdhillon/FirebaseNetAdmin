using System;
using System.Collections.Generic;
using FirebaseNetStandardAdmin.Extensions;
using FirebaseNetStandardAdmin.HttpClients;

namespace FirebaseNetStandardAdmin.Firebase.Database
{
    public class FirebaseAdminRef : IFirebaseAdminRef
    {
        private readonly List<KeyValuePair<string, string>> _queryStore = new List<KeyValuePair<string, string>>();

        public IFirebaseHttpClient HttpClient { get; }

        public string Path { get; }

        public FirebaseAdminRef(IFirebaseHttpClient httpClient, string refPath)
        {
            if (string.IsNullOrWhiteSpace(refPath))
            {
                throw new ArgumentNullException(nameof(refPath));
            }
            var normalizedPath = $"{refPath.TrimSlashes()}.json";
            HttpClient = httpClient;
            Path = normalizedPath;
        }

        public void Add(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            _queryStore.Add(new KeyValuePair<string, string>(key, value));
        }

        public void AddBool(string key, bool value)
        {
            var valueString = value ? "true" : "false";
            Add(key, valueString);
        }

        public void AddString(string key, string value) => Add(key, $"\"{value}\"");

        public IList<KeyValuePair<string, string>> GetQueryStore() => _queryStore;
    }
}
