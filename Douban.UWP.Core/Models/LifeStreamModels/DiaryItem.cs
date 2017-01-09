using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.LifeStreamModels {
    public class DiaryItem : InfosItemBase {

        public string ID { get; set; }

        public string Domain { get; set; }

        public string SharingUrl { get; set; }

        public string Title { get; set; }

        public string Abstract { get; set; }

        public bool HasCover { get; set; }

        public Uri Cover { get; set; }

        public int TimeLineShareCount { get; set; }

        public string UpdateTime { get; set; }

        public string Url { get; set; }

        public bool AllowComment { get; set; }

        public AuthorStatus Author { get; set; }

    }
}
