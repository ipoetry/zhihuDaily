using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace zhihuDaily.Model
{
    [DataContract]
    public class UserInfo: NotificationObject
    {
        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; } = "M3X1ANI4QKKDdNLjVR_d8w";

        [DataMember(Name = "bound_services")]
        public string[] BoundServices { get; set; }
        
        private string _name;
        [DataMember(Name = "name")]
        public string Name { get { return _name; } set { _name = value; RaisePropertyChanged(() => Name); } }
        [DataMember(Name = "avatar")]
        private string _avatar;
        public string Avatar { get { return _avatar; } set { _avatar = value; RaisePropertyChanged(() => Avatar); } }
    }
}
