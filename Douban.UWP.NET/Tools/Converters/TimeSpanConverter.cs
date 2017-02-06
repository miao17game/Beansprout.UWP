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
            return ToTimeSpan(value);
        }

        private double ToValueCode(int num) {
            return System.Convert.ToDouble(num);
        }

        private int ToDouble(object value) {
            var num = default(TimeSpan);
            try { num = (TimeSpan)value; } catch { }
            return (int)num.TotalSeconds;
        }

        private TimeSpan ToTimeSpan(object value) {
            var num = default(TimeSpan);
            try { num = TimeSpan.FromSeconds(System.Convert.ToDouble(value)); } catch { }
            return num;
        }

    }
}
