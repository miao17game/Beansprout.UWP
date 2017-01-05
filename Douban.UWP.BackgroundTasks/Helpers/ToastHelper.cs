using System.Text;
using Windows.Data.Xml.Dom;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI.Notifications;

namespace Douban.UWP.BackgroundTasks.Helpers {
    public static class ToastHelper {
        public static ToastNotification PopToast(string title, string content, string imageUri, string pathUri) {
            return PopToast(title, content, imageUri, pathUri, null, null);
        }

        public static ToastNotification PopToast(string title, string content, string imageUri, string pathUri, string tag, string group) {
            string xml = $@"
                                        <toast activationType='foreground' launch='{pathUri}'>
                                            <visual>
                                                <binding template='ToastGeneric'>
                                                    <text>{title}</text>
                                                    <text>{content}</text>
                                                    <image placement='inline' src='{imageUri}'/>
                                                </binding>
                                            </visual>
                                         </toast>";
            
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return PopCustomToast(doc, tag, group);
        }

        public static ToastNotification PopCustomToast(string xml) {
            return PopCustomToast(xml, null, null);
        }

        public static ToastNotification PopCustomToast(string xml, string tag, string group) {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return PopCustomToast(doc, tag, group);
        }

        [DefaultOverload]
        public static ToastNotification PopCustomToast(XmlDocument doc, string tag, string group) {
            var toast = new ToastNotification(doc);
            if (tag != null)
                toast.Tag = tag;
            if (group != null)
                toast.Group = group;
            ToastNotificationManager.CreateToastNotifier().Show(toast);
            return toast;
        }

    }
}
