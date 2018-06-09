## FirebaseNetAdmin [![NuGet](https://img.shields.io/nuget/v/FirebaseNetAdmin.svg)](https://www.nuget.org/packages/FirebaseNetStandardAdmin)

FirebaseNetAdmin is a .NET library for interacting with Firebase Database and Storage.
The interface of library is similar to offical Google Node, Java (Admin) SDKs.

## Initialization

Supports both JSON and P12 config files.
In order to give permissions to use your Firebase Database and Storage you need first in your Firebase app create [service account](https://firebase.google.com/docs/admin/setup) with corresponding permissions. After creating the service account you will be prompted to download either JSON file or P12 file, recommended is JSON file. Download that file and attach to your project.

* JSON file
``` C#
var credentials = new JSONServiceAccountCredentials("your-file.json");
var firebaseClient = new FirebaseAdmin(credentials);
```
* P12 file
``` C#
var credentials = new P12ServiceAccountCredentials("your-file.p12", "your-secret", "your-service-account", "your-database");
var firebaseClient = new FirebaseAdmin(credentials);
```

## Auth
Create token for some `userId` which should be used by client to authenticate against Firebase Database, that token could be used in client sdks by calling `firebase.auth().signInWithCustomToken(token)`

```C#
 var token = firebaseClient.Auth.CreateCustomToken(userId);
```

## Database
Getting reference on some node of Database use `firebaseClient.Database.Ref("endpoint")` for example `firebaseClient.Database.Ref("users/12/details")`

### Query database
Following reference query methods are available
* StartAt
* EndAt
* EqualTo
* LimitToFirst
* LimitToLast
* OrderBy

Note: when using filters you should have index on that field, otherwise exception will throw saying specify index on the field.

For getting data
* Get
* GetArray
* GetWithKeyInjected
* GetArrayWithKeyInjected


with their corresponding async methods

Examples:
Let's say you have this structure in Firebase:

`-users/{userId}/events`

                      --EventKey1
                      ---- CodeId: 1
                      ---- IsRead: true,
                      ---- Timestamp: 1502047422150
                      --EventKey2
                      ---- CodeId: 2
                      ---- IsRead: false,
                      ---- Timestamp: 1502047422279

Let's assume we have UserHistory class
```C#
class UserHistory {
            public int CodeId { get; set; }
            public bool IsRead { get; set; }
            public long Timestamp { get; set; }
}
```

and can query via

```C#

var result = firebaseClient.Database.Ref("users/330/events")
                .OrderBy("isRead").LimitToLast(1)
                .Get<UserHistory>();

```
We can inject key into model by inheriting `UserHistory: KeyEntity`
and instead of calling `.Get<UserHistory>()` call `.GetWithKeyInjected<UserHistory>()`
like:

```C#
var result = firebaseClient.Database.Ref("users/330/events")
                .OrderBy("isRead").LimitToLast(1)
                .GetWithKeyInjected<UserHistory>();
```


### Update database

* Push
* Set
* Update
* Delete

With corresponding async methods.
Methods are functioning exactly like their counterparts in NodeJS or Java SDKs.

Examples:

`Push`
```C#
var result = firebaseClient.Database.Ref("/users/30/details").Push(new Detail())

```

`Bulk update`
```C#
var result = firebaseClient.Database.Ref("/users/30/details").Update(new Dictionary<string, object>() {
                { "codeId", 20 } ,
                { "info","info"} ,
                { "sub/info","subinfo"} ,
             });
```

`Set`

```C#
var result = firebaseClient.Database.Ref("/test").Set(new Test1());
```

`Delete`

```C#
firebaseClient.Database.Ref("/test").Delete();
```

## Storage
Following storage methods are supported

* GetPublicUrl
* GetSignedUrl
* RemoveObjectAsync
* GetObjectMetaDataAsync
* MoveObjectAsync

Examples:

```C#
var result = await firebaseClient.Storage.GetObjectMetaDataAsync("test/my-image");

var publicUrl = firebaseClient.Storage.GetPublicUrl("my-image");

var signedUrl = firebaseClient.Storage.GetSignedUrl(new Firebase.Storage.SigningOption()
             {
                 Action = Firebase.Storage.SigningAction.Write,
                 Path = "my-image",
                 ContentType = "image/jpeg",
                 ExpireDate = DateTime.Now + new TimeSpan(0, 0, 0, 0, 60000000)
             });
```
