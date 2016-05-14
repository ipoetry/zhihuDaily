using System.Collections.Generic;
using System.Runtime.Serialization;

namespace zhihuDaily.Model
{
    [DataContract]
    public class Notification
    {
        [DataMember(Name = "user_count")]
        public int UserCount { get; set; }
        [DataMember(Name = "users")]
        public IEnumerable<User> Users { get; set; }
        [DataMember(Name = "object_id")]
        public long ObjectId { get; set; }
        [DataMember(Name = "time")]
        public long Time { get; set; }
        [DataMember(Name = "type")]
        public int Type { get; set; }
        [DataMember(Name = "id")]
        public long Id { get; set; }
        [DataMember(Name = "description")]
        public string Description { get; set; }
    }
    [DataContract]
    public class User
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "avatar")]
        public string Avatar { get; set; }
    }

}
