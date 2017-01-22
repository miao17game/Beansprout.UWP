using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.LifeStreamModels {
    public class StatusCard {

        public string Rating { get; set; }

        public bool HasRating { get; set; }

        public string Url { get; set; }

        public string Uri { get; set; }

        public string Title { get; set; }

        public string SubTitle { get; set; }

        public bool HasImage { get; set; }

        public PictureItemBase Image { get; set; }

    }
}
