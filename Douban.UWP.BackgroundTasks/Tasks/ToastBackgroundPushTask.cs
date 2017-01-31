using Douban.UWP.Core.Tools;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Wallace.UWP.Helpers;
using Windows.ApplicationModel.Background;
using Windows.Foundation;

namespace Douban.UWP.BackgroundTasks {
    public sealed class ToastBackgroundPushTask : IBackgroundTask {

        public async void Run(IBackgroundTaskInstance taskInstance) {
            var deferral = taskInstance.GetDeferral();
            await GetNewsAndPushToast();
            deferral.Complete();
        }

        private IAsyncOperation<bool> GetNewsAndPushToast() {
            try {
                return AsyncInfo.Run(token => ToastHelper.GetNewsAndPushToastAsync());
            } catch (Exception) { /* ignored */ }
            return null;
        }

    }
}
