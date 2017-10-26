using FirebaseNetAdmin.Firebase.Database;

namespace FirebaseNetAdmin.Firebase.Commands
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
