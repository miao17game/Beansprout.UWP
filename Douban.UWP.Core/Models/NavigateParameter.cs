using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models {
    public class NavigateParameter {
        public Uri ToUri { get; set; }
        public string Title { get; set; }
        public string ApiHeadString { get; set; }
        public object SpecialParameter { get; set; }
        public bool IsFromInfoClick { get; set; }
        public bool IsNative { get; set; }
        public string UserUid { get; set; }

        private FrameType frameType = FrameType.Content;
        public FrameType FrameType {
            get { return frameType; }
            set { frameType = value; }
        }

    }
}
