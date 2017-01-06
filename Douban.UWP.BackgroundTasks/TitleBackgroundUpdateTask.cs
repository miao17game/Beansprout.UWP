using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using System.Diagnostics;
using Windows.UI.StartScreen;
using Douban.Core.NET.Tools;
using Douban.UWP.Core.Tools;
using Newtonsoft.Json.Linq;

namespace Douban.UWP.BackgroundTasks {
    public sealed class TitleBackgroundUpdateTask : IBackgroundTask {

        public async void Run(IBackgroundTaskInstance taskInstance) {
            var deferral = taskInstance.GetDeferral();
            await GetLatestNews();
            deferral.Complete();
        }

        private IAsyncOperation<string> GetLatestNews() {
            try {
                return AsyncInfo.Run(token => TilesHelper.GetNewsAsync());
            } catch (Exception) { /* ignored */ }
            return null;
        }

    }
}
