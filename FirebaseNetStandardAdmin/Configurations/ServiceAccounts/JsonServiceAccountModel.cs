﻿using Newtonsoft.Json;

namespace FirebaseNetStandardAdmin.Configurations.ServiceAccounts
{
    public class JsonServiceAccountModel
    {
        [JsonProperty("project_id")]
        public string ProjectId { get; set; }

        [JsonProperty("private_key_id")]
        public string PrivateKeyId { get; set; }

        [JsonProperty("private_key")]
        public string PrivateKey { get; set; }

        [JsonProperty("client_email")]
        public string ClientEmail { get; set; }

        [JsonProperty("client_id")]
        public string ClientId { get; set; }
    }
}
