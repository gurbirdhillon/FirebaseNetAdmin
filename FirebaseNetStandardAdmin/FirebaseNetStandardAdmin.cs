using System;
using FirebaseNetStandardAdmin.Configurations.ServiceAccounts;
using FirebaseNetStandardAdmin.Firebase.Auth;
using FirebaseNetStandardAdmin.Firebase.Database;
using FirebaseNetStandardAdmin.Firebase.Storage;

namespace FirebaseNetStandardAdmin
{
    public class FirebaseNetStandardAdmin : IFirebaseNetStandardAdmin, IDisposable
    {
        #region Fields

        private IServiceAccountCredentials _credentials;

        private GoogleServiceAccess _requestedAccess;

        #endregion

        #region Properties

        public IFirebaseAdminAuth Auth { get; private set; }

        public IFirebaseAdminDatabase Database { get; private set; }

        public IGoogleStorage Storage { get; private set; }

        #endregion

        #region Constructors

        public FirebaseNetStandardAdmin(IServiceAccountCredentials credentials) : this(credentials, GoogleServiceAccess.Full)
        {
        }

        public FirebaseNetStandardAdmin(IServiceAccountCredentials credentials, GoogleServiceAccess access)
        {
            Initialize(credentials, access);
        }

        #endregion

        #region IDisposable Methods

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Auth.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~FirebaseNetStandardAdmin() => Dispose(false);

        #endregion

        #region Private Helpers

        private void Initialize(IServiceAccountCredentials credentials, GoogleServiceAccess access)
        {
            _requestedAccess = access;
            _credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
            Auth = new FirebaseAdminAuth();

            if (GoogleServiceAccess.DatabaseOnly == (_requestedAccess & GoogleServiceAccess.DatabaseOnly))
                Database = new FirebaseAdminDatabase(Auth, _credentials);

            if (GoogleServiceAccess.StorageOnly == (_requestedAccess & GoogleServiceAccess.StorageOnly))
                Storage = new GoogleCloudStorage(Auth, _credentials);
        }

        #endregion
    }

    [Flags]
    public enum GoogleServiceAccess
    {
        DatabaseOnly = 0b0001,
        StorageOnly = 0b0010,
        Full = 0b0011
    }
}
