
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Windows.Web.Http;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;

namespace Douban.UWP.Test {
    [TestClass]
    public class UnitTest1 {
        [TestMethod]
        public void TestMethod1() {
        }

        public static async Task<string> GetMDoubanResponseAsync(
            string path,
            string host,
            string reffer,
            bool isMobileDevice = true,
            bool allowToRedirect = true,
            HttpClient client = null,
            Encoding ecd = null) {
            var returnString = default(string);
            try { // do not dispose, so that the global undirect httpclient will stay in referenced. dispose it when you need.
                var httpClient = client ?? new HttpClient();
                using (var request = GET(path)) {
                    request.Headers.Host = new Windows.Networking.HostName(host);
                    if (reffer != null)
                        request.Headers.Referer = new Uri(reffer);
                    request.Headers["User-Agent"] = "Mozilla/5.0 (Windows Phone 10.0; Android 4.2.1; Microsoft; Lumia 950) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2486.0 Mobile Safari/537.36 Edge/14.14263";
                    request.Headers["Connection"] = "Keep-Alive";
                    var result = await httpClient.SendRequestAsync(request);
                    returnString = ecd == null ? (await CastStreamResultToStringAsync(result, false)).ToString() : (await CastStreamResultToStringAsync(result, ecd)).ToString();
                }
            } catch (ObjectDisposedException ex) { // when web connect recovery , recreate a new instance to implemente a recursive function to solve the problem.
                Debug.WriteLine("\nObjectDisposedException -- Failed£º\n" + ex.StackTrace);
                return await GetMDoubanResponseAsync(path, host, reffer, allowToRedirect, client: new HttpClient(), ecd: ecd);
            } catch (COMException ex) { // it is obvious that the internrt connect goes wrong.
                Debug.WriteLine("\nCOMException -- Failed£º\n" + ex.StackTrace);
                return null;
            } catch (Exception ex) { // unkown error, report it.
                Debug.WriteLine("\nException -- Failed£º\n" + ex.StackTrace);
                return null;
            }
            return returnString;
        }

        private async static Task<HttpResponseMessage> LOGIN_POSTAsync(HttpClient client, string urlString) {
            return await client.PostAsync(new Uri(urlString), null);
        }

        private async static Task<HttpResponseMessage> LOGIN_POSTAsync(HttpClient client, string urlString, HttpFormUrlEncodedContent content) {
            return await client.PostAsync(new Uri(urlString), content);
        }

        private static HttpRequestMessage POST(string urlString, bool notForLogin = true) {
            return new HttpRequestMessage(HttpMethod.Post, new Uri(urlString));
        }

        private static HttpRequestMessage GET(string urlString) {
            return new HttpRequestMessage(HttpMethod.Get, new Uri(urlString));
        }

        /// <summary>
        /// In gb2312. 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private static async Task<StringBuilder> CastStreamResultToStringAsync(HttpResponseMessage result, bool IsGB2312 = true) {
            var stream = await (result.Content as HttpStreamContent).ReadAsInputStreamAsync();
            var LrcStringBuider = new StringBuilder();
            var streamReader = new StreamReader(stream.AsStreamForRead(), IsGB2312 ? DBCSEncoding.GetDBCSEncoding("gb2312") : Encoding.UTF8);
            LrcStringBuider.Append(await streamReader.ReadToEndAsync());
            return LrcStringBuider;
        }

        private static async Task<StringBuilder> CastStreamResultToStringAsync(HttpResponseMessage result, Encoding ecd) {
            var stream = await (result.Content as HttpStreamContent).ReadAsInputStreamAsync();
            var LrcStringBuider = new StringBuilder();
            var streamReader = new StreamReader(stream.AsStreamForRead(), ecd);
            LrcStringBuider.Append(await streamReader.ReadToEndAsync());
            return LrcStringBuider;
        }

    }
}
