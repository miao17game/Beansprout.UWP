using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.FMModels {
    public class FMListGroup {

        public IList<FMListProgramme> Programmes { get; set; }
        public int GroupID { get; set; }
        public string GroupName { get; set; }

    }
}
