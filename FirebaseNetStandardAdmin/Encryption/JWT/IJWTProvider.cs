using System.Collections.Generic;
using System.Security.Cryptography;

namespace FirebaseNetStandardAdmin.Encryption.JWT
{
    public interface IJWTProvider
    {
        string Encode(IDictionary<string, string> payload, RSA privateKey);
    }
}
