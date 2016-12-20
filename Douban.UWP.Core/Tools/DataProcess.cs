using DBCSCodePage;
using Wallace.UWP.Helpers.Controls;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

using static Wallace.UWP.Helpers.Tools.UWPStates;
using Douban.UWP.Core.Tools.PersonalExpressions;

namespace Douban.UWP.Core.Tools {
    public static class DataProcess {

        #region Properties and State
        
        #endregion

        public static Uri ConvertToUri(string str) { return !string.IsNullOrEmpty(str) ? new Uri(str) : null; }

    }
}
