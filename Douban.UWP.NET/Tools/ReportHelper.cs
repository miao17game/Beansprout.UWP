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
            try {
                await Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                    new ToastSmooth("Fetch Data Error \n" + erroeMessage).Show();
                });
            } catch { System.Diagnostics.Debug.WriteLine("Repoeter error...... >> " + erroeMessage); }
        }

        public static async void ReportAttentionAsync(string erroeMessage) {
            try {
                await Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                    new ToastSmooth(erroeMessage).Show();
                });
            } catch { System.Diagnostics.Debug.WriteLine("Repoeter error...... >> " + erroeMessage); }
        }

        public static async void ReportAttentionDebugAsync(string erroeMessage, string debug_mess) {
            try {
                await Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                    new ToastSmooth(erroeMessage).Show();
                });
            } catch { System.Diagnostics.Debug.WriteLine("Repoeter error...... >>\n " + erroeMessage + "\n=================\n" +debug_mess); }
        }

    }
}
