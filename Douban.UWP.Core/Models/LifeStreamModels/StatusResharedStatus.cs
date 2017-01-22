using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.LifeStreamModels {
    public class StatusResharedStatus {

        public string ResharesCount { get; set; }

        public bool IsLiked { get; set; }

        public string SharingUrl { get; set; }

        public AuthorStatus Author { get; set; }

        public string Text { get; set; }

        public string LikeCount { get; set; }

        public string Uri { get; set; }

        public string ResharersCount  { get; set; }

        public object ParentStatus { get; set; }

        public bool HasEntity { get; set; }

        public IList<StatusEntity> Entities { get; set; }

        public string SubscriptionText { get; set; }

        public bool IsSubscription { get; set; }

        public string CreateTime { get; set; }

        public string ReshareID { get; set; }

        public string Activity { get; set; }

        public bool HasImages { get; set; }

        public IList<PictureItemBase> Images { get; set; }

        public string CommentsCount { get; set; }

        public string ID { get; set; }

        public bool HasCard { get; set; }

        public StatusCard Card { get; set; }

    }
}
