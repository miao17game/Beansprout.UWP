using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.LifeStreamModels {
    [DataContract]
    public class InfosItemBase {

        [IgnoreDataMember]
        public JsonType Type { get; set; }

        [DataMember(Name = "create_time")]
        public string Time { get; set; }

        [DataMember(Name = "likers_count")]
        public string LikersCounts { get; set; }

        [DataMember(Name = "uri")]
        public string Uri { get; set; }

        [DataMember(Name = "comments_count")]
        public string CommentsCounts { get; set; }

        public enum JsonType { Undefined, Card, Album, Article, Status, Note }

    }
}
