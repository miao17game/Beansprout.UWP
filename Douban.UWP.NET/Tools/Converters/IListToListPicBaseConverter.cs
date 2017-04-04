using Douban.UWP.Core.Models.LifeStreamModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Douban.UWP.Tools.Converters {
    public class IListToListPicBaseConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            return ConvertToVisibility((IList<PictureItemBase>)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }

        private List<PictureItemBase> ConvertToVisibility(IList<PictureItemBase> value) {
            return value as List<PictureItemBase> ?? new List<PictureItemBase>();
        }
    }
}
