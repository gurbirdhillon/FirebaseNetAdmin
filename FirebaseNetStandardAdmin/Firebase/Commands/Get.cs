using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FirebaseNetStandardAdmin.Firebase.Database;

namespace FirebaseNetStandardAdmin.Firebase.Commands
{
    public static partial class CommandExtensions
    {
        public static T Get<T>(this IFirebaseAdminRef firebaseRef) =>
            GetAsync<T>(firebaseRef).Result;

        public static async Task<T> GetAsync<T>(this IFirebaseAdminRef firebaseRef) =>
            await firebaseRef.HttpClient.GetFromPathAsync<T>(PrepareUri(firebaseRef));

        public static List<T> GetArray<T>(this IFirebaseAdminRef firebaseRef) =>
            GetArrayAsync<T>(firebaseRef).Result;
        
        public static async Task<List<T>> GetArrayAsync<T>(this IFirebaseAdminRef firebaseRef) =>
            await firebaseRef.HttpClient.GetArrayFromPathAsync<T>(PrepareUri(firebaseRef));

        public static async Task<IList<T>> GetWithKeyInjectedAsync<T>(this IFirebaseAdminRef firebaseRef) where T : KeyEntity =>
            await firebaseRef.HttpClient.GetFromPathAsyncWithKeyInjected<T>(PrepareUri(firebaseRef));

        public static IList<T> GetWithKeyInjected<T>(this IFirebaseAdminRef firebaseRef) where T : KeyEntity =>
            GetWithKeyInjectedAsync<T>(firebaseRef).Result;

        public static async Task<List<T>> GetArrayWithKeyInjectedAsync<T>(this IFirebaseAdminRef firebaseRef) where T : KeyEntity =>
            await firebaseRef.HttpClient.GetArrayFromPathAsyncWithKeyInjected<T>(PrepareUri(firebaseRef));

        public static List<T> GetArrayWithKeyInjected<T>(this IFirebaseAdminRef firebaseRef) where T : KeyEntity =>
            GetArrayWithKeyInjectedAsync<T>(firebaseRef).Result;

        private static Uri PrepareUri(IFirebaseAdminRef firebaseRef)
        {
            var queryStore = firebaseRef.GetQueryStore();
            var queryParams = new StringBuilder("?");

            foreach (var param in queryStore)
            {
                queryParams.Append($"{param.Key}={param.Value}&");
            }

            return new Uri(firebaseRef.Path + queryParams, UriKind.Relative);
        }

    }
}
