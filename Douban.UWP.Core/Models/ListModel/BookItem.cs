using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.ListModel {
    public class BookItem : ListItemBase {

        public new object ReleaseDate { get; set; }

        public new string ID { get; set; }

    }
}
