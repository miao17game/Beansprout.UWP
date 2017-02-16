using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.FMModels {

    [DataContract]
    public class FMListProgramme {

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "collected_count")]
        public int CollectedCount { get; set; }

        [DataMember(Name = "creator")]
        public FMListCreater Creator { get; set; }

        [DataMember(Name = "duration")]
        public int Duration { get; set; }

        [DataMember(Name = "rec_reason")]
        public string RecommandReason { get; set; }

        [DataMember(Name = "is_public")]
        public bool IsPublic { get; set; }

        [DataMember(Name = "can_play")]
        public bool CanPlay { get; set; }

        [DataMember(Name = "id")]
        public int ID { get; set; }

        [DataMember(Name = "songs_count")]
        public int SongsCount { get; set; }

        [DataMember(Name = "show_not_playable")]
        public bool ShowNotPlayable { get; set; }

        [DataMember(Name = "cover_type")]
        public int CoverType { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "cover")]
        public string Cover { get; set; }

        [DataMember(Name = "updated_time")]
        public string UpdateTime { get; set; }

        [DataMember(Name = "is_collected")]
        public bool IsCollected { get; set; }

        [DataMember(Name = "type")]
        public int Type { get; set; }

    }

    [DataContract]
    public class FMListCreater {

        [DataMember(Name = "picture")]
        public string Picture { get; set; }

        [DataMember(Name = "songlists_count")]
        public int SonglistsCount { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "id")]
        public string ID { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
    }

}
