using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.FMModels.MHzSongListModels {

    [DataContract]
    public class MHzSongList : FMListProgramme {

        [DataMember(Name = "count")]
        public int Count { get; set; }
        [DataMember(Name = "comments_count")]
        public int CommentsCount { get; set; }
        [DataMember(Name = "created_time")]
        public string CreateTime { get; set; }
        [DataMember(Name = "can_collect")]
        public bool CanCollect { get; set; }
        [DataMember(Name = "songs")]
        public IList<MHzSong> Songs { get; set; }
    }

    [DataContract]
    public class MHzSong : MHzSongBase {

        [DataMember(Name = "taste_status")]
        public int TasteStatus { get; set; }
        [DataMember(Name = "item_info")]
        public MHzSongItemInfo ItemInfo { get; set; }

    }

    [DataContract]
    public class MHzSongItemInfo {

        [DataMember(Name = "item_id")]
        public string ItemID { get; set; }
        [DataMember(Name = "comment")]
        public string Comment { get; set; }
        [DataMember(Name = "created_time")]
        public string CreateTime { get; set; }
    }

}
