using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

using static Wallace.UWP.Helpers.Tools.UWPStates;

namespace Douban.UWP.NET.Tools.Converters {
    public class IconConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            return ToIconCode(value as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }

        private string ToIconCode(string title) {
            return 
                title == GetUIString("DB_INDEX") ? char.ConvertFromUtf32(0xEC0A) :
                title == GetUIString("DB_BOOK") ? char.ConvertFromUtf32(0xE1D3) :
                title == GetUIString("DB_MOVIE") ? char.ConvertFromUtf32(0xEC80) :
                title == GetUIString("DB_MUSIC") ? char.ConvertFromUtf32(0xE93C) :
                title == GetUIString("DB_LOCATION") ? char.ConvertFromUtf32(0xEC43) :
                title == GetUIString("DB_GROUP") ? char.ConvertFromUtf32(0xE125) :
                title == GetUIString("DB_READ") ? char.ConvertFromUtf32(0xECA7) :
                title == GetUIString("DB_FM") ? char.ConvertFromUtf32(0xE95A) :
                title == GetUIString("DB_DONGXI") ? char.ConvertFromUtf32(0xED58) :
                title == GetUIString("DB_MARKET") ? char.ConvertFromUtf32(0xE719) :
                title == GetUIString("DB_MORE") ? char.ConvertFromUtf32(0xE71B) :
                char.ConvertFromUtf32(0xE1F6);
        }
    }
}
