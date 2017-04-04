using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.LifeStreamModels {

    [DataContract]
    public class PositionStatus {

        [DataMember(Name = "id")]
        public string PositionID { get; set; }

        [DataMember(Name = "name")]
        public string LocationName { get; set; }

        [DataMember(Name = "uid")]
        public string LocationUid { get; set; }

    }
}
