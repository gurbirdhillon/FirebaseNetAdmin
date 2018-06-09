using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FirebaseNetStandardAdmin.Configurations;
using FirebaseNetStandardAdmin.Configurations.ServiceAccounts;
using FirebaseNetStandardAdmin.Extensions;
using FirebaseNetStandardAdmin.Firebase.Auth;
using FirebaseNetStandardAdmin.HttpClients;

namespace FirebaseNetStandardAdmin.Firebase.Storage
{
    public class GoogleCloudStorage : IGoogleStorage, IDisposable
    {
        private readonly IFirebaseHttpClient _httpClient;
        private readonly IServiceAccountCredentials _credentials;
        private readonly IFirebaseConfiguration _firebaseConfiguration;

        public GoogleCloudStorage(IFirebaseAdminAuth auth, IServiceAccountCredentials credentials)
        {
            var firebaseConfiguration = new DefaultFirebaseConfiguration(GoogleServiceAccess.StorageOnly);
            var storageAuthority = new Uri($"{firebaseConfiguration.StorageBaseAuthority.TrimSlashes()}", UriKind.Absolute);

            _httpClient = new FirebaseHttpClient(credentials, firebaseConfiguration, storageAuthority);
            _credentials = credentials;
            _firebaseConfiguration = firebaseConfiguration;

            auth.AddFirebaseHttpClient(_httpClient);
        }

        public string GetPublicUrl(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            var normalziedPath = WebUtility.UrlEncode(path.TrimSlashes());

            var auth = _httpClient.GetAuthority().ToString().TrimSlashes();
            var fullPath = new Uri($"{auth}/{_credentials.GetDefaultBucket()}/{normalziedPath}?alt=media", UriKind.Absolute);
            return fullPath.AbsoluteUri;
        }

        public string GetSignedUrl(SigningOption options)
        {
            ValidateOptions(options);

            var signingPayloadAsString = string.Join("\n", BuildSigningPayload(options));
            var encryptedBase64String = EncryptPayload(signingPayloadAsString);

            return PrepareSignedUrl(options, encryptedBase64String);
        }

        public async Task RemoveObjectAsync(string path)
        {
            var urlEncodedPath = GetUrlEncodedPath(path);
            var deleteUri = new Uri($"{ _firebaseConfiguration.StorageBaseAuthority2}/v1/b/{_credentials.GetDefaultBucket()}/o/{urlEncodedPath}", UriKind.Absolute);
            await _httpClient.SendStorageRequestAsync<object>(deleteUri, HttpMethod.Delete);
        }

        public async Task<Tuple<bool /* Result */, Exception /* ex */>> TryRemoveObjectAsync(string path)
        {
            try
            {
                await RemoveObjectAsync(path);
                return Tuple.Create(true, (Exception) null);
            }
            catch (Exception ex)
            {
                return Tuple.Create(false, ex);
            }
        }


        public async Task MoveObjectAsync(string originPath, string destinationPath)
        {
            var urlEncodedOrigin = GetUrlEncodedPath(originPath);
            var urlEncodedDestination = GetUrlEncodedPath(destinationPath);

            var reWriteUri = new Uri($"{ _firebaseConfiguration.StorageBaseAuthority2}/v1/b/{_credentials.GetDefaultBucket()}/o/{urlEncodedOrigin}/rewriteTo/b/{_credentials.GetDefaultBucket()}/o/{urlEncodedDestination}", UriKind.Absolute);
            await _httpClient.SendStorageRequestAsync<object>(reWriteUri, HttpMethod.Post);

            await RemoveObjectAsync(originPath);
        }

        public async Task<Tuple<bool /* Result */, Exception /* ex */>> TryMoveObjectAsync(string originPat, string destinationPath)
        {
            try
            {
                await MoveObjectAsync(originPat, destinationPath);
                return Tuple.Create(true, (Exception) null);
            }
            catch (Exception ex)
            {
                return Tuple.Create(false, ex);
            }
        }

        public async Task<ObjectMetadata> GetObjectMetaDataAsync(string path)
        {
            var urlEncodedPath = GetUrlEncodedPath(path);
            var metaUri = new Uri($"{ _firebaseConfiguration.StorageBaseAuthority2}/v1/b/{_credentials.GetDefaultBucket()}/o/{urlEncodedPath}", UriKind.Absolute);
            return await _httpClient.SendStorageRequestAsync<ObjectMetadata>(metaUri, HttpMethod.Get);
        }

        private string GetUrlEncodedPath(string path)
        {
            var normalizedPath = path.TrimSlashes();

            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            return WebUtility.UrlEncode(normalizedPath);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _httpClient.Dispose();
            }
        }

        ~GoogleCloudStorage() => Dispose(false);

        private string PrepareSignedUrl(SigningOption options, string signature) => new UriBuilder
        {
            Scheme = "https",
            Host = $"{_firebaseConfiguration.StorageBaseAuthority.TrimSlashes().Replace("https://", "")}/{_credentials.GetDefaultBucket().TrimSlashes()}",
            Path = options.Path.Trim().TrimSlashes(),
            Query = $"GoogleAccessId={_credentials.GetServiceAccountEmail()}&Expires={BuildExpiration(options.ExpireDate)}&Signature={WebUtility.UrlEncode(signature)}"
        }.Uri.AbsoluteUri;

        private string EncryptPayload(string signingPayloadAsString)
        {
            string encryptedBase64String;
            var byteConverter = new UTF8Encoding();
            var payloadBytes = byteConverter.GetBytes(signingPayloadAsString);

            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(_credentials.GetRSAParams());
                var encrypt = rsa.SignData(payloadBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                encryptedBase64String = Convert.ToBase64String(encrypt);
            }
            return encryptedBase64String;
        }

        private IEnumerable<string> BuildSigningPayload(SigningOption options) => new List<string>
        {
            BuildActionMethod(options.Action),
            BuildContentMD5(options.ContentMD5),
            BuildContentType(options.ContentType),
            BuildExpiration(options.ExpireDate),
            BuilCanonicalizedResource(options.Path)
        };

        private static string BuildActionMethod(SigningAction action)
        {
            switch (action)
            {
                case SigningAction.Read:
                    return "GET";
                case SigningAction.Write:
                    return "PUT";
                case SigningAction.Delete:
                    return "DELETE";
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }

        private static string BuildContentMD5(string contentMD5) => string.IsNullOrWhiteSpace(contentMD5) ? string.Empty : contentMD5.Trim();

        private static string BuildContentType(string contentType) => string.IsNullOrWhiteSpace(contentType) ? string.Empty : contentType.Trim();

        private static string BuildExpiration(DateTime dateTo) => dateTo.ToUnixSeconds().ToString();

        private string BuilCanonicalizedResource(string path) => $"/{_credentials.GetDefaultBucket().Trim().TrimSlashes()}/{WebUtility.UrlEncode(path.TrimSlashes())}";

        private void ValidateOptions(SigningOption options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (options.ExpireDate.ToUnixSeconds() == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(options.ExpireDate), "ExpireDate should be reasonable value");
            }
            if (string.IsNullOrWhiteSpace(options.Path))
            {
                throw new ArgumentNullException(nameof(options.Path));
            }
        }
    }
}
