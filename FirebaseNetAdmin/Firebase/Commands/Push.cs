using System;
using System.Threading.Tasks;
using FirebaseNetAdmin.Firebase.Database;

namespace FirebaseNetAdmin.Firebase.Commands
{
    public static partial class CommandExtensions
    {
        public static async Task<string> PushAsync<T>(this IFirebaseAdminRef firebaseRef, T content)
        {
            if(content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }
            return await firebaseRef.HttpClient.PushToPathAsync(firebaseRef.Path, content);
        }

        public static string Push<T>(this IFirebaseAdminRef firebaseRef, T content) => PushAsync(firebaseRef, content).Result;
    }
}
