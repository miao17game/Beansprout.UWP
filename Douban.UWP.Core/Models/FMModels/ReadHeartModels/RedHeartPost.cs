using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.FMModels.ReadHeartModels {

    [DataContract]
    public class RedHeartPost {

        [DataMember(Name = "type", Order = 0)]
        public string Type { get; set; }

        [DataMember(Name = "play_mode", Order = 1)]
        public string PlayMode { get; set; }

        [DataMember(Name = "play_source", Order = 2)]
        public string PlaySource { get; set; }

        [DataMember(Name = "sid", Order = 3)]
        public string SID { get; set; }

        [DataMember(Name = "time", Order = 4)]
        public string Time { get; set; }

        [DataMember(Name = "pid", Order = 5)]
        public long PID { get; set; }

    }
}
