using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.LifeStreamModels {

    [DataContract]
    public class SimpleUserBase {

        [DataMember(Name = "loc")]
        public SimpleUserLocationBase Location { get; set; }

        [DataMember(Name = "kind")]
        public string Kind { get; set; }

        [DataMember(Name = "followed")]
        public bool Followed { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "in_blacklist")]
        public bool InBlackList { get; set; }

        [DataMember(Name = "url")]
        public string URL { get; set; }

        [DataMember(Name = "gender")]
        public string Gender { get; set; }

        [DataMember(Name = "remark")]
        public string Remark { get; set; }

        [DataMember(Name = "uri")]
        public string Uri { get; set; }

        [DataMember(Name = "avatar")]
        public string Avatar { get; set; }

        [DataMember(Name = "large_avatar")]
        public string LargeAvatar { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "id")]
        public string ID { get; set; }

        [DataMember(Name = "uid")]
        public string UID { get; set; }

    }

    [DataContract]
    public class SimpleUserLocationBase {

        [DataMember(Name = "id")]
        public string ID { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "uid")]
        public string UID { get; set; }

    }

}
