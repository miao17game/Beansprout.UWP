using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.FMModels {
    public class FMListProgramme {

        public string Description { get; set; }
        public int CollectedCount { get; set; }
        public FMListCreater Creator { get; set; }
        public int Duration { get; set; }
        public string RecommandReason { get; set; }
        public bool IsPublic { get; set; }
        public bool CanPlay { get; set; }
        public int ID { get; set; }
        public int SongsCount { get; set; }
        public bool ShowNotPlayable { get; set; }
        public int CoverType { get; set; }
        public string Title { get; set; }
        public string Cover { get; set; }
        public string UpdateTime { get; set; }
        public bool IsCollected { get; set; }
        public int Type { get; set; }

    }

    public class FMListCreater {
        public string Picture { get; set; }
        public int SonglistsCount { get; set; }
        public string Url { get; set; }
        public string Type { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
    }

}
