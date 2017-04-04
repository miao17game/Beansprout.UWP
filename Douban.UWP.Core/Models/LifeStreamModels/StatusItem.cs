using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.LifeStreamModels {
    [DataContract]
    public class StatusItem : InfosItemBase {

        [DataMember(Name ="id")]
        public int ID { get; set; }

        [DataMember(Name = "activity")]
        public string Activity { get; set; }

        [DataMember(Name = "reshares_count")]
        public string ResharesCounts { get; set; }

        [DataMember(Name = "resharers_count")]
        public string ResharersCounts { get; set; }

        [DataMember(Name = "liked")]
        public bool Liked { get; set; }

        [DataMember(Name = "sharing_url")]
        public string SharingUrl { get; set; }

        [DataMember(Name = "parent_status")]
        public object ParentStatus { get; set; }

        [DataMember(Name = "reshare_id")]
        public string ReshareID { get; set; }

        [IgnoreDataMember]
        public bool HasResharedStatus { get { return this.ResharedStatus != null; } }

        [DataMember(Name = "reshared_status")]
        public StatusResharedStatus ResharedStatus { get; set; }

        [IgnoreDataMember]
        public bool HasText { get { return this.Text != null && this.Text != ""; } }

        [DataMember(Name = "text")]
        public string Text { get; set; }

        [IgnoreDataMember]
        public bool HasImages { get { return this.Images != null && this.Images.Count != 0; } }

        [DataMember(Name = "images")]
        public IList<PictureItemBase> Images { get; set; }

        [IgnoreDataMember]
        public bool HasCard { get { return this.Card != null; } }

        [DataMember(Name = "card")]
        public StatusCard Card { get; set; }

        [IgnoreDataMember]
        public bool HasEntity { get { return this.Entities != null && this.Entities.Count != 0; } }

        [DataMember(Name = "entities")]
        public IList<StatusEntity> Entities { get; set; }

        [DataMember(Name = "author")]
        public AuthorStatus Author { get; set; }

        [IgnoreDataMember]
        public bool HasComment { get { return this.Comments != null && this.Comments.Count != 0; } }

        [DataMember(Name = "comments")]
        public IList<object> Comments { get; set; }

    }
}
