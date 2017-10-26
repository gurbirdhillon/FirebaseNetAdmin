using System.Security.Cryptography;

namespace FirebaseNetAdmin.Configurations.ServiceAccounts
{
    public interface IServiceAccountCredentials
    {
        RSAParameters GetRSAParams();
        string GetServiceAccountEmail();
        string GetProjectId();
        string GetDefaultBucket();
    }
}
