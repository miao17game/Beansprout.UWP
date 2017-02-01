using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.FMModels {
    public class ChannelsItem {

        public ChannelStyle Style { get; set; }
        public string Intro { get; set; }
        public string Name { get; set; }
        public int SongNumber { get; set; }
        public string Collected { get; set; }
        public string Cover { get; set; }
        public int Id { get; set; }
        public int? ChannelType { get; set; }
        public ChannelRelation Relation { get; set; }

    }

    public class ChannelStyle {
        public string DisplayText { get; set; }
        public string BgColor { get; set; }
        public int LayoutType { get; set; }
        public string BgImage { get; set; }
    }

    public class ChannelRelation {
        public string ArtistID { get; set; }
        public string SongID { get; set; }
        public string SSID { get; set; }
    }

}
