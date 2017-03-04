using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Douban.UWP.Tools.Converters {
    public class ValueConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            return ToValueCode(System.Convert.ToDouble(value), System.Convert.ToDouble(parameter));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }

        private double ToValueCode(double num, double seed) {
            return num / seed;
        }
    }
}
