namespace FirebaseNetAdmin.Firebase.Commands
{
    using FirebaseNetAdmin.Firebase.Database;
    using System.Threading.Tasks;

    public static partial class CommandExtensions
    {
        public static async Task DeleteAsync(this IFirebaseAdminRef firebaseRef)
        {
            await firebaseRef.HttpClient.DeleteFromPathAsync(firebaseRef.Path);
        }

        public static void Delete(this IFirebaseAdminRef firebaseRef)
        {
            DeleteAsync(firebaseRef).Wait();
        }
    }
}
