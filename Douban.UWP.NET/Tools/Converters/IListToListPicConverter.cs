using Douban.UWP.Core.Models.LifeStreamModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Douban.UWP.Tools.Converters {
    public class IListToListPicConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            return ConvertToVisibility((IList<PictureItem>)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }

        private List<PictureItem> ConvertToVisibility(IList<PictureItem> value) { return value as List<PictureItem> == null ? new List<PictureItem>() : (List<PictureItem>)value; }
    }
}
