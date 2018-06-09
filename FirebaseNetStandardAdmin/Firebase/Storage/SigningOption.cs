﻿using System;

namespace FirebaseNetStandardAdmin.Firebase.Storage
{
    public class SigningOption
    {
        public SigningAction Action { get; set; }
        public DateTime ExpireDate { get; set; }
        public string Path { get; set; }
        public string ContentType { get; set; }
        public string ContentMD5 { get; set; }
    }
}
