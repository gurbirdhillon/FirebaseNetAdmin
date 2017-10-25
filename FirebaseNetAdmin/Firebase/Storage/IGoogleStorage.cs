using System;
using System.Threading.Tasks;

namespace FirebaseNetAdmin.Firebase.Storage
{
    public interface IGoogleStorage
    {
        string GetPublicUrl(string path);

        string GetSignedUrl(SigningOption options);

        Task RemoveObjectAsync(string path);

        Task<Tuple<bool /* Result */, Exception /* ex */>> TryRemoveObjectAsync(string path);

        Task<ObjectMetadata> GetObjectMetaDataAsync(string path);

        Task MoveObjectAsync(string originPat, string destinationPath);

        Task<Tuple<bool /* Result */, Exception /* ex */>> TryMoveObjectAsync(string originPat, string destinationPath);
    }
}
