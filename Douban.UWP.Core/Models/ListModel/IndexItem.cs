using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.ListModel {
    public class IndexItem {

        public ItemType Type { get; set; }

        public string ThisDate { get; set; }

        public uint ID { get; set; }

        public uint ReadCount { get; set; }

        public uint PhotosCount { get; set; }

        public uint LikersCount { get; set; }

        public string Title { get; set; }

        public string ImpressionUrl { get; set; }

        public string PathUrl { get; set; }

        public bool HasCover { get; set; }

        public Uri Cover { get; set; }

        public string Source { get; set; }

        public uint CommentCount { get; set; }

        public object OutSource { get; set; }

        public IList<object> Comments { get; set; }

        public string Action { get; set; }

        public string SourceHead { get; set; }

        public bool HasSourceHead { get; set; }

        public string Description { get; set; }

        public IList<Uri> MorePictures { get; set; }

        public string MonitorUrl { get; set; }

        #region Author

        public string AuthorName { get; set; }

        public Uri AuthorAvatar { get; set; }

        #endregion

        #region Column

        public string ColumnUrl { get; set; }

        public string ColumnName { get; set; }

        public int? ColumnId { get; set; }

        public Uri ColumnCover { get; set; }

        #endregion

        public enum ItemType { Normal, Gallary, Paragraph, DateBlock }

    }
}
