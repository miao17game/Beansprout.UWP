using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.FMModels {

    [DataContract]
    public class FMListProgramme {

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public int CollectedCount { get; set; }

        [DataMember]
        public FMListCreater Creator { get; set; }

        [DataMember]
        public int Duration { get; set; }

        [DataMember]
        public string RecommandReason { get; set; }

        [DataMember]
        public bool IsPublic { get; set; }

        [DataMember]
        public bool CanPlay { get; set; }

        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public int SongsCount { get; set; }

        [DataMember]
        public bool ShowNotPlayable { get; set; }

        [DataMember]
        public int CoverType { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Cover { get; set; }

        [DataMember]
        public string UpdateTime { get; set; }

        [DataMember]
        public bool IsCollected { get; set; }

        [DataMember]
        public int Type { get; set; }

    }

    [DataContract]
    public class FMListCreater {

        [DataMember]
        public string Picture { get; set; }

        [DataMember]
        public int SonglistsCount { get; set; }

        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public string ID { get; set; }

        [DataMember]
        public string Name { get; set; }
    }

}
