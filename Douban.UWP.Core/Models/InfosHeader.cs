using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models {
    [DataContract]
    public class InfosHeader {
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string PathHref { get; set; }
        [DataMember]
        public int Number { get; set; }
    }
}
