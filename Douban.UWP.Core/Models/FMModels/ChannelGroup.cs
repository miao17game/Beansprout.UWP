using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.FMModels {
    public class ChannelGroup {

        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public IList<ChannelsItem> CHLS { get; set; }

    }
}
