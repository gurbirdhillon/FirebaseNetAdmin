using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FirebaseNetAdmin.Configurations;
using FirebaseNetAdmin.Configurations.AuthPayload;
using FirebaseNetAdmin.Configurations.ServiceAccounts;
using FirebaseNetAdmin.Exceptions;
using FirebaseNetAdmin.Extensions;
using FirebaseNetAdmin.Firebase;
using FirebaseNetAdmin.Serializations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FirebaseNetAdmin.HttpClients
{
    /// <summary>
    /// Creates Rest Api client for firebase rest http interactions
    /// </summary>
    public class FirebaseHttpClient : HttpClient, IFirebaseHttpClient
    {
        #region Fields

        private IFirebaseConfiguration _firebaseConfig;
        private IServiceAccountCredentials _serviceAccountCredentials;
        private JwtAuthPayloadGenerator _jwtPayload;
        private CustomTokenPayloadGenerator _jwtCustomTokenPayload;

        private FirebaseAccessToken _accessToken;
        private static readonly HttpMethod httpPatchMethod = new HttpMethod("PATCH");
        private Uri _authority;

        #endregion Fields

        #region Properties
        #endregion Properties

        #region Constructors

        public FirebaseHttpClient(IServiceAccountCredentials credentials) => Initialize(credentials, new DefaultFirebaseConfiguration(), null);

        public FirebaseHttpClient(IServiceAccountCredentials credentials, Uri authority) => Initialize(credentials, new DefaultFirebaseConfiguration(), authority);

        public FirebaseHttpClient(IServiceAccountCredentials credentials, IFirebaseConfiguration config, Uri authority) => Initialize(credentials, config, authority);

        public FirebaseHttpClient(IServiceAccountCredentials credentials, IFirebaseConfiguration config, HttpMessageHandler handler, Uri authority) : base(handler) => Initialize(credentials, config, authority);

        public FirebaseHttpClient(IServiceAccountCredentials credentials, IFirebaseConfiguration config, HttpMessageHandler handler, bool disposeHandler, Uri authority) : base(handler, disposeHandler) => Initialize(credentials, config, authority);

        #endregion

        #region Public

        public FirebaseAccessToken Send2LOTokenRequest() => Send2LOTokenRequestAsync().Result;

        public async Task<T> GetFromPathAsync<T>(string path) => await GetFromPathAsync<T>(new Uri(path, UriKind.Relative));

        public async Task<T> GetFromPathAsync<T>(Uri path)
        {
            var dataAsString = await SendAsyncCore(() => PrepareGetRequest(path));
            var serializationOptions = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            return JsonConvert.DeserializeObject<T>(dataAsString, serializationOptions);
        }

        public async Task<List<T>> GetArrayFromPathAsync<T>(string path) => await GetArrayFromPathAsync<T>(new Uri(path, UriKind.Relative));

        public async Task<List<T>> GetArrayFromPathAsync<T>(Uri path)
        {
            var dataAsString = await SendAsyncCore(() => PrepareGetRequest(path));
            var serializationOptions = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            return JsonConvert.DeserializeObject<List<T>>(dataAsString, serializationOptions);
        }

        public async Task<IList<T>> GetFromPathAsyncWithKeyInjected<T>(string path) where T : KeyEntity => await GetFromPathAsyncWithKeyInjected<T>(new Uri(path, UriKind.Relative));

        public async Task<IList<T>> GetFromPathAsyncWithKeyInjected<T>(Uri path) where T : KeyEntity
        {
            var dataAsString = await SendAsyncCore(() => PrepareGetRequest(path));
            var serializationOptions = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var result = JsonConvert.DeserializeObject<Dictionary<string, T>>(dataAsString, serializationOptions);

            return result.Select(s =>
            {
                if (s.Value != null)
                    s.Value.Key = s.Key;
                return s.Value;
            }).ToList();
        }

        public async Task<List<T>> GetArrayFromPathAsyncWithKeyInjected<T>(string path) where T : KeyEntity => await GetArrayFromPathAsyncWithKeyInjected<T>(new Uri(path, UriKind.Relative));

        public async Task<List<T>> GetArrayFromPathAsyncWithKeyInjected<T>(Uri path) where T : KeyEntity
        {
            var dataAsString = await SendAsyncCore(() => PrepareGetRequest(path));
            var serializationOptions = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var result = JsonConvert.DeserializeObject<List<T>>(dataAsString, serializationOptions);
            var index = 0;

            result.ForEach(item =>
            {
                if (item != null)
                    item.Key = index++.ToString();
            });

            return result;
        }

        public async Task<T> SetToPathAsync<T>(string path, T content) => await SetToPathAsync(new Uri(path, UriKind.Relative), content);

        public async Task<T> SetToPathAsync<T>(Uri path, T content)
        {
            var dataAsString = await SendAsyncCore(() => PrepareSetRequest(path, content));
            var serializationOptions = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            return JsonConvert.DeserializeObject<T>(dataAsString, serializationOptions);
        }

        public async Task<string> PushToPathAsync<T>(Uri path, T content)
        {
            var dataAsString = await SendAsyncCore(() => PreparePushRequest(path, content));
            var firebaseKey = new { name = "" };
            var result = JsonConvert.DeserializeAnonymousType(dataAsString, firebaseKey);
            return result.name;
        }

        public async Task<object> UpdatePathAsync(string path, Dictionary<string, object> content)
        {
            var dataAsString = await SendAsyncCore(() => PreparePatchRequest(new Uri(path, UriKind.Relative), content));
            return JsonConvert.DeserializeObject<object>(dataAsString);
        }

        public async Task<object> UpdatePathAsync(Uri path, Dictionary<string, object> content)
        {
            var dataAsString = await SendAsyncCore(() => PreparePatchRequest(path, content));
            return JsonConvert.DeserializeObject<object>(dataAsString);
        }

        public async Task<string> PushToPathAsync<T>(string path, T content) => await PushToPathAsync(new Uri(path, UriKind.Relative), content);

        public async Task DeleteFromPathAsync(Uri path) => await SendAsyncCore(() => PrepareDeleteRequest(path));

        public async Task DeleteFromPathAsync(string path) => await DeleteFromPathAsync(new Uri(path, UriKind.Relative));

        public async Task<FirebaseAccessToken> Send2LOTokenRequestAsync()
        {
            var jwtPayload = _jwtPayload.GetPayload();
            var rsaParams = _serviceAccountCredentials.GetRSAParams();

            string jwtToken;
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(rsaParams);
                jwtToken = _firebaseConfig.JwtTokenProvider.Encode(jwtPayload, rsa);
            }

            var permissionPayload = new PermissionAuthPayloadGenerator(jwtToken).GetPayload();

            var urlEncodedPayload = new FormUrlEncodedContent(permissionPayload);
            
            var response = await PostAsync(_firebaseConfig.GoogleOAuthTokenPath, urlEncodedPayload);
            await response.EnsureSuccessStatusCodeAsync();

            if (response.Content == null)
            {
                throw new FirebaseHttpException("Authentication failed, empty response content from firebase server");
            }
            var strinRepresentation = await response.Content.ReadAsStringAsync();
            var serializationSettings = new JsonSerializerSettings { ContractResolver = new FirebaseAccessTokenContractResolver() };
            _accessToken = JsonConvert.DeserializeObject<FirebaseAccessToken>(strinRepresentation, serializationSettings);
            if (_accessToken == null)
            {
                throw new FirebaseHttpException("Authentication failed, unsupported content type was returned from firebase server");
            }
            return _accessToken;

        }

        public Uri GetAuthority() => _authority;

        public string CreateCustomToken(long userId)
        {
            var jwtPayload = _jwtCustomTokenPayload.GetPayload(new Dictionary<string, string>
            {
                { "uid", userId.ToString() }
            });
            var rsaParams = _serviceAccountCredentials.GetRSAParams();

            string jwtToken;
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(rsaParams);
                jwtToken = _firebaseConfig.JwtTokenProvider.Encode(jwtPayload, rsa);
            }

            return jwtToken;
        }

        #endregion

        #region Private Helpers

        private HttpRequestMessage PrepareSetRequest<T>(Uri path, T content) => PrepareContentRequest(path, content, HttpMethod.Put);

        private HttpRequestMessage PreparePushRequest<T>(Uri path, T content) => PrepareContentRequest(path, content, HttpMethod.Post);

        private HttpRequestMessage PreparePatchRequest(Uri path, Dictionary<string, object> content) => PrepareContentRequest(path, content, httpPatchMethod);

        private HttpRequestMessage PrepareContentRequest<T>(Uri path, T content, HttpMethod method)
        {
            var serializationOptions = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var stringContent = JsonConvert.SerializeObject(content, serializationOptions);
            var jsonContent = new StringContent(stringContent, Encoding.UTF8, "application/json");
            var fullUri = GetFullAbsoluteUrl(path);

            var message = new HttpRequestMessage
            {
                RequestUri = fullUri,
                Method = method,
                Content = jsonContent
            };
            AddAuthHeaders(message);
            return message;
        }

        public async Task<T> SendStorageRequestAsync<T>(Uri path, HttpMethod method)
        {
            var dataAsString = await SendAsyncCore(() => PrepareStorageRequest(path, method));
            var serializationOptions = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            return JsonConvert.DeserializeObject<T>(dataAsString, serializationOptions);
        }

        private HttpRequestMessage PrepareStorageRequest(Uri path, HttpMethod method)
        {
            var fullUri = GetFullAbsoluteUrl(path);
            var message = new HttpRequestMessage
            {
                RequestUri = fullUri,
                Method = method
            };
            AddAuthHeaders(message);
            return message;
        }

        private async Task<string> SendAsyncCore(Func<HttpRequestMessage> requestMessage)
        {
            var response = await SendAsyncWithReAuthentication(requestMessage);
            await response.EnsureSuccessStatusCodeAsync();

            if (response.Content == null)
                return null;

            var dataAsString = await response.Content.ReadAsStringAsync();

            return dataAsString;
        }

        private async Task<HttpResponseMessage> SendAsyncWithReAuthentication(Func<HttpRequestMessage> requestMessage)
        {
            var response = await SendAsync(requestMessage());
            if (response.StatusCode != HttpStatusCode.Unauthorized) return response;
            await Send2LOTokenRequestAsync();
            response = await SendAsync(requestMessage());
            return response;
        }
        
        private HttpRequestMessage PrepareGetRequest(Uri path)
        {
            var fullUri = GetFullAbsoluteUrl(path);
            var message = new HttpRequestMessage
            {
                RequestUri = fullUri,
                Method = HttpMethod.Get
            };
            AddAuthHeaders(message);
            return message;
        }

        private HttpRequestMessage PrepareDeleteRequest(Uri path)
        {
            var fullUri = GetFullAbsoluteUrl(path);
            var message = new HttpRequestMessage
            {
                RequestUri = fullUri,
                Method = HttpMethod.Delete
            };
            AddAuthHeaders(message);
            return message;
        }

        protected void AddAuthHeaders(HttpRequestMessage request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken?.AccessToken);
        }

        protected void Initialize(IServiceAccountCredentials credentials, IFirebaseConfiguration config, Uri authority)
        {
            if (authority != null && !authority.IsAbsoluteUri)
            {
                throw new ArgumentOutOfRangeException(nameof(authority), "Authority should be absalute uri");
            }

            _authority = authority;
            _serviceAccountCredentials = credentials;
            _firebaseConfig = config;
            _jwtPayload = new JwtAuthPayloadGenerator(_serviceAccountCredentials, _firebaseConfig);
            _jwtCustomTokenPayload = new CustomTokenPayloadGenerator(_serviceAccountCredentials, _firebaseConfig);

            DefaultRequestHeaders.Add("accept", "application/json");
        }

        private Uri GetFullAbsoluteUrl(Uri uri)
        {
            if (_authority == null && !uri.IsAbsoluteUri)
            {
                throw new ArgumentOutOfRangeException(nameof(uri), "If Authority is missing uri should be absalute");
            }

            if (uri.IsAbsoluteUri)
            {
                return uri;
            }

            return _authority == null ? uri : new Uri(_authority, uri);
        }

        #endregion
    }
}
