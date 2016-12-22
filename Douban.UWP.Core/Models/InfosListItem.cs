using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models {
    [DataContract]
    public class InfosListItem {

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public Uri ImageSource { get; set; }

        [DataMember]
        public string Date { get; set; }

        [DataMember]
        public Uri Path { get; set; }

        [DataMember]
        public Uri Share { get; set; }

        [DataMember]
        public int ShareNum { get; set; }

        [DataMember]
        public int ID { get; set; }

    }
}
