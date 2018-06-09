using System;
using System.Collections.Generic;
using FirebaseNetStandardAdmin.Configurations.ServiceAccounts;
using FirebaseNetStandardAdmin.Extensions;

namespace FirebaseNetStandardAdmin.Configurations.AuthPayload
{
    public class JwtAuthPayloadGenerator : PayloadGenerator
    {
        private readonly IServiceAccountCredentials _creadentials;
        private readonly IFirebaseConfiguration _configuration;

        public JwtAuthPayloadGenerator(IServiceAccountCredentials credentials, IFirebaseConfiguration configuration)
        {
            _creadentials = credentials;
            _configuration = configuration;
        }

        public sealed override IDictionary<string, string> GetPayload(IDictionary<string, string> additionalPayload = null)
        {
            var iat = DateTime.Now.ToUnixSeconds();
            var exp = (DateTime.Now + _configuration.AccessTokenTTL).ToUnixSeconds();

            AddToPayload("scope", string.Join<string>(_configuration.GoogleScopeDelimiter, _configuration.AllowedGoogleScopes));
            AddToPayload("iss", _creadentials.GetServiceAccountEmail());
            AddToPayload("aud", _configuration.GoogleOAuthTokenPath.AbsoluteUri);
            AddToPayload("exp", exp.ToString());
            AddToPayload("iat", iat.ToString());

            return base.GetPayload(additionalPayload);
        }
    }
}
