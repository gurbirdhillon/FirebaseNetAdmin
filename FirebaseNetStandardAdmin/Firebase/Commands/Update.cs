using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FirebaseNetStandardAdmin.Firebase.Database;

namespace FirebaseNetStandardAdmin.Firebase.Commands
{
    public static partial class CommandExtensions
    {
        public static async Task<object> UpdateAsync(this IFirebaseAdminRef firebaseRef, Dictionary<string, object> content)
        {
            if (content == null || content.Count == 0)
            {
                throw new ArgumentNullException(nameof(content));
            }
            return await firebaseRef.HttpClient.UpdatePathAsync(firebaseRef.Path, content);
        }

        public static object Update(this IFirebaseAdminRef firebaseRef, Dictionary<string, object> content) => firebaseRef.HttpClient.UpdatePathAsync(firebaseRef.Path, content).Result;
    }
}
