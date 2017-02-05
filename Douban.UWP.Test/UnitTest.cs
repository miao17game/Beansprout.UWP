
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Windows.Web.Http;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using DBCSCodePage;
using System.Collections.Generic;

namespace Douban.UWP.Test {
    [TestClass]
    public class UnitTest1 {

        const string apiKey = "02f7751a55066bcb08e65f4eff134361";
        const string dId = "d0166bef2e066290987be5eb123cd2a0080fb654";
        const string sdk_version = "1.0.14";
        const string uid = "155845973";
        const string bearer = "2af5e2101f36826180e1a6305da38723";

        [TestMethod]
        public void MainMethod() {
            Method02Async();
        }

        [TestMethod]
        public async void TestMethod1Async() {
            long minised = (long)((DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
            var result = await GetMDoubanResponseAsync(
                $"{"https://"}amonsul.douban.com/check2?apikey={apiKey}&app_name=Radio_Android&ltime={minised}&did={dId}&sdkVersion={sdk_version}",
                "amonsul.douban.com",
                null,
                false);
            Debug.WriteLine(result);
            TestPostAsync();
        }

        public async void TestPostAsync() {
            long minised = (long)((DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
            var result = await PostDoubanResponseAsync(
                        path: "https://amonsul.douban.com",
                        host: "amonsul.douban.com",
                        reffer: null,
                        userAgent: $"com.douban.amonsul/android {sdk_version}",
                        content:
                        new HttpFormUrlEncodedContent(new List<KeyValuePair<string, string>> {
                            new KeyValuePair<string, string>("apikey", apiKey),
                            new KeyValuePair<string, string>("app_name","Radio_Android"),
                            new KeyValuePair<string, string>("ltime", minised.ToString()),
                            new KeyValuePair<string, string>("did", dId),
                            new KeyValuePair<string, string>("userid", uid),
                        }),
                        isMobileDevice: true);
            Debug.WriteLine(result);
        }

        public async void Method02Async() {
            var result = await GetMDoubanResponseAsync(
                $"{"https://"}api.douban.com/v2/fm/song/{"2671110g5685"}/?version=644&push_device_id={dId}&start=0&app_name=radio_android&apikey={apiKey}",
                "api.douban.com",
                null,
                false,
                userAgt: @"api-client/2.0 com.douban.radio/4.6.4(464) Android/18 TCL_P306C TCL TCL-306C");
            Debug.WriteLine(result);
        }

        public async void Method03Async() {
            var result = await PostDoubanResponseAsync(
                $"{"https://"}api.douban.com/v2/fm/songlist/{14913862}/detail",
                "api.douban.com",
                null,
                userAgent: @"api-client/2.0 com.douban.radio/4.6.4(464) Android/18 TCL_P306C TCL TCL-306C",
                content: new HttpFormUrlEncodedContent(new List<KeyValuePair<string, string>>{
                    new KeyValuePair<string, string>( "version", "644" ),
                    new KeyValuePair<string, string>( "kbps", "64" ),
                    new KeyValuePair<string, string>( "app_name", "radio_android" ),
                    new KeyValuePair<string, string>( "apikey", apiKey ),
                }));
            Debug.WriteLine(result);
        }

        public static async Task<string> GetMDoubanResponseAsync(
            string path,
            string host,
            string reffer,
            bool isMobileDevice = true,
            bool allowToRedirect = true,
            string userAgt = null,
            HttpClient client = null,
            Encoding ecd = null) {
            var returnString = default(string);
            try { 
                var httpClient = client ?? new HttpClient();
                using (var request = GET(path)) {
                    request.Headers.Host = new Windows.Networking.HostName(host);
                    if (reffer != null)
                        request.Headers.Referer = new Uri(reffer);
                    if (userAgt != null) {
                        request.Headers["User-Agent"] = userAgt;
                    } else {
                        request.Headers["User-Agent"] =
                            isMobileDevice ?
                            "Mozilla/5.0 (Windows Phone 10.0; Android 4.2.1; Microsoft; Lumia 950) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2486.0 Mobile Safari/537.36 Edge/14.14263" :
                            $"com.douban.amonsul/android {sdk_version}";
                    }
                    request.Headers["Connection"] = "Keep-Alive";
                    request.Headers["Authorization"] = $"Bearer {bearer}";
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

        public static async Task<string> PostDoubanResponseAsync(
            string path,
            string host,
            string reffer,
            string userAgent,
            HttpFormUrlEncodedContent content,
            bool isMobileDevice = false,
            Encoding ecd = null) {
            var returnString = default(string);
            try {
                using (var httpClient = new HttpClient()) {
                    httpClient.DefaultRequestHeaders.Host = new Windows.Networking.HostName(host);
                    if (reffer != null)
                        httpClient.DefaultRequestHeaders.Referer = new Uri(reffer);
                    if (userAgent != null)
                        httpClient.DefaultRequestHeaders["User-Agent"] = userAgent;
                    using (var request = await LOGIN_POSTAsync(httpClient, path, content)) {
                        returnString = ecd == null ? (await CastStreamResultToStringAsync(request, false)).ToString() : (await CastStreamResultToStringAsync(request, ecd)).ToString();
                    }
                }
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
