using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models {
    public class NavigationBar {

        #region Fields
        private FrameType dataType = FrameType.LeftPart;
        #endregion

        public string Title { get; set; }
        public string IdentityToken { get; set; }
        public Uri PathUri { get; set; }
        public NavigateType NaviType { get; set; }
        public FrameType FrameType {
            get { return dataType; }
            set { dataType = value; }
        }

    }
}
