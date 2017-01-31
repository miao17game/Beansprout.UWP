using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI.Notifications;

namespace Douban.UWP.Core.Tools {
    public static class ToastHelper {
        public static ToastNotification PopToast(string title, string content, string imageUri, string uri) {
            return PopToast(title, content, imageUri, uri, null, null);
        }

        public static ToastNotification PopToast(string title, string content, string imageUri, string uri, string tag, string group) {

            ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText02;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

            var toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode(title));
            toastTextElements[1].AppendChild(toastXml.CreateTextNode(content));

            var toastImageAttributes = toastXml.GetElementsByTagName("image")[0] as XmlElement;
            toastImageAttributes.SetAttribute("src", $"ms-appx:///Assets/douban3_for_wechat108.png");
            toastImageAttributes.SetAttribute("placement", "appLogoOverride");
            toastImageAttributes.SetAttribute("hint-crop", "circle");

            var toastNode = toastXml.SelectSingleNode("/toast") as XmlElement;
            toastNode.SetAttribute("launch", uri);
            toastNode.SetAttribute("duration", "long");

            var audio = toastXml.CreateElement("audio");
            audio.SetAttribute("src", $"ms-winsoundevent:Notification.Mail");
            toastNode.AppendChild(audio);

            var binding = toastNode.SelectSingleNode("visual").SelectSingleNode("binding") as XmlElement;
            binding.SetAttribute("template", "ToastGeneric");
            var image = toastXml.CreateElement("image");
            image.SetAttribute("src", imageUri);
            image.SetAttribute("placement", "inline");
            binding.AppendChild(image);

            return PopCustomToast(toastXml, tag, group);
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

        public static async Task<bool> GetNewsAndPushToastAsync() {
            var now = DateTime.Now;
            if (now.Hour < 10 || now.Hour > 23)
                return false;
            var date = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
            var Host = "https://m.douban.com/rexxar/api/v2/recommend_feed?alt=json&next_date={0}&loc_id=&gender=&birthday=&udid=&for_mobile=true";
            var item = await FetchMessageFromAPIAsync(string.Format(Host, date), 0);
            if (item == null)
                return false;
            var result = PopToast(
                title: item.Title ?? "",
                content: item.Content ?? now.ToString("h:mm tt"),
                imageUri: item.ImageSrc ?? "",
                uri: item.Uri ?? "");
            return result.SuppressPopup;
        }

        private static async Task<ToastItem> FetchMessageFromAPIAsync(string target, int offset = 0) {
            try {
                var result = await DoubanWebProcess.GetMDoubanResponseAsync(target, client: new Windows.Web.Http.HttpClient());
                if (result == null)
                    return null;
                JObject jo = JObject.Parse(result);
                var feeds = jo["recommend_feeds"];
                if (feeds == null || !feeds.HasValues)
                    return null;
                if (feeds.HasValues) {
                    var now = DateTime.Now.Hour - 8;
                    if (now < 2)
                        now = 2;
                    try {
                        var single = feeds.Children().ElementAt(now);
                        var uri = UriDecoder.UriToEncode(single["uri"].Value<string>(), UriDecoder.ToatFromInfosList, single["title"].Value<string>());
                        return new ToastItem {
                            Title = single["title"].Value<string>(),
                            ImageSrc = single["cover_url"].Value<string>() != "" ? single["cover_url"].Value<string>() : null,
                            Uri = uri,
                            Content = single["desc"].Value<string>() != "" ? single["desc"].Value<string>() : null,
                        };
                    } catch { return null; }
                }
            } catch { /* Ignore */ }
            return null;
        }

    }

    class ToastItem {
        public string Title { get; set; }
        public string ImageSrc { get; set; }
        public string Content { get; set; }
        public string Uri { get; set; } 
    }
}
