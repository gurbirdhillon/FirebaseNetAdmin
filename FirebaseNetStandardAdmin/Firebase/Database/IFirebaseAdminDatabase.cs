namespace FirebaseNetStandardAdmin.Firebase.Database
{
    public interface IFirebaseAdminDatabase
    {
        IFirebaseAdminRef Ref(string path);
    }
}
