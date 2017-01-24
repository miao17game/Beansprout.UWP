using MicroMsg.sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Wallace.UWP.Helpers.Tools;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.Web.Http;

namespace Wallace.UWP.Helpers.SDK {
    public static class SDKHelpers {

        public static async Task SendWechatShareToUserChoiceRequestAsync(string url, string title, byte[] thumb, string desc = "", bool toTimeLine = true) {
            try {
                var message = new WXWebpageMessage {
                    WebpageUrl = url,
                    Title = title,
                    Description = desc,
                    ThumbData = thumb
                };
                var requset = new SendMessageToWX.Req(message, toTimeLine ? SendMessageToWX.Req.WXSceneTimeline : SendMessageToWX.Req.WXSceneSession);
                IWXAPI api = WXAPIFactory.CreateWXAPI("123456789");
                var tes = await api.SendReq(requset);
            } catch(WXException e) { System.Diagnostics.Debug.WriteLine(e.StackTrace); }
        }

        public static async Task<byte[]> ReadResFromImageAsync(string url) {
            try {
                return await ImageHelpers.GetThumbImageFromUriToBytes(url);
            } catch {
                var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/douban3_for_wechat108.png"));
                using (var stream = await file.OpenReadAsync()) {
                    var pic = new byte[stream.Size];
                    await stream.AsStream().ReadAsync(pic, 0, pic.Length);
                    return pic;
                }
            }

        }

    }
}
