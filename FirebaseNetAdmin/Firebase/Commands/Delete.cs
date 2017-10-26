using System.Threading.Tasks;
using FirebaseNetAdmin.Firebase.Database;

namespace FirebaseNetAdmin.Firebase.Commands
{
    public static partial class CommandExtensions
    {
        public static async Task DeleteAsync(this IFirebaseAdminRef firebaseRef) => await firebaseRef.HttpClient.DeleteFromPathAsync(firebaseRef.Path);

        public static void Delete(this IFirebaseAdminRef firebaseRef) => DeleteAsync(firebaseRef).Wait();
    }
}
