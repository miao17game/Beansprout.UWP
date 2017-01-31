using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallace.UWP.Helpers;
using Windows.ApplicationModel.Background;

namespace Douban.UWP.BackgroundTasks {
    public static class TaskHelpers {

        public static BackgroundTaskRegistration FindTask(string taskName) {
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
                if (cur.Value.Name == taskName)
                    return (BackgroundTaskRegistration)(cur.Value);
            return null;
        }

        public static void RegisterLiveTitleTask() {
            var taskBuilder = new BackgroundTaskBuilder {
                Name = TaskConstants.LiveTitle,
                TaskEntryPoint = typeof(TitleBackgroundUpdateTask).FullName
            };
            taskBuilder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));
            taskBuilder.SetTrigger(new TimeTrigger(360, true));
            var register = taskBuilder.Register();
        }

        public static void RegisterToastBackgroundTask() {
            var taskBuilder = new BackgroundTaskBuilder {
                Name = TaskConstants.ToastBackground,
                TaskEntryPoint = typeof(ToastBackgroundPushTask).FullName
            };
            taskBuilder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));
            taskBuilder.SetTrigger(new TimeTrigger(180, false));
            var register = taskBuilder.Register();
        }

        public static void RegisterServiceCompleteTask() {
            var taskBuilder = new BackgroundTaskBuilder {
                Name = TaskConstants.ServiceComplete,
                TaskEntryPoint = typeof(ServicingComplete).FullName
            };
            taskBuilder.SetTrigger(new SystemTrigger(SystemTriggerType.ServicingComplete, false));
            var register = taskBuilder.Register();
        }

    }
}
