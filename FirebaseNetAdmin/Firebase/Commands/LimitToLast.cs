
namespace FirebaseNetAdmin.Firebase.Commands
{
    using FirebaseNetAdmin.Firebase.Database;

    public static partial class CommandExtensions
    {
        public static IFirebaseAdminRef LimitToLast(this IFirebaseAdminRef firebaseRef, long value)
        {
            firebaseRef.Add("limitToLast", value.ToString());
            return firebaseRef;
        }
    }
}
