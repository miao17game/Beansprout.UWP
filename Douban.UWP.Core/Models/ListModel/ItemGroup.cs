using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.ListModel {

    [DataContract]
    public class ItemGroup<T> : ItemGroupBase {

        [DataMember(Name = "subject_collection_items")]
        public new IList<T> Items { get; set; }

    }
}
