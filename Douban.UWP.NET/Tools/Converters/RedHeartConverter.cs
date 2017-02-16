using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

using static Wallace.UWP.Helpers.Tools.UWPStates;

namespace Douban.UWP.NET.Tools.Converters {
    public class RedHeartConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            return ToColorSolidBrush((int)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }

        private Brush ToColorSolidBrush(int title) {
            return title == 0 ? 
                new SolidColorBrush(Color.FromArgb(128, 128, 128, 128)) : 
                new SolidColorBrush(Colors.Red);
        }
    }
}
