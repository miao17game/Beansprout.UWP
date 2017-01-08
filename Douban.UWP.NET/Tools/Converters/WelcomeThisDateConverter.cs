using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallace.UWP.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Douban.UWP.Tools.Converters {
    public class WelcomeThisDateConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            return value == null ? Visibility.Visible : ConvertToVisibility(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }

        private Visibility ConvertToVisibility(string value) {
            DateTime date = default(DateTime);
            try { date = DateTime.Parse(value); } catch { date = DateTime.Now; }
            return date == DateTime.Now.Date ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
