using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models {
    public class NavigateParameter {
        public Uri ToUri { get; set; }
        public string Title { get; set; }
        public object SpecialParameter { get; set; }
    }
}
