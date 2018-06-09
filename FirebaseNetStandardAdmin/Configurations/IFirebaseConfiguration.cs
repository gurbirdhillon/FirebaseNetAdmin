using System;
using System.Collections.Generic;
using FirebaseNetStandardAdmin.Encryption.JWT;

namespace FirebaseNetStandardAdmin.Configurations
{
    public interface IFirebaseConfiguration
    {
        Uri GoogleOAuthTokenPath { get; }
        Uri CustomTokenPath { get; }
        TimeSpan AccessTokenTTL { get; }
        TimeSpan CustomTokenTTL { get; }
        IList<string> AllowedGoogleScopes { get; }
        string GoogleScopeDelimiter { get; }
        IJWTProvider JwtTokenProvider { get; }
        string FirebaseHost { get; }
        string StorageBaseAuthority { get; }
        string StorageBaseAuthority2 { get; }
    }
}
