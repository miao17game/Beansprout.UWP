using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.ListModel {
    public class MovieItem {

        public ItemType Type { get; set; }

        public object OriginalPrice { get; set; }

        public string Info { get; set; }

        public uint ID { get; set; }

        public string SubType{ get; set; }

        public string Title { get; set; }

        public string ReviewerName { get; set; }

        public object Price { get; set; }

        public string DispatchUrl { get; set; }

        public string PathUrl { get; set; }

        public string PathInnerUri { get; set; }

        public bool HasCover { get; set; }

        public IList<string> Directors { get; set; }

        public IList<string> Actors { get; set; }

        public IList<string> Actions { get; set; }

        public string Description { get; set; }

        public object Date { get; set; }

        public object Label { get; set; }

        public string TypeString { get; set; }

        public string ReleaseDate { get; set; }

        #region Cover

        public Uri CoverUrl { get; set; }

        public double CoverWidth { get; set; }

        public double CoverHeight { get; set; }

        public string CoverShape { get; set; }

        #endregion

        #region Rating

        public uint RatingCount { get; set; }

        public uint RatingMax { get; set; }

        public double RatingValue { get; set; }

        #endregion

        public enum ItemType { Movie, }

    }
}
