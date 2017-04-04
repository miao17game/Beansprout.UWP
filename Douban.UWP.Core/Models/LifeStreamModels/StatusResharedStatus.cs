using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.LifeStreamModels {
    [DataContract]
    public class StatusResharedStatus {

        [DataMember(Name = "reshares_count")]
        public string ResharesCount { get; set; }

        [DataMember(Name = "liked")]
        public bool IsLiked { get; set; }

        [DataMember(Name = "sharing_url")]
        public string SharingUrl { get; set; }

        [DataMember(Name = "author")]
        public AuthorStatus Author { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "like_count")]
        public string LikeCount { get; set; }

        [DataMember(Name = "uri")]
        public string Uri { get; set; }

        [DataMember(Name = "resharers_count")]
        public string ResharersCount  { get; set; }

        [DataMember(Name = "parent_status")]
        public object ParentStatus { get; set; }

        [IgnoreDataMember]
        public bool HasEntity { get; set; }

        [DataMember(Name = "entities")]
        public IList<StatusEntity> Entities { get; set; }

        [DataMember(Name = "subscription_text")]
        public string SubscriptionText { get; set; }

        [DataMember(Name = "is_subscription")]
        public bool IsSubscription { get; set; }

        [DataMember(Name = "create_time")]
        public string CreateTime { get; set; }

        [DataMember(Name = "reshare_id")]
        public string ReshareID { get; set; }

        [DataMember(Name = "activity")]
        public string Activity { get; set; }

        [IgnoreDataMember]
        public bool HasImages { get; set; }

        [DataMember(Name = "images")]
        public IList<PictureItemBase> Images { get; set; }

        [DataMember(Name = "comments_count")]
        public string CommentsCount { get; set; }

        [DataMember(Name = "id")]
        public string ID { get; set; }

        [IgnoreDataMember]
        public bool HasCard { get; set; }

        [DataMember(Name = "card")]
        public StatusCard Card { get; set; }

    }
}
