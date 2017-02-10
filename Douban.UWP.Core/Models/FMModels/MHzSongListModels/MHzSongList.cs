using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.FMModels.MHzSongListModels {

    [DataContract]
    public class MHzSongList : FMListProgramme {

        [DataMember]
        public int Count { get; set; }
        [DataMember]
        public int CommentsCount { get; set; }
        [DataMember]
        public string CreateTime { get; set; }
        [DataMember]
        public bool CanCollect { get; set; }
        [DataMember]
        public IList<MHzSong> Songs { get; set; }
    }

    [DataContract]
    public class MHzSong : MHzSongBase {

        [DataMember]
        public int TasteStatus { get; set; }
        [DataMember]
        public MHzSongItemInfo ItemInfo { get; set; }

    }

    [DataContract]
    public class MHzSongItemInfo {

        [DataMember]
        public string ItemID { get; set; }
        [DataMember]
        public string Comment { get; set; }
        [DataMember]
        public string CreateTime { get; set; }
    }

}
