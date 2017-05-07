using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Douban.UWP.Tools.Converters {
    public class IListToListConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            return ConvertToVisibility((IList<string>)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }

        private List<Uri> ConvertToVisibility(IList<string> value) {
            if (value == null)
                return null;
            var newList = new List<Uri>();
            value.ToList().ForEach(i => newList.Add(new Uri(i != null && i != "" ? i : NoPictureUrl)));
            return newList;
        }

        string NoPictureUrl = "https://www.none-wallace-767fc6vh7653df0jb.com/no_wallace_085sgdfg7447fddds65.jpg";
    }
}
