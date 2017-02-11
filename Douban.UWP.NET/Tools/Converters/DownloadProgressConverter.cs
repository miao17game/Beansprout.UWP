using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallace.UWP.Helpers;
using Windows.Networking.BackgroundTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Douban.UWP.NET.Tools.Converters {
    public class DownloadProgressConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            return ConvertToUInt((BackgroundDownloadProgress)value, parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }

        private string ConvertToUInt(BackgroundDownloadProgress value, string parameter) {
            return parameter == "NowValue" ? (((double)value.BytesReceived) / (1024 * 1024)).ToString("#.##") + "MB":
                parameter == "WholeValue" ? (((double)value.TotalBytesToReceive)/ (1024 * 1024)).ToString("#.##") + "MB":
                parameter == "HasRespinseChanged" ? value.HasResponseChanged.ToString() :
                parameter == "HasStarted" ? value.HasRestarted.ToString() :
                parameter == "Status" ? value.Status.ToString() :
                null;
        }
    }
}
