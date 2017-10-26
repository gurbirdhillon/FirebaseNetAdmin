using System;
using FirebaseNetAdmin.Firebase.Database;

namespace FirebaseNetAdmin.Firebase.Commands
{
    public static partial class CommandExtensions
    {
        public static IFirebaseAdminRef OrderBy(this IFirebaseAdminRef firebaseRef, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }
            firebaseRef.Add("orderBy", $"\"{value}\"");
            return firebaseRef;
        }
    }
}
