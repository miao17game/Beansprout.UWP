using static Wallace.UWP.Helpers.Tools.UWPStates;
using static Douban.UWP.NET.Resources.AppResources;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallace.UWP.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Douban.UWP.Tools.Converters {
    public class DateTimeConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            return ConvertToUInt(value as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }

        private string ConvertToUInt(string value) {
            if (value == null)
                return null;
            var timeSpan = default(TimeSpan);
            try { timeSpan = DateTime.Now - DateTime.Parse(value); } catch { timeSpan = new TimeSpan(0); }
            return timeSpan.TotalSeconds < 60 ? timeSpan.Seconds + " " + GetUIString("SecondsAgo") :
                timeSpan.TotalSeconds < 3600 ? timeSpan.Minutes + " " + GetUIString("MinutesAgo") :
                timeSpan.TotalSeconds < 86400 ? timeSpan.Hours + " " + GetUIString("HoursAgo") :
                timeSpan.TotalDays < 7 ? timeSpan.Days + " " + GetUIString("DaysAgo") :
                value;
        }
    }
}
