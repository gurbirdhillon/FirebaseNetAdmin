using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Parameters;

namespace FirebaseNetAdmin.Extensions
{
    public static class EncryptionExtensions
    {
        public static RSAParameters ToRSAParameters(this RsaPrivateCrtKeyParameters privKey) => new RSAParameters
        {
            Modulus = privKey.Modulus.ToByteArrayUnsigned(),
            Exponent = privKey.PublicExponent.ToByteArrayUnsigned(),
            D = privKey.Exponent.ToByteArrayUnsigned(),
            P = privKey.P.ToByteArrayUnsigned(),
            Q = privKey.Q.ToByteArrayUnsigned(),
            DP = privKey.DP.ToByteArrayUnsigned(),
            DQ = privKey.DQ.ToByteArrayUnsigned(),
            InverseQ = privKey.QInv.ToByteArrayUnsigned()
        };
    }
}
