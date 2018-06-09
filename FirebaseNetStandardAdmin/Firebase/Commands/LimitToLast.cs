using FirebaseNetStandardAdmin.Firebase.Database;

namespace FirebaseNetStandardAdmin.Firebase.Commands
{
    public static partial class CommandExtensions
    {
        public static IFirebaseAdminRef LimitToLast(this IFirebaseAdminRef firebaseRef, long value)
        {
            firebaseRef.Add("limitToLast", value.ToString());
            return firebaseRef;
        }
    }
}
