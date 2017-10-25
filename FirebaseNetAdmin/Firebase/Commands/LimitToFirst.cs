
namespace FirebaseNetAdmin.Firebase.Commands
{
    using FirebaseNetAdmin.Firebase.Database;

    public static partial class CommandExtensions
    {
        public static IFirebaseAdminRef LimitToFirst(this IFirebaseAdminRef firebaseRef, long value)
        {
            firebaseRef.Add("limitToFirst", value.ToString());
            return firebaseRef;
        }
    }
}
