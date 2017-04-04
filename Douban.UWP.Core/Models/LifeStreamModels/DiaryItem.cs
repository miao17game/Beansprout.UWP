using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.LifeStreamModels {
    [DataContract]
    public class DiaryItem : InfosItemBase {

        [DataMember(Name = "id")]
        public string ID { get; set; }

        [DataMember(Name = "domain")]
        public string Domain { get; set; }

        [DataMember(Name = "sharing_url")]
        public string SharingUrl { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "abstract")]
        public string Abstract { get; set; }

        [IgnoreDataMember]
        public bool HasCover { get; set; }

        [DataMember(Name = "cover")]
        public string Cover { get; set; }

        [DataMember(Name = "timeline_share_count")]
        public int TimeLineShareCount { get; set; }

        [DataMember(Name = "update_time")]
        public string UpdateTime { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "allow_comment")]
        public bool AllowComment { get; set; }

        [DataMember(Name = "author")]
        public AuthorStatus Author { get; set; }

    }
}
