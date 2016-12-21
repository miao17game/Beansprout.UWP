using DBCSCodePage;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Wallace.UWP.Helpers.Controls;
using Windows.UI.Core;
using Windows.UI.Xaml;

using static Wallace.UWP.Helpers.Tools.UWPStates;

namespace Wallace.UWP.Helpers.Tools {
    public static class WebProcess {
        static CoreDispatcher Dispatcher = Window.Current.Dispatcher;
        /// <summary>
        /// Get the html in string by string-Uri
        /// </summary>
        /// <param name="urlString">Target web uri-string</param>
        /// <param name="ifNeedGB2312">GB2312 or UTF-8</param>
        /// <returns></returns>
        public static async Task<string> GetHtmlResources ( string urlString , bool ifNeedGB2312 = false ) {
            var LrcStringBuider = new StringBuilder ( );
            try {
                var request = WebRequest.Create(urlString) as HttpWebRequest;
                request.Headers[HttpRequestHeader.Referer] = "https://www.douban.com/";
                request.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.79 Safari/537.36 Edge/14.14393";
                request.Headers[HttpRequestHeader.Host] = "www.douban.com";
                request.Headers[HttpRequestHeader.Connection] = "Keep-Alive";
                request.Method = "GET";
                try {
                    using (var response = await request.GetResponseAsync() as HttpWebResponse) {
                        var stream = response.GetResponseStream();
                        var streamReader = new StreamReader(stream, ifNeedGB2312 ? DBCSEncoding.GetDBCSEncoding("gb2312") : Encoding.UTF8);
                        LrcStringBuider.Append(await streamReader.ReadToEndAsync());
                    }
                } catch (WebException ex) {
                    Debug.WriteLine("\nTimeOut：\n" + ex.Message);
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { new ToastSmoothBase(GetUIString("TimeOutError")).Show(); });
                    return null;
                } catch (Exception e) {
                    Debug.WriteLine("\nTimeOut：\n" + e.Message);
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { new ToastSmoothBase(GetUIString("TimeOutError")).Show(); });
                    return null;
                } request.Abort();
            } catch {
                Debug.WriteLine("\nTimeOut：\n" );
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { new ToastSmoothBase(GetUIString("TimeOutError")).Show(); });
                return null;
            }
            return LrcStringBuider.ToString();
        }

    }
}
