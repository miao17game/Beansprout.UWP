using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.LifeStreamModels {
    public class StatusItem : InfosItemBase {

        public int ID { get; set; }

        public string Activity { get; set; }

        public string ResharesCounts { get; set; }

        public string ResharersCounts { get; set; }

        public bool Liked { get; set; }

        public string SharingUrl { get; set; }

        public object ParentStatus { get; set; }

        public string ReshareID { get; set; }

        public bool HasResharedStatus { get; set; }

        public StatusResharedStatus ResharedStatus { get; set; }

        public bool HasText { get; set; }

        public string Text { get; set; }

        public bool HasImages { get; set; }

        public IList<PictureItemBase> Images { get; set; }

        public bool HasCard { get; set; }

        public StatusCard Card { get; set; }

        public bool HasEntity { get; set; }

        public IList<StatusEntity> Entities { get; set; }

        public AuthorStatus Author { get; set; }

        public bool HasComment { get; set; }

        public IList<object> Comments { get; set; }

    }
}
