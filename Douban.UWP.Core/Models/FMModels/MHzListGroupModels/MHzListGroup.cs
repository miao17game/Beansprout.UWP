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
    public class MHzSongBase {
        [DataMember]
        public bool IsCached { get; set; }
        [DataMember]
        public string LocalPath { get; set; }
        [DataMember]
        public bool IsSelect { get; set; }
        [DataMember]
        public string AlbumTitle { get; set; }
        [DataMember]
        public string Url { get; set; }
        [DataMember]
        public string FileExtensionName { get; set; }
        [DataMember]
        public string Album { get; set; }
        [DataMember]
        public string SSID { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string SID { get; set; }
        [DataMember]
        public string SHA256 { get; set; }
        [DataMember]
        public int Status { get; set; }
        [DataMember]
        public string Picture { get; set; }
        [DataMember]
        public long UpdateTime { get; set; }
        [DataMember]
        public string AlertMessage { get; set; }
        [DataMember]
        public string PublicTime { get; set; }
        [DataMember]
        public int LikeCount { get; set; }
        [DataMember]
        public string Artist { get; set; }
        [DataMember]
        public bool IsRoyal { get; set; }
        [DataMember]
        public string SubType { get; set; }
        [DataMember]
        public int Length { get; set; }
        [DataMember]
        public string AID { get; set; }
        [DataMember]
        public string Kbps { get; set; }
        [DataMember]
        public IList<MHzSingerBase> Singers { get; set; }
        [DataMember]
        public MHzListRelease Release { get; set; }

        [IgnoreDataMember]
        public virtual string SingerShow {
            get { return string.Join(",", ((Singers??new List<MHzSingerBase>()).Select(i => i.Name)) ?? new string[] { "Unknown" }); }
        }

    }

    [DataContract]
    public class MHzSingerBase {

        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public IList<string> Region { get; set; }
        [DataMember]
        public string NameUsual { get; set; }
        [DataMember]
        public IList<string> Genre { get; set; }
        [DataMember]
        public string Avatar { get; set; }
        [DataMember]
        public int RelatedSiteId { get; set; }
        [DataMember]
        public bool IsSiteArtist { get; set; }
        [DataMember]
        public string ID { get; set; }

    }

    [DataContract]
    public class MHzListRelease {

        [DataMember]
        public string ID { get; set; }
        [DataMember]
        public string SSID { get; set; }
        [DataMember]
        public string Link { get; set; }

    }

}
