using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.ListModel {
    public class ItemGroupBase {

        public uint NowCount { get; set; }

        public uint StartIndex { get; set; }

        public uint Total { get; set; }

        public string GroupName { get; set; }

        public string GroupPathUrl { get; set; }

        public string GroupInnerUri { get; set; }

        public string Description { get; set; }

        public string SharingUrl { get; set; }

        public string Id { get; set; }

        public string CoverUrl { get; set; }

        public bool HasCover { get; set; }

        public virtual IList<object> Items { get; set; }

    }
}
