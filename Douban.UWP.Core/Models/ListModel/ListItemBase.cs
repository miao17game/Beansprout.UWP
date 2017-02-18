using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.ListModel {

    [DataContract]
    public abstract class ListItemBase {

        [IgnoreDataMember]
        public ItemType Type { get; set; }

        [DataMember(Name = "original_price")]
        public object OriginalPrice { get; set; }

        [DataMember(Name = "info")]
        public string Info { get; set; }

        [DataMember(Name = "id")]
        public object ID { get; set; }

        [DataMember(Name = "subtype")]
        public string SubType { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "reviewer_name")]
        public string ReviewerName { get; set; }

        [DataMember(Name = "price")]
        public object Price { get; set; }

        [IgnoreDataMember]
        public string DispatchUrl { get; set; }

        [DataMember(Name = "url")]
        public string PathUrl { get; set; }

        [DataMember(Name = "uri")]
        public string PathInnerUri { get; set; }

        [IgnoreDataMember]
        public bool HasCover { get; set; }

        [DataMember(Name = "actions")]
        public IList<string> Actions { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "date")]
        public object Date { get; set; }

        [DataMember(Name = "label")]
        public object Label { get; set; }

        [DataMember(Name = "type")]
        public string TypeString { get; set; }

        [DataMember(Name = "release_date")]
        public string ReleaseDate { get; set; }

        #region Cover

        [DataMember(Name = "cover")]
        public ItemCoverBase Cover { get; set; }

        [IgnoreDataMember]
        public Uri CoverUrl { get; set; }

        [IgnoreDataMember]
        public double CoverWidth { get; set; }

        [IgnoreDataMember]
        public double CoverHeight { get; set; }

        [IgnoreDataMember]
        public string CoverShape { get; set; }

        #endregion

        #region Rating

        [DataMember(Name = "rating")]
        public ItemRatingBase Rating { get; set; }

        [IgnoreDataMember]
        public uint RatingCount { get; set; }

        [IgnoreDataMember]
        public uint RatingMax { get; set; }

        [IgnoreDataMember]
        public double RatingValue { get; set; }

        #endregion

        public enum ItemType { Movie, Book, Music, FM, Read, Group, Location }

        [DataContract]
        public class ItemRatingBase {

            [DataMember(Name = "count")]
            public uint RatingCount { get; set; }

            [DataMember(Name = "max")]
            public uint RatingMax { get; set; }

            [DataMember(Name = "value")]
            public double RatingValue { get; set; }

        }

        [DataContract]
        public class ItemCoverBase {

            [DataMember(Name = "url")]
            public string CoverUrl { get; set; }

            [DataMember(Name = "width")]
            public double CoverWidth { get; set; }

            [DataMember(Name = "height")]
            public double CoverHeight { get; set; }

            [DataMember(Name = "shape")]
            public string CoverShape { get; set; }

        }

    }
}
