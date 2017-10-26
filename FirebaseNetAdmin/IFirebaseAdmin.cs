using FirebaseNetAdmin.Firebase.Auth;
using FirebaseNetAdmin.Firebase.Database;
using FirebaseNetAdmin.Firebase.Storage;

namespace FirebaseNetAdmin
{
    public interface IFirebaseAdmin
    {
        IFirebaseAdminAuth Auth { get; }
        IFirebaseAdminDatabase Database { get; }
        IGoogleStorage Storage { get; }
    }
}
