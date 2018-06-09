using System.Collections.Generic;
using Newtonsoft.Json.Serialization;

namespace FirebaseNetStandardAdmin.Serializations
{
    public class FirebaseAccessTokenContractResolver : DefaultContractResolver
    {
        private static readonly Dictionary<string, string> _propertyMappings = new Dictionary<string, string>
        {
            { "AccessToken","access_token"},
            { "TokenType", "token_type"},
            { "ExpiresIn", "expires_in"}
        };

        protected override string ResolvePropertyName(string propertyName)
        {
            return !_propertyMappings.TryGetValue(propertyName, out var resolvedName) ? base.ResolvePropertyName(propertyName) : resolvedName;
        }
    }
}
