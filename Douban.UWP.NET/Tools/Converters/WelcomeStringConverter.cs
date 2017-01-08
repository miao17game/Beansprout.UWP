using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallace.UWP.Helpers;
using Wallace.UWP.Helpers.Tools;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Douban.UWP.Tools.Converters {
    public class WelcomeStringConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            return value == null ? null : ConvertToString(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }

        private string ConvertToString(string value) {
            var nowHour = DateTime.Now.Hour;
            return nowHour > 3 && nowHour <= 11 ? UWPStates.GetUIString("GoodMorning") :
                nowHour > 11 && nowHour <= 14 ? UWPStates.GetUIString("GoodNoon") :
                nowHour > 14 && nowHour <= 18 ? UWPStates.GetUIString("GoodAfternoon") :
                UWPStates.GetUIString("GoodNight");
        }
    }
}
