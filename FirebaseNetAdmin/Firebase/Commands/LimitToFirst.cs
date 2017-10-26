using FirebaseNetAdmin.Firebase.Database;

namespace FirebaseNetAdmin.Firebase.Commands
{
    public static partial class CommandExtensions
    {
        public static IFirebaseAdminRef LimitToFirst(this IFirebaseAdminRef firebaseRef, long value)
        {
            firebaseRef.Add("limitToFirst", value.ToString());
            return firebaseRef;
        }
    }
}
