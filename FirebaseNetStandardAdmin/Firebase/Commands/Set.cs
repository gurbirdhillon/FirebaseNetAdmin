using System;
using System.Threading.Tasks;
using FirebaseNetStandardAdmin.Firebase.Database;

namespace FirebaseNetStandardAdmin.Firebase.Commands
{
    public static partial class CommandExtensions
    {
        public static async Task<T> SetAsync<T>(this IFirebaseAdminRef firebaseRef, T content)
        {
            if(content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }
            return await firebaseRef.HttpClient.SetToPathAsync(firebaseRef.Path, content);
        }

        public static T Set<T>(this IFirebaseAdminRef firebaseRef, T content) => SetAsync(firebaseRef, content).Result;
    }
}
