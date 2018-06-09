using System.Security.Cryptography;

namespace FirebaseNetStandardAdmin.Configurations.ServiceAccounts
{
    public interface IServiceAccountCredentials
    {
        RSAParameters GetRSAParams();
        string GetServiceAccountEmail();
        string GetProjectId();
        string GetDefaultBucket();
    }
}
