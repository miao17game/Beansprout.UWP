using Douban.UWP.NET.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Douban.UWP.NET.Tools {
    public class ReportHelper {

        public static async void ReportError(string erroeMessage) {
            await Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                new ToastSmooth("Fetch Data Error \n" + erroeMessage).Show();
            });
        }

        public static async void ReportAttention(string erroeMessage) {
            await Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                new ToastSmooth(erroeMessage).Show();
            });
        }

    }
}
