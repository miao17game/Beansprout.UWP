using Douban.UWP.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallace.UWP.Helpers;
using Windows.ApplicationModel.Background;

namespace Douban.UWP.BackgroundTasks {
    public sealed class ServicingComplete : IBackgroundTask {

        public async void Run(IBackgroundTaskInstance taskInstance) {
            var deferral = taskInstance.GetDeferral();

            var succeed = await TaskHelper.IfAccessNotDeniedAsync();
            if (!succeed)
                return;

            TaskHelpers.FindTask(TaskConstants.LiveTitle)?.Unregister(true);
            TaskHelpers.FindTask(TaskConstants.ToastBackground)?.Unregister(true);

            TaskHelpers.RegisterLiveTitleTask();
            TaskHelpers.RegisterToastBackgroundTask();

            deferral.Complete();
        }

    }
}
