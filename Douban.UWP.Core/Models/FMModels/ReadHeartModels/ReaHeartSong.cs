using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.FMModels.ReadHeartModels {

    [DataContract]
    public class RedHeartList {

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "collected_count")]
        public int CollectedCount { get; set; }

        [DataMember(Name = "offshelf_alert")]
        public string OffshelfAlert { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "cover")]
        public string Cover { get; set; }

        [DataMember(Name = "updated_time")]
        public string UpdateTime { get; set; }

        [DataMember(Name = "is_collected")]
        public bool IsCollected { get; set; }

        [DataMember(Name = "rec_reason")]
        public string RecReason { get; set; }

        [DataMember(Name = "created_time")]
        public string CreateTime { get; set; }

        [DataMember(Name = "can_play")]
        public string CanPlay { get; set; }

        [DataMember(Name = "type")]
        public int Type { get; set; }

        [DataMember(Name = "id")]
        public int ID { get; set; }

        [DataMember(Name = "songs")]
        public IList<RedHeartSong> Songs { get; set; }

    }

    [DataContract]
    public class RedHeartSong {

        [DataMember(Name = "update_time")]
        public long UpdateTime { get; set; }

        [DataMember(Name = "playable")]
        public string Playable { get; set; }

        [DataMember(Name = "sid")]
        public string SID { get; set; }

        [DataMember(Name = "like")]
        public int Like { get; set; }

    }

    [DataContract]
    public class RedHeartListCreator {

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "picture")]
        public string Picture { get; set; }

        [DataMember(Name = "id")]
        public string ID { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

    }
}
