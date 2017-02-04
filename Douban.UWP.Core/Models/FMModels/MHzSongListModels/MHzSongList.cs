using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.FMModels.MHzSongListModels {
    public class MHzSongList : FMListProgramme {
        public int Count { get; set; }
        public int CommentsCount { get; set; }
        public string CreateTime { get; set; }
        public bool CanCollect { get; set; }
        public IList<MHzSong> Songs { get; set; }
    }

    public class MHzSong : MHzListSong {
        public int TasteStatus { get; set; }
        public new IList<MHzSinger> Singers { get; set; }
        public MHzSongItemInfo ItemInfo { get; set; }

        public new string SingerShow {
            get { return string.Join(",", ((Singers ?? new List<MHzSinger>()).Select(i => i.Name)) ?? new string[] { "Unknown" }); }
        }

    }

    public class MHzSinger : MHzListSinger { }

    public class MHzSongItemInfo {
        public string ItemID { get; set; }
        public string Comment { get; set; }
        public string CreateTime { get; set; }
    }

}
