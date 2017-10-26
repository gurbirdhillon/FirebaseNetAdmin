using Newtonsoft.Json;

namespace FirebaseNetAdmin.Firebase
{
    public class KeyEntity
    {
        [JsonIgnore]
        public string Key { get; set; }
    }
}
