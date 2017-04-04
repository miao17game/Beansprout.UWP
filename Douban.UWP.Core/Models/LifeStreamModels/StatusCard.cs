using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.LifeStreamModels {
    [DataContract]
    public class StatusCard {

        [DataMember(Name = "rating")]
        public string Rating { get; set; }

        [IgnoreDataMember]
        public bool HasRating { get { return this.Rating != null && this.Rating != ""; } }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "uri")]
        public string Uri { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "subtitle")]
        public string SubTitle { get; set; }

        [IgnoreDataMember]
        public bool HasImage { get { return Image != null; } }

        [DataMember(Name = "image")]
        public PictureItemBase Image { get; set; }

    }
}
