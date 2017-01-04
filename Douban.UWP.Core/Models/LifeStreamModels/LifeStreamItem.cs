using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.LifeStreamModels {
    public class LifeStreamItem : InfosItemBase {

        public string PathUrl { get; set; }

        #region Content

        public string Title { get; set; }

        public string Description { get; set; }

        public string ContentType { get; set; }

        public bool HasCover { get; set; }

        public Uri Cover { get; set; }

        public string Abstract { get; set; }

        public bool HasAlbum { get; set; }

        public IList<PictureItem> AlbumList { get; set; }

        #endregion

        public double TimeForOrder { get; set; }

    }
}
