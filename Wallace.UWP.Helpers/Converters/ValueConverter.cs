using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Wallace.UWP.Helpers.Converters {
    public class ValueConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            return ToValueCode(System.Convert.ToDouble(value), System.Convert.ToUInt32(parameter));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }

        private double ToValueCode(double num, uint seed) {
            return num / seed;
        }
    }
}
