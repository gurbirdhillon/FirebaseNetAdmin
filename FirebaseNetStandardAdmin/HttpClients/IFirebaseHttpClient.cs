﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FirebaseNetStandardAdmin.Firebase;

namespace FirebaseNetStandardAdmin.HttpClients
{
    public interface IFirebaseHttpClient : IDisposable
    {
        Uri GetAuthority();
        FirebaseAccessToken Send2LOTokenRequest();
        string CreateCustomToken(long userId);

        Task<FirebaseAccessToken> Send2LOTokenRequestAsync();

        Task<T> GetFromPathAsync<T>(string path);
        Task<T> GetFromPathAsync<T>(Uri path);

        Task<List<T>> GetArrayFromPathAsync<T>(string path);
        Task<List<T>> GetArrayFromPathAsync<T>(Uri path);

        Task<IList<T>> GetFromPathAsyncWithKeyInjected<T>(Uri path) where T : KeyEntity;
        Task<IList<T>> GetFromPathAsyncWithKeyInjected<T>(string path) where T : KeyEntity;

        Task<List<T>> GetArrayFromPathAsyncWithKeyInjected<T>(Uri path) where T : KeyEntity;
        Task<List<T>> GetArrayFromPathAsyncWithKeyInjected<T>(string path) where T : KeyEntity;

        Task<T> SetToPathAsync<T>(string path, T content);
        Task<T> SetToPathAsync<T>(Uri path, T content);

        Task<string> PushToPathAsync<T>(Uri path, T content);
        Task<string> PushToPathAsync<T>(string path, T content);

        Task<object> UpdatePathAsync(string path, Dictionary<string, object> content);
        Task<object> UpdatePathAsync(Uri path, Dictionary<string, object> content);

        Task DeleteFromPathAsync(string path);
        Task DeleteFromPathAsync(Uri path);

        Task<T> SendStorageRequestAsync<T>(Uri path, HttpMethod method);
    }
}
