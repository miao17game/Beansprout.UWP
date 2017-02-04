using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.FMModels {
    public enum MHzListType {
        MHzRecommand,
        MHzList
    }

    public class MHzParameter : NavigateParameterBase {
        public string SerializedParameter { get; set; }
        public MHzListType Type { get; set; }
    }

}
