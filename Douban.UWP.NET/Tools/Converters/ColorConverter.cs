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
    public class ColorConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            return ToColorSolidBrush(value as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }

        private Brush ToColorSolidBrush(string title) {
            SolidColorBrush result = new SolidColorBrush();
            result.Color = 
                title == GetUIString("DB_INDEX") ? Color.FromArgb(255, 75, 21, 173) :
                title == GetUIString("DB_BOOK") ? Color.FromArgb(255, 217, 6, 94) :
                title == GetUIString("DB_MOVIE") ? Color.FromArgb(255, 60, 188, 98) :
                title == GetUIString("DB_MUSIC") ? Color.FromArgb(255, 97, 17, 171) :
                title == GetUIString("DB_LOCATION") ? Color.FromArgb(255, 254, 183, 8) :
                title == GetUIString("DB_GROUP") ? Color.FromArgb(255, 69, 90, 172) :
                title == GetUIString("DB_READ") ? Color.FromArgb(255, 141, 4, 33) :
                title == GetUIString("DB_FM") ? Color.FromArgb(255, 244, 78, 97) :
                title == GetUIString("DB_DONGXI") ? Color.FromArgb(255, 255, 193, 63) :
                title == GetUIString("DB_MARKET") ? Color.FromArgb(255, 49, 199, 155) :
                title == GetUIString("DB_MORE") ? Color.FromArgb(255, 255, 63, 138) :
                Color.FromArgb(255, 82, 82, 82);
            return result;
        }
    }
}
