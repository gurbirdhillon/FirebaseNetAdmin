using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Jose;

namespace FirebaseNetAdmin.Encryption.JWT.Providers
{
    public class JoseJwtProvider : IJWTProvider
    {
        public string Encode(IDictionary<string, string> payload, RSA privateKey)
        {
            if (payload == null || payload.Count == 0)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            if (privateKey == null)
            {
                throw new ArgumentNullException(nameof(privateKey));
            }

            return Jose.JWT.Encode(payload, privateKey, JwsAlgorithm.RS256);
        }
    }
}
