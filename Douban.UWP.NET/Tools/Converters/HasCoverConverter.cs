using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallace.UWP.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Douban.UWP.Tools.Converters {
    public class HasCoverConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            return ConvertToVisibility((string)value, (string)parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }

        private Visibility ConvertToVisibility(string value, string parameter) { return parameter == value ? Visibility.Visible : Visibility.Collapsed; }
    }
}
