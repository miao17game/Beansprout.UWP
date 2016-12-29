using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Douban.UWP.Tools.Converters {
    public class StarSizeColorConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            return ToValueCode(System.Convert.ToUInt32(parameter), System.Convert.ToDouble(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }

        private Brush ToValueCode(uint num, double value) {
            return
                value/10 - 0.2<= 0 ? num <= 1 ? new SolidColorBrush(Color.FromArgb(255, 254, 183, 8)) : new SolidColorBrush(Color.FromArgb(255, 200, 200, 200)) :
                value / 10 - 0.4 <= 0  ? num <= 2 ? new SolidColorBrush(Color.FromArgb(255, 254, 183, 8)) : new SolidColorBrush(Color.FromArgb(255, 200, 200, 200)) :
                value / 10 - 0.6 <= 0 ? num <= 3 ? new SolidColorBrush(Color.FromArgb(255, 254, 183, 8)) : new SolidColorBrush(Color.FromArgb(255, 200, 200, 200)) :
                value / 10 - 0.8 <= 0 ? num <= 4 ? new SolidColorBrush(Color.FromArgb(255, 254, 183, 8)) : new SolidColorBrush(Color.FromArgb(255, 200, 200, 200)) :
                num <= 5 ? new SolidColorBrush(Color.FromArgb(255, 254, 183, 8)) : new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
        }
    }
}
