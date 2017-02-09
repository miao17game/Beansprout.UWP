using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.FMModels {
    public class MHzGroupBase {

        public int R { get; set; }
        public int VersionMAX { get; set; }
        public int IsShowQuickStart { get; set; }
        public IList<MHzSongBase> Songs { get; set; }

    }

    public class MHzSongBase {

        public bool IsSelect { get; set; }

        public string AlbumTitle { get; set; }
        public string Url { get; set; }
        public string FileExtensionName { get; set; }
        public string Album { get; set; }
        public string SSID { get; set; }
        public string Title { get; set; }
        public string SID { get; set; }
        public string SHA256 { get; set; }
        public int Status { get; set; }
        public string Picture { get; set; }
        public long UpdateTime { get; set; }
        public string AlertMessage { get; set; }
        public string PublicTime { get; set; }
        public int LikeCount { get; set; }
        public string Artist { get; set; }
        public bool IsRoyal { get; set; }
        public string SubType { get; set; }
        public int Length { get; set; }
        public string AID { get; set; }
        public string Kbps { get; set; }
        public IList<MHzSingerBase> Singers { get; set; }
        public MHzListRelease Release { get; set; }

        public string SingerShow {
            get { return string.Join(",", ((Singers??new List<MHzSingerBase>()).Select(i => i.Name)) ?? new string[] { "Unknown" }); }
        }

    }

    public class MHzSingerBase {

        public string Name { get; set; }
        public IList<string> Region { get; set; }
        public string NameUsual { get; set; }
        public IList<string> Genre { get; set; }
        public string Avatar { get; set; }
        public int RelatedSiteId { get; set; }
        public bool IsSiteArtist { get; set; }
        public string ID { get; set; }

    }

    public class MHzListRelease {

        public string ID { get; set; }
        public string SSID { get; set; }
        public string Link { get; set; }

    }

}
