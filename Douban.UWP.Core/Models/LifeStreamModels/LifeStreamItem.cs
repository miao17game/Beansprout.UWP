using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.LifeStreamModels {
    //[DataContract]
    //public class LifeStreamItem : InfosItemBase {

    //    [DataMember(Name = "url")]
    //    public string PathUrl { get; set; }

    //    #region Content

    //    [DataMember(Name = "activity")]
    //    public string Activity { get; set; }

    //    [DataMember(Name = "title")]
    //    public string Title { get; set; }

    //    [DataMember(Name = "des")]
    //    public string Description { get; set; }

    //    [DataMember(Name = "type")]
    //    public string ContentType { get; set; }

    //    [IgnoreDataMember]
    //    public bool HasCover { get; set; }

    //    [DataMember(Name = "cover")]
    //    public Uri Cover { get; set; }

    //    [DataMember(Name = "large")]
    //    public string Text { get; set; }

    //    [IgnoreDataMember]
    //    public bool HasImages { get; set; }

    //    [DataMember(Name = "images")]
    //    public IList<PictureItemBase> Images { get; set; }

    //    [DataMember(Name = "abstract")]
    //    public string Abstract { get; set; }

    //    [IgnoreDataMember]
    //    public bool HasAlbum { get; set; }

    //    [DataMember(Name = "album")]
    //    public IList<PictureItem> AlbumList { get; set; }

    //    #endregion

    //    [IgnoreDataMember]
    //    public double TimeForOrder { get; set; }

    //}

    [DataContract]
    public class ListStreamOne {

        [DataMember(Name = "items")]
        public LifeStreamItem[] Items { get; set; }

        [DataMember(Name = "next_filter_after")]
        public string NextFilter { get; set; }

    }

    [DataContract]
    public class LifeStreamItem {

        [DataMember(Name = "rating")]
        public object Rating { get; set; }

        [DataMember(Name = "likers_count")]
        public int LikersCount { get; set; }

        [DataMember(Name = "time")]
        public string Time { get; set; }

        [DataMember(Name = "uri")]
        public string Uri { get; set; }

        [DataMember(Name = "content")]
        public LifeStreamContent Content { get; set; }

        [DataMember(Name = "url")]
        public string PathUrl { get; set; }

        [DataMember(Name = "comments_count")]
        public int CommentsCount { get; set; }

        [DataMember(Name = "activity")]
        public string Activity { get; set; }

        [DataMember(Name = "type")]
        public string ContentType { get; set; }

        [IgnoreDataMember]
        public string Cover {
            get => this.Content != null ?
                this.Content.CoverUrl != null && this.Content.CoverUrl != "" ?
                this.Content.CoverUrl 
                : NoPictureUrl
                : NoPictureUrl;
        }

        [IgnoreDataMember]
        public bool HasCover {
            get =>
                this.Content != null ?
                this.Content.CoverUrl != null && this.Content.CoverUrl != "" ?
                true : false : false;
        }

        [IgnoreDataMember]
        public bool HasImages {
            get =>
                this.Content != null ?
                this.Content.Images != null && this.Content.Images.Count() > 0 ?
                true : false : false;
        }

        [IgnoreDataMember]
        public bool HasAlbum {
            get =>
                this.Content != null ?
                this.Content.Photos != null && this.Content.Photos.Count() > 0 ?
                true : false : false;
        }

        [IgnoreDataMember]
        public double TimeForOrder {
            get {
                var time = default(double);
                try {
                    time = (DateTime.Parse(this.Time) - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                } catch { time = 0; }
                return time;
            }
        }

        const string NoPictureUrl = "https://www.none-wallace-767fc6vh7653df0jb.com/no_wallace_085sgdfg7447fddds65.jpg";

    }

    [DataContract]
    public class LifeStreamContent {

        [DataMember(Name = "card_uri")]
        public string CardUri { get; set; }

        [DataMember(Name = "card_url")]
        public string CardUrl { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "cover_url")]
        public string CoverUrl { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "abstract")]
        public string Abstract { get; set; }

        [DataMember(Name = "images")]
        public LifeStreamImage[] Images { get; set; }

        [DataMember(Name = "photos")]
        public LifeStreamPhoto[] Photos { get; set; }

        [DataMember(Name = "photos_count")]
        public int PhotosCount { get; set; }
    }

    [DataContract]
    public class LifeStreamImage {

        [DataMember(Name = "large")]
        public ImageSize Large { get; set; }

        [DataMember(Name = "is_animated")]
        public bool IsAnimated { get; set; }

        [DataMember(Name = "normal")]
        public ImageSize Normal { get; set; }

        [DataMember(Name = "small")]
        public ImageSize Small { get; set; }
    }

    [DataContract]
    public class ImageSize {

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "width")]
        public int Width { get; set; }

        [DataMember(Name = "height")]
        public int Height { get; set; }
    }

    [DataContract]
    public class LifeStreamPhoto {

        [DataMember(Name = "liked")]
        public bool Liked { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "author")]
        public LifeStreamAuthor Author { get; set; }

        [DataMember(Name = "likers_count")]
        public int LikersCount { get; set; }

        [DataMember(Name = "image")]
        public LifeStreamImage Image { get; set; }

        [DataMember(Name = "uri")]
        public string Uri { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "create_time")]
        public string CreateTime { get; set; }

        [DataMember(Name = "comments_count")]
        public int CommentsCount { get; set; }

        [DataMember(Name = "allow_comment")]
        public bool AllowComment { get; set; }

        [DataMember(Name = "position")]
        public int Position { get; set; }

        [DataMember(Name = "owner_uri")]
        public string OwnerUri { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "sharing_url")]
        public string SharingUrl { get; set; }
    }

    [DataContract]
    public class LifeStreamAuthor {

        [DataMember(Name = "loc")]
        public LifeStreamLoc Loc { get; set; }

        [DataMember(Name = "kind")]
        public string Kind { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "uri")]
        public string Uri { get; set; }

        [DataMember(Name = "avatar")]
        public string Avatar { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "uid")]
        public string Uid { get; set; }
    }

    [DataContract]
    public class LifeStreamLoc {

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "uid")]
        public string Uid { get; set; }
    }

}
