using Douban.UWP.BackgroundTasks.Helpers;
using Douban.UWP.Core.Tools;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallace.UWP.Helpers;
using Windows.ApplicationModel.Background;

namespace Douban.UWP.BackgroundTasks {
    public sealed class ToastBackgroundPushTask : IBackgroundTask {

        public void Run(IBackgroundTaskInstance taskInstance) {
            var deferral = taskInstance.GetDeferral();
            //await GetNewsAndPushToast();
            deferral.Complete();
        }

        //private static async Task GetNewsAndPushToast() {
        //    if ((bool?)SettingsHelper.ReadSettingsValue(SettingsSelect.IsQuietTime) ?? true)
        //        if ((DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 7) || DateTime.Now.Hour >= 23)
        //            return;
        //    foreach (var item in (
        //        await DataHandler.SetHomeListResources())
        //        .Skip(4)
        //        .Take(2)) {
        //        var resultHtml = await WebProcess.GetHtmlResources(item.Path.ToString());
        //        try {
        //            ToastHelper.PopToast(
        //                item.Title,
        //                GetPageContent(resultHtml.ToString()),
        //                item.ImageSource.ToString(),
        //                item.ID.ToString());
        //            await Task.Delay(1500);
        //        } catch { /* don not need to check. */}
        //    }
        //}

    }
}
