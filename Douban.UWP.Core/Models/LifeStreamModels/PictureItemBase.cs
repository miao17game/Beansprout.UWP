using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.LifeStreamModels {
    [DataContract]
    public class PictureItemBase {

        [IgnoreDataMember]
        public Uri Normal { get; set; }

        [DataMember(Name = "normal")]
        public ImageJson NormalString { get; set; }

        [IgnoreDataMember]
        public Uri Small { get; set; }

        [DataMember(Name = "small")]
        public ImageJson SmallString { get; set; }

        [IgnoreDataMember]
        public Uri Large { get; set; }

        [DataMember(Name = "large")]
        public ImageJson LargeString { get; set; }

    }

    [DataContract]
    public  class ImageJson {

        [DataMember(Name = "url")]
        public string UrlString { get; set; }

        [IgnoreDataMember]
        public Uri Url { get { return this.UrlString == null || this.UrlString == "" ? null : new Uri(this.UrlString); } }

    }

}
