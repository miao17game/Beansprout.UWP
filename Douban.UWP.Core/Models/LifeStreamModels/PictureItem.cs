using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.LifeStreamModels {
    public class PictureItem : PictureItemBase {

        public AuthorStatus Author { get; set; }

        public string Id { get; set; }

        public string LikersCount { get; set; }

        public string CommentsCount { get; set; }

        public bool AllowComment { get; set; }

        public string Type { get; set; }

        public string Uri { get; set; }

        public string Url { get; set; }

        public string SharingUrl { get; set; }

        public string CreateTime { get; set; }

        public int Position { get; set; }

        public string OwnedUri { get; set; }

        public string Description { get; set; }

        public bool Liked { get; set; }

    }
}
