using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.LifeStreamModels {
    [DataContract]
    public class LifeStreamItem : InfosItemBase {

        [DataMember(Name = "url")]
        public string PathUrl { get; set; }

        #region Content

        [DataMember(Name = "activity")]
        public string Activity { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "des")]
        public string Description { get; set; }

        [DataMember(Name = "type")]
        public string ContentType { get; set; }

        [IgnoreDataMember]
        public bool HasCover { get; set; }

        [DataMember(Name = "cover")]
        public Uri Cover { get; set; }

        [DataMember(Name = "large")]
        public string Text { get; set; }

        [IgnoreDataMember]
        public bool HasImages { get; set; }

        [DataMember(Name = "images")]
        public IList<PictureItemBase> Images { get; set; }

        [DataMember(Name = "abstract")]
        public string Abstract { get; set; }

        [IgnoreDataMember]
        public bool HasAlbum { get; set; }

        [DataMember(Name = "album")]
        public IList<PictureItem> AlbumList { get; set; }

        #endregion

        [IgnoreDataMember]
        public double TimeForOrder { get; set; }

    }
}
