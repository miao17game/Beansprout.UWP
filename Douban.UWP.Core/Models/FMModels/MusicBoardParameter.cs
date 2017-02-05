using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.FMModels {
    public class MusicBoardParameter : NavigateParameterBase {

        public string SID { get; set; }
        public string SSID { get; set; }
        public string AID { get; set; }

    }
}
