using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.ListModel {
    public class ItemGroup<T> : ItemGroupBase {

        public new IList<T> Items { get; set; }

    }
}
