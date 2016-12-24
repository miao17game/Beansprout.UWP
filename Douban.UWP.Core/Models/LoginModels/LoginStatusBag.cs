using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models {
    public class LoginStatusBag {

        public string UserName { get; set; }

        public Uri ImageUrl { get; set; }

        public Uri BigHeadUrl { get; set; }

        public string LocationString { get; set; }

        public Uri LocationUrl { get; set; }

        public string Description { get; set; }

    }
}
