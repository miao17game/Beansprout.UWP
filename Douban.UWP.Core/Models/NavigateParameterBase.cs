using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models {
    public class NavigateParameterBase {

        private FrameType frameType = FrameType.Content;
        public FrameType FrameType {
            get { return frameType; }
            set { frameType = value; }
        }

    }
}
