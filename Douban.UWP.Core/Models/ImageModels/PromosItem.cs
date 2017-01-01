using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.ImageModels {
    public class PromosItem {

        public uint NotificationCount { get; set; }

        public string NotificationVersion { get; set; }

        public string Image { get; set; }

        public Uri ImageSrc { get; set; }

        public string Uri { get; set; }

        public string Tag { get; set; }

        public string Text { get; set; }

        public string ID { get; set; }

    }
}
