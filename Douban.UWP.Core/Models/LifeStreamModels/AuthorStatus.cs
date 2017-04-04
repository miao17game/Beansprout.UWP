using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.LifeStreamModels {
    [DataContract]
    public class AuthorStatus {

        [IgnoreDataMember]
        public string LocationID { get; set; }

        [IgnoreDataMember]
        public string LocationName { get; set; }

        [IgnoreDataMember]
        public string LocationUid { get; set; }

        [DataMember(Name = "loc")]
        public PositionStatus Position { get; set; }

        [DataMember(Name = "kind")]
        public string Kind { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "uri")]
        public string Uri { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "gender")]
        public string Gender { get; set; }

        [DataMember(Name = "abstract")]
        public string Abstract { get; set; }

        [DataMember(Name = "avatar")]
        public string Avatar { get; set; }

        [DataMember(Name = "large_avatar")]
        public string LargeAvatar { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "id")]
        public string ID { get; set; }

        [DataMember(Name = "uid")]
        public string Uid { get; set; }
    }
}
