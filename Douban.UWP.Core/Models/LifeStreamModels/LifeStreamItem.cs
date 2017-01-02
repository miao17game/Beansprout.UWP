using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.LifeStreamModels {
    public class LifeStreamItem {

        public string LikersCounts { get; set; }

        public string Time { get; set; }

        public string Uri { get; set; }

        public string PathUrl { get; set; }

        public string CommentsCounts { get; set; }

        public string Activity { get; set; }

        public JsonType Type { get; set; }

        #region Content

        public string Title { get; set; }

        public string Text { get; set; }

        public string Description { get; set; }

        public string ContentType { get; set; }

        public bool HasCover { get; set; }

        public Uri Cover { get; set; }

        public string Abstract { get; set; }

        public bool HasImages { get; set; }

        public bool HasAlbum { get; set; }

        public IList<PictureItemBase> Images { get; set; }

        public IList<PictureItem> AlbumList { get; set; }

        #endregion

        public double TimeForOrder { get; set; }

        public enum JsonType { Undefined, Card, Album, Article, Status}

    }
}
