using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Wallace.UWP.Helpers.Converters {
    /// <summary>
    /// 定义一个值转换器，用于将绑定的数据格式化为指定的格式
    /// </summary>
    public class VisibilityConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            return ConvertToVisibility(value as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }

        private Visibility ConvertToVisibility(string value) { return string.IsNullOrEmpty(value) ? Visibility.Collapsed : Visibility.Visible;}
    }
}
