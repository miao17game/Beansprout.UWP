using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Wallace.UWP.Helpers;
using Windows.Data.Xml.Dom;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI.Notifications;

namespace Douban.UWP.Core.Tools {
    public static class ToastHelper {

        const string DefaultVoice = @"ms-appx:///Voice/yiner.mp3";

        public static XmlDocument CreateToastNotificaion(string title, string content, string imageUri, string uri, string logoOverride, string voice = DefaultVoice) {
            ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText02;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

            var toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode(title));
            toastTextElements[1].AppendChild(toastXml.CreateTextNode(content));

            var toastImageAttributes = toastXml.GetElementsByTagName("image")[0] as XmlElement;
            toastImageAttributes.SetAttribute("src", logoOverride ?? @"ms-appx:///Assets/star004.png");
            toastImageAttributes.SetAttribute("placement", "appLogoOverride");
            toastImageAttributes.SetAttribute("hint-crop", "circle");

            var toastNode = toastXml.SelectSingleNode("/toast") as XmlElement;
            toastNode.SetAttribute("launch", uri);
            toastNode.SetAttribute("duration", "long");

            var audio = toastXml.CreateElement("audio");
            audio.SetAttribute("src", voice);
            toastNode.AppendChild(audio);

            var binding = toastNode.SelectSingleNode("visual").SelectSingleNode("binding") as XmlElement;
            binding.SetAttribute("template", "ToastGeneric");
            var image = toastXml.CreateElement("image");
            image.SetAttribute("src", imageUri);
            image.SetAttribute("placement", "inline");
            binding.AppendChild(image);
            return toastXml;
        }

        public static ToastNotification PopToast(string title, string content, string imageUri, string uri, string logoOverride = null, string voice = DefaultVoice) {
            return PopToast(title, content, imageUri, uri, logoOverride, voice, null, null);
        }

        public static ToastNotification PopToast(string title, string content, string imageUri, string uri, string logoOverride, string voice, string tag, string group) {
            XmlDocument toastXml = CreateToastNotificaion(title, content, imageUri, uri, logoOverride, voice);
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
            var support = (bool?)SettingsHelper.ReadSettingsValue(SettingsSelect.IsToastEnable) ?? true;
            if (!support)
                return false;
            var nowHour = DateTime.Now.Hour;
            if (forbiddenTimeHoursOfToast.Contains(nowHour))
                return false;
            var date = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
            var Host = "https://m.douban.com/rexxar/api/v2/recommend_feed?alt=json&next_date={0}&loc_id=&gender=&birthday=&udid=&for_mobile=true";
            var item = await FetchMessageFromAPIAsync(string.Format(Host, date), 0);
            if (item == null)
                return false;
            var result = PopToast(
                title: item.Title ?? "",
                content: item.Content ?? DateTime.Now.ToString("h:mm tt"),
                imageUri: item.ImageSrc ?? "",
                uri: item.Uri ?? "",
                logoOverride: item.LogoOverride,
                voice: SettingsHelper.ReadSettingsValue(SettingsSelect.ToastVoice) as string ?? DefaultVoice);
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
                    var now = DateTime.Now.Hour - 4;
                    if (now < 3)
                        now = 3;
                    try {
                        var single = feeds.Children().ElementAt(now);
                        var author = single["target"]["author"];
                        var uri = UriDecoder.CreateJson(new ToastParameters {
                            Title = single["title"].Value<string>(),
                            Uri = UriDecoder.GetUrlFromUri(single["target"]["uri"].Value<string>()),
                            Type = EncodeFormat.ToatFromInfosList
                        });
                        return new ToastItem {
                            Title = single["title"].Value<string>(),
                            ImageSrc = single["target"]["cover_url"].Value<string>() != "" ? single["target"]["cover_url"].Value<string>() : null,
                            LogoOverride = author.HasValues ? author["avatar"].Value<string>() != "" ? author["avatar"].Value<string>() : null : null,
                            Content = single["target"]["desc"].Value<string>() != "" ? single["target"]["desc"].Value<string>() : null,
                            Uri = uri,
                        };
                    } catch { return null; }
                }
            } catch { /* Ignore */ }
            return null;
        }

        static int[] forbiddenTimeHoursOfToast = new int[] { 0, 1, 2, 3, 4, 5, 6, 8, 9, 11, 12, 14, 15, 17, 18, 20, 21, 23, 24 };

    }

    class ToastItem {
        public string Title { get; set; }
        public string ImageSrc { get; set; }
        public string Content { get; set; }
        public string Uri { get; set; } 
        public string LogoOverride { get; set; }
    }

    public enum EncodeFormat {
        ToastFromDefault,
        ToatFromInfosList,
    }

    [DataContract]
    public class ToastParameters {
        [DataMember]
        public EncodeFormat Type { get; set; }
        [DataMember]
        public string Uri { get; set; }
        [DataMember]
        public string Title { get; set; }
    }

}
