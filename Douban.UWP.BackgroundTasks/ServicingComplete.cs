using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace Douban.UWP.BackgroundTasks {
    public sealed class ServicingComplete : IBackgroundTask {

        private const string liveTitleTask = "LIVE_TITLE_TASK";
        private const string ToastBackgroundTask = "TOAST_BACKGROUND_TASK";
        private const string ServiceCompleteTask = "SERVICE_COMPLETE_TASK";

        public async void Run(IBackgroundTaskInstance taskInstance) {
            var deferral = taskInstance.GetDeferral();

            var status = await BackgroundExecutionManager.RequestAccessAsync();
            if (status == BackgroundAccessStatus.Unspecified || status == BackgroundAccessStatus.DeniedBySystemPolicy || status == BackgroundAccessStatus.DeniedByUser) { return; }

            var task = FindTask(liveTitleTask);
            if (task != null)
                task.Unregister(true);

            var task2 = FindTask(ToastBackgroundTask);
            if (task2 != null)
                task2.Unregister(true);

            this.RegisterLiveTitleTask();
            //this.RegisterToastBackgroundTask();

            deferral.Complete();
        }

        // 
        // Check for a registration of the named background task. If one
        // exists, return it.
        // 
        public static BackgroundTaskRegistration FindTask(string taskName) {
            foreach (var cur in BackgroundTaskRegistration.AllTasks) 
                if (cur.Value.Name == taskName) 
                    return (BackgroundTaskRegistration)(cur.Value);
            return null;
        }

        private void RegisterLiveTitleTask() {
            
            foreach (var item in BackgroundTaskRegistration.AllTasks)
                if (item.Value.Name == liveTitleTask)
                    item.Value.Unregister(true);

            var taskBuilder = new BackgroundTaskBuilder {
                Name = liveTitleTask,
                TaskEntryPoint = typeof(TitleBackgroundUpdateTask).FullName
            };

            taskBuilder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));

            taskBuilder.SetTrigger(new TimeTrigger(60, false));
            var register = taskBuilder.Register();
        }

        //private void RegisterToastBackgroundTask() {
        //    var taskBuilder = new BackgroundTaskBuilder {
        //        Name = ToastBackgroundTask,
        //        TaskEntryPoint = typeof(ToastBackgroundPushTask).FullName
        //    };
        //    taskBuilder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));
        //    taskBuilder.SetTrigger(new TimeTrigger(240, false));
        //    var register = taskBuilder.Register();
        //}

    }
}
