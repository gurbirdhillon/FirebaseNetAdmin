using Newtonsoft.Json;

namespace FirebaseNetAdmin.Firebase
{
    public class KeyEntity
    {
        [JsonIgnore]
        public virtual string Key { get; set; }
    }
}
