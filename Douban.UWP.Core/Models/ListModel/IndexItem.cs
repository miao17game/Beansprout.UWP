using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.ListModel {
    [DataContract]
    public class IndexItem {

        public ItemType Type { get; set; }

        public string ThisDate { get; set; }

        [DataMember(Name = "id")]
        public string ID { get; set; }

        public int ReadCount { get => Target?.ReadCount ?? 0; }

        public int PhotosCount { get => Target?.PhotesCount ?? 0; }

        public int LikersCount { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        public string ImpressionUrl { get => Target?.ImpressionUrl; }

        public string PathUrl { get => Target?.Uri; }

        public bool HasCover { get => Target?.CoverUrl != null; }

        public string Cover { get => Target?.CoverUrl; }

        [DataMember(Name = "source")]
        public string Source { get; set; }

        [DataMember(Name = "source_cn")]
        public string SourceCN { get; set; }

        public int CommentCount { get; set; }

        public object OutSource { get; set; }

        public IList<object> Comments { get; set; }

        public string Action { get; set; }

        public string SourceHead { get; set; }

        public bool HasSourceHead { get; set; }

        public string Description { get => Target?.Description; }

        public IList<string> MorePictures { get => Target?.MorePicturesUrls; }

        public string MonitorUrl { get; set; }

        #region Author

        public string AuthorName { get => Target?.Author?.AuthorName; }

        public string AuthorAvatar { get => Target?.Author?.AuthorAvatar; }

        #endregion

        #region Column

        public string ColumnUrl { get; set; }

        public string ColumnName {
            get =>
                SourceCN != null && SourceCN != "" ?
                SourceCN : 
                Card?.Name ??
                "Beansprout UWP";
        }

        public int? ColumnId { get; set; }

        public Uri ColumnCover { get; set; }

        #endregion

        public enum ItemType { Normal, Gallary, Paragraph, DateBlock }

        [DataMember(Name = "target")]
        public IndexTarget Target { get; set; }

        [DataMember(Name = "card")]
        public IndexCard Card { get; set; }

    }

    [DataContract]
    public class IndexUser {

        [DataMember(Name = "name")]
        public string AuthorName { get; set; }

        [DataMember(Name = "avatar")]
        public string AuthorAvatar { get; set; }

    }

    [DataContract]
    public class IndexTarget {

        [DataMember(Name ="kind")]
        public int Kind { get; set; }

        [DataMember(Name = "photos_count")]
        public int PhotesCount { get; set; }

        [DataMember(Name = "uri")]
        public string Uri { get; set; }

        [DataMember(Name = "more_pic_urls")]
        public IList<string> MorePicturesUrls { get; set; }

        [DataMember(Name = "monitor_urls")]
        public IList<string> MoonitorUrls { get; set; }

        [DataMember(Name = "cover_url")]
        public string CoverUrl { get; set; }

        [DataMember(Name = "read_count")]
        public int ReadCount { get; set; }

        [DataMember(Name = "id")]
        public int ID { get; set; }

        [DataMember(Name = "impression_url")]
        public string ImpressionUrl { get; set; }

        [DataMember(Name = "author")]
        public IndexUser Author { get; set; }

        [DataMember(Name = "desc")]
        public string Description { get; set; }

    }

    [DataContract]
    public class IndexCard {

        [DataMember(Name = "name")]
        public string Name { get; set; }

    }

}
