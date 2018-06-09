using FirebaseNetStandardAdmin.Firebase.Auth;
using FirebaseNetStandardAdmin.Firebase.Database;
using FirebaseNetStandardAdmin.Firebase.Storage;

namespace FirebaseNetStandardAdmin
{
    public interface IFirebaseNetStandardAdmin
    {
        IFirebaseAdminAuth Auth { get; }
        IFirebaseAdminDatabase Database { get; }
        IGoogleStorage Storage { get; }
    }
}
