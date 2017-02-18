using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.ListModel {

    [DataContract]
    public class MovieItem : ListItemBase {

        [DataMember(Name = "directors")]
        public IList<string> Directors { get; set; }

        [DataMember(Name = "actors")]
        public IList<string> Actors { get; set; }

    }
}
