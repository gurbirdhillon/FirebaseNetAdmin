namespace FirebaseNetAdmin
{
    using FirebaseNetAdmin.Firebase.Auth;
    using FirebaseNetAdmin.Firebase.Database;
    using FirebaseNetAdmin.Firebase.Storage;

    public interface IFirebaseAdmin
    {
        IFirebaseAdminAuth Auth { get; }
        IFirebaseAdminDatabase Database { get; }
        IGoogleStorage Storage { get; }
    }
}
