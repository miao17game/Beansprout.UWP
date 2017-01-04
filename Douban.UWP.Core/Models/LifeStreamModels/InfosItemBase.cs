using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.LifeStreamModels {
    public class InfosItemBase {

        public JsonType Type { get; set; }

        public string Time { get; set; }

        public string LikersCounts { get; set; }

        public string Text { get; set; }

        public string Uri { get; set; }

        public string Activity { get; set; }

        public string CommentsCounts { get; set; }

        public bool HasImages { get; set; }

        public IList<PictureItemBase> Images { get; set; }

        public enum JsonType { Undefined, Card, Album, Article, Status }

    }
}
