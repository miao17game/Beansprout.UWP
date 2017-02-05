using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Douban.UWP.Tools.Converters {
    public class TimeSpanConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            return ToValueCode(ToDouble(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }

        private double ToValueCode(double num) {
            return (double)(((int)(num * 10000)) / 1000) / 10;
        }

        private double ToDouble(object value) {
            var num = default(TimeSpan);
            try { num = (TimeSpan)value; } catch { }
            return num.TotalSeconds;
        }

    }
}
