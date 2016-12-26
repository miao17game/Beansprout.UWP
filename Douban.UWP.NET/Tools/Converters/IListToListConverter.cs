using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Douban.UWP.Tools.Converters {
    public class IListToListConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            return ConvertToVisibility((IList<Uri>)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }

        private List<Uri> ConvertToVisibility(IList<Uri> value) { return value as List<Uri> == null ? new List<Uri>() : (List<Uri>)value; }
    }
}
