using Newtonsoft.Json;

namespace FirebaseNetStandardAdmin.Firebase
{
    public class KeyEntity
    {
        [JsonIgnore]
        public virtual string Key { get; set; }
    }
}
