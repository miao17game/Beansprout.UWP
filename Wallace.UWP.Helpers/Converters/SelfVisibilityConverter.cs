using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Wallace.UWP.Helpers.Converters {
    public class SelfVisibilityConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            return ConvertToVisibility((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }

        private Visibility ConvertToVisibility(string value) { return string.IsNullOrEmpty(value) ? Visibility.Collapsed : Visibility.Visible; }
    }
}
