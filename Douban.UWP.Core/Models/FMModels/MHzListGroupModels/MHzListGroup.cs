using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.FMModels {

    public static class MHzSongBaseHelper {
        public static string GetIdentity(MHzSongBase song) {
            return IdentityWithSHA256(song);
        }

        public static string GetIdentity(MusicBoardParameter para) {
            return IdentityWithSHA256(para);
        }

        private static string IdentityWithID(MHzSongBase song) {
            return song.SID + "SS" + song.AID + "AA" + song.SSID;
        }

        public static string IdentityWithID(MusicBoardParameter para) {
            return para.SID + "SS" + para.AID + "AA" + para.SSID;
        }

        private static string IdentityWithSHA256(MHzSongBase song) {
            return song.SHA256;
        }

        public static string IdentityWithSHA256(MusicBoardParameter para) {
            return para.SHA256;
        }

    }

    [DataContract]
    public class MHzGroupBase {

        [DataMember]
        public int R { get; set; }
        [DataMember]
        public int VersionMAX { get; set; }
        [DataMember]
        public int IsShowQuickStart { get; set; }
        [DataMember]
        public IList<MHzSongBase> Songs { get; set; }

    }

    [DataContract]
    public class MHzSongBase : SongBase {

        public MHzSongBase() { }

        public MHzSongBase(SongBase baseOne) {
            this.AID = baseOne.AID;
            this.Album = baseOne.Album;
            this.AlbumTitle = baseOne.AlbumTitle;
            this.AlertMessage = baseOne.AlertMessage;
            this.Artist = baseOne.Artist;
            this.FileExtensionName = baseOne.FileExtensionName;
            this.IsRoyal = baseOne.IsRoyal;
            this.Kbps = baseOne.Kbps;
            this.Length = baseOne.Length;
            this.Picture = baseOne.Picture;
            this.PublicTime = baseOne.PublicTime;
            this.Release = baseOne.Release;
            this.SHA256 = baseOne.SHA256;
            this.SID = baseOne.SID;
            this.Singers = baseOne.Singers;
            this.SSID = baseOne.SSID;
            this.Status = baseOne.Status;
            this.SubType = baseOne.SubType;
            this.Title = baseOne.Title;
            this.UpdateTime = baseOne.UpdateTime;
            this.Url = baseOne.Url;
        }

        [DataMember]
        public bool IsCached { get; set; }
        [DataMember]
        public string LocalPath { get; set; }
        [DataMember]
        public bool IsSelect { get; set; }
        [DataMember(Name ="like")]
        public int LikeCount { get; set; }

    }

    [DataContract]
    public class SongBase {
        
        [DataMember(Name = "albumtitle")]
        public string AlbumTitle { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "file_ext")]
        public string FileExtensionName { get; set; }

        [DataMember(Name = "album")]
        public string Album { get; set; }

        [DataMember(Name = "ssid")]
        public string SSID { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "sid")]
        public string SID { get; set; }

        [DataMember(Name = "sha256")]
        public string SHA256 { get; set; }

        [DataMember(Name = "status")]
        public int Status { get; set; }

        [DataMember(Name = "picture")]
        public string Picture { get; set; }

        [DataMember(Name = "update_time")]
        public long UpdateTime { get; set; }

        [DataMember(Name = "alert_msg")]
        public string AlertMessage { get; set; }

        [DataMember(Name = "public_time")]
        public string PublicTime { get; set; }

        [DataMember(Name = "artist")]
        public string Artist { get; set; }

        [DataMember(Name = "is_royal")]
        public bool IsRoyal { get; set; }

        [DataMember(Name = "subtype")]
        public string SubType { get; set; }

        [DataMember(Name = "length")]
        public int Length { get; set; }

        [DataMember(Name = "aid")]
        public string AID { get; set; }

        [DataMember(Name = "kbps")]
        public string Kbps { get; set; }

        [DataMember(Name = "singers")]
        public IList<MHzSingerBase> Singers { get; set; }

        [DataMember(Name = "release")]
        public MHzListRelease Release { get; set; }

        [IgnoreDataMember]
        public virtual string SingerShow {
            get { return string.Join(",", ((Singers ?? new List<MHzSingerBase>()).Select(i => i.Name)) ?? new string[] { "Unknown" }); }
        }
    }

    [DataContract]
    public class MHzSingerBase {

        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "region")]
        public IList<string> Region { get; set; }
        [DataMember(Name = "name_usual")]
        public string NameUsual { get; set; }
        [DataMember(Name = "genre")]
        public IList<string> Genre { get; set; }
        [DataMember(Name = "avatar")]
        public string Avatar { get; set; }
        [DataMember(Name = "related_site_id")]
        public int RelatedSiteId { get; set; }
        [DataMember(Name = "is_site_artist")]
        public bool IsSiteArtist { get; set; }
        [DataMember(Name = "id")]
        public string ID { get; set; }

    }

    [DataContract]
    public class MHzListRelease {

        [DataMember(Name = "id")]
        public string ID { get; set; }
        [DataMember(Name = "ssid")]
        public string SSID { get; set; }
        [DataMember(Name = "link")]
        public string Link { get; set; }

    }

}
