using DBCSCodePage;
using Douban.UWP.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace Douban.UWP.Core.Tools {
    public static class DoubanWebProcess {

        #region Properties and fields

        static CoreDispatcher Dispatcher = Window.Current.Dispatcher;

        #endregion

        #region Client and Tools

        /// <summary>
        /// UnRedirect HttpClient Cookie Manager, refresh when UnRedirectHttpClient is called.
        /// </summary>
        public static HttpCookieManager UnRedirectCookiesManager;

        public static HttpBaseProtocolFilter UnRedirectHttpFilter;

        private static HttpClient unRedirectHttpClient;
        /// <summary>
        /// Global HttpClient Without Auto-Redirect, And will create a new one when it is called again.
        /// </summary>
        public static HttpClient UnRedirectHttpClient {
            get {
                return unRedirectHttpClient ?? new HttpClient(new Func<HttpBaseProtocolFilter>(() => {
                    if (UnRedirectHttpFilter != null)
                        return UnRedirectHttpFilter;
                    UnRedirectHttpFilter = new HttpBaseProtocolFilter { AllowAutoRedirect = false };
                    UnRedirectCookiesManager = UnRedirectHttpFilter.CookieManager;
                    return UnRedirectHttpFilter;
                }).Invoke());
            }
        }

        public static HttpCookieManager RedirectCookiesManager;

        public static HttpBaseProtocolFilter RedirectHttpFilter;

        private static HttpClient RedirectHttpClient = new HttpClient(new Func<HttpBaseProtocolFilter>(() => {
            if (RedirectHttpFilter != null)
                return RedirectHttpFilter;
            RedirectHttpFilter = new HttpBaseProtocolFilter { AllowAutoRedirect = true };
            RedirectCookiesManager = RedirectHttpFilter.CookieManager;
            return RedirectHttpFilter;
        }).Invoke());

        public static void RefreshHttpClient() { unRedirectHttpClient = null; }

        #endregion

        #region Exception

        public class AccessUnPassedException : Exception {
            private string message;
            public AccessUnPassedException(string message) { this.message = message; }
            public override string Message { get { return message; } }
            public override string StackTrace { get { return message; } }
        }

        #endregion

        #region Methods

        private async static Task<HttpResponseMessage> LOGIN_POSTAsync(HttpClient client, string urlString) {
            return await client.PostAsync(new Uri(urlString), null);
        }

        private async static Task<HttpResponseMessage> LOGIN_POSTAsync(HttpClient client, string urlString, HttpFormUrlEncodedContent content) {
            return await client.PostAsync(new Uri(urlString), content);
        }

        private static HttpRequestMessage POST(string urlString , bool notForLogin = true) {
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
        private static async Task<StringBuilder> CastStreamResultToStringAsync(HttpResponseMessage result, bool IsGB2312 = true ) {
            var stream = await (result.Content as HttpStreamContent).ReadAsInputStreamAsync();
            var LrcStringBuider = new StringBuilder();
            var streamReader = new StreamReader(stream.AsStreamForRead(), IsGB2312? DBCSEncoding.GetDBCSEncoding("gb2312"): Encoding.UTF8);
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

        #endregion

        #region Douban Methods

        /// <summary>
        /// command from left tab.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task<string> GetDoubanResponseAsync(string path, bool allowToRedirect = true, HttpClient client = null, Encoding ecd = null) {
            var returnString = default(string);
            try { // do not dispose, so that the global undirect httpclient will stay in referenced. dispose it when you need.
                var httpClient = client ?? (allowToRedirect ? RedirectHttpClient : UnRedirectHttpClient);
                using (var request = GET(path)) {
                    request.Headers.Host = new Windows.Networking.HostName("www.douban.com");
                    request.Headers.Referer = new Uri("https://www.douban.com/");
                    request.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.79 Safari/537.36 Edge/14.14393";
                    request.Headers["Connection"] = "Keep-Alive";
                    var result = await httpClient.SendRequestAsync(request);
                    if (allowToRedirect) {
                        returnString = ecd == null? (await CastStreamResultToStringAsync(result, false)).ToString() : (await CastStreamResultToStringAsync(result, ecd)).ToString();
                    } else {
                        returnString = ecd == null ? await GetDoubanResponseAsync(result.Headers["Location"]) : await GetDoubanResponseAsync(result.Headers["Location"], ecd: ecd);
                    }
                }
            } catch (ObjectDisposedException ex) { // when web connect recovery , recreate a new instance to implemente a recursive function to solve the problem.
                Debug.WriteLine("\nObjectDisposedException -- Failed：\n" + ex.StackTrace);
                return await GetDoubanResponseAsync(path, allowToRedirect, client: new HttpClient(), ecd: ecd);
            } catch (COMException ex) { // it is obvious that the internrt connect goes wrong.
                Debug.WriteLine("\nCOMException -- Failed：\n" + ex.StackTrace);
                return null;
            } catch (Exception ex) { // unkown error, report it.
                Debug.WriteLine("\nException -- Failed：\n" + ex.StackTrace);
                return null;
            }
            return returnString;
        }

        public static async Task<string> GetMDoubanResponseAsync(string path, bool isMobileDevice = true, bool allowToRedirect = true, HttpClient client = null, Encoding ecd = null) {
            return await GetMDoubanResponseAsync(path, "m.douban.com", "https://m.douban.com/", allowToRedirect, client, ecd);
        }

        public static async Task<string> GetMDoubanResponseAsync(
            string path,
            string host ,
            string reffer ,
            bool allowToRedirect = true, 
            HttpClient client = null, 
            Encoding ecd = null) {
            var returnString = default(string);
            try { // do not dispose, so that the global undirect httpclient will stay in referenced. dispose it when you need.
                var httpClient = client ?? (allowToRedirect ? RedirectHttpClient : UnRedirectHttpClient);
                using (var request = GET(path)) {
                    request.Headers.Host = new Windows.Networking.HostName(host);
                    if (reffer != null)
                        request.Headers.Referer = new Uri(reffer);
                    request.Headers["User-Agent"] = "Mozilla/5.0 (Windows Phone 10.0; Android 4.2.1; Microsoft; Lumia 950) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2486.0 Mobile Safari/537.36 Edge/14.14263";
                    request.Headers["Connection"] = "Keep-Alive";
                    var result = await httpClient.SendRequestAsync(request);
                    if (allowToRedirect) {
                        returnString = ecd == null ? (await CastStreamResultToStringAsync(result, false)).ToString() : (await CastStreamResultToStringAsync(result, ecd)).ToString();
                    } else {
                        returnString = ecd == null ?
                            await GetMDoubanResponseAsync(result.Headers["Location"], host, reffer, allowToRedirect) :
                            await GetMDoubanResponseAsync(result.Headers["Location"], host, reffer, allowToRedirect, ecd: ecd);
                    }
                }
            } catch (ObjectDisposedException ex) { // when web connect recovery , recreate a new instance to implemente a recursive function to solve the problem.
                Debug.WriteLine("\nObjectDisposedException -- Failed：\n" + ex.StackTrace);
                return await GetMDoubanResponseAsync(path, host, reffer, allowToRedirect, client: new HttpClient(), ecd: ecd);
            } catch (COMException ex) { // it is obvious that the internrt connect goes wrong.
                Debug.WriteLine("\nCOMException -- Failed：\n" + ex.StackTrace);
                return null;
            } catch (Exception ex) { // unkown error, report it.
                Debug.WriteLine("\nException -- Failed：\n" + ex.StackTrace);
                return null;
            }
            return returnString;
        }

        public static async Task<string> GetMDoubanResponseAsync(
            string path,
            string host,
            string reffer,
            string bearer,
            bool isMobileDevice = true,
            string userAgt = null,
            Encoding ecd = null) {
            var returnString = default(string);
            try {
                using(var httpClient = new HttpClient()) {
                    using (var request = GET(path)) {
                        request.Headers.Host = new Windows.Networking.HostName(host);
                        if (reffer != null)
                            request.Headers.Referer = new Uri(reffer);
                        if (userAgt != null) 
                            request.Headers["User-Agent"] = userAgt;
                        else 
                            request.Headers["User-Agent"] =
                                isMobileDevice ?
                                "Mozilla/5.0 (Windows Phone 10.0; Android 4.2.1; Microsoft; Lumia 950) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2486.0 Mobile Safari/537.36 Edge/14.14263" :
                                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.79 Safari/537.36 Edge/14.14393";
                        request.Headers["Connection"] = "Keep-Alive";
                        request.Headers["Authorization"] = $"Bearer {bearer}";
                        var result = await httpClient.SendRequestAsync(request);
                        returnString = ecd == null ? (await CastStreamResultToStringAsync(result, false)).ToString() : (await CastStreamResultToStringAsync(result, ecd)).ToString();
                    }
                }
            } catch (COMException ex) { // it is obvious that the internrt connect goes wrong.
                Debug.WriteLine("\nCOMException -- Failed：\n" + ex.StackTrace);
                return null;
            } catch (Exception ex) { // unkown error, report it.
                Debug.WriteLine("\nException -- Failed：\n" + ex.StackTrace);
                return null;
            }
            return returnString;
        }

        public static async Task<string> PostDoubanResponseAsync(
            string path,
            string host,
            string reffer,
            HttpFormUrlEncodedContent content,
            bool isMobileDevice = false,
            Encoding ecd = null) {
            var returnString = default(string);
            try { 
                using (var httpClient = new HttpClient()) {
                    httpClient.DefaultRequestHeaders.Host = new Windows.Networking.HostName(host);
                    if (reffer != null)
                        httpClient.DefaultRequestHeaders.Referer = new Uri(reffer);
                    using (var request = await LOGIN_POSTAsync(httpClient, path, content)) {
                        returnString = ecd == null ? 
                            (await CastStreamResultToStringAsync(request, false)).ToString() : 
                            (await CastStreamResultToStringAsync(request, ecd)).ToString();
                    }
                }
            } catch (COMException ex) { // it is obvious that the internrt connect goes wrong.
                Debug.WriteLine("\nCOMException -- Failed：\n" + ex.StackTrace);
                return null;
            } catch (Exception ex) { // unkown error, report it.
                Debug.WriteLine("\nException -- Failed：\n" + ex.StackTrace);
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
            Encoding ecd = null,
            string bearer = null) {
            return await PostDoubanResponseAsync(path, host, reffer, userAgent: userAgent, origin: null, content: content, ecd: ecd, bearer: bearer);
        }

        public static async Task<string> PostDoubanResponseAsync(
            string path,
            string host,
            string reffer,
            string userAgent,
            string origin,
            HttpFormUrlEncodedContent content,
            Encoding ecd = null,
            string bearer = null) {
            var returnString = default(string);
            try {
                using (var httpClient = new HttpClient()) {
                    httpClient.DefaultRequestHeaders.Host = new Windows.Networking.HostName(host);
                    if (reffer != null)
                        httpClient.DefaultRequestHeaders.Referer = new Uri(reffer);
                    if (userAgent != null)
                        httpClient.DefaultRequestHeaders["User-Agent"] = userAgent;
                    if (bearer != null)
                        httpClient.DefaultRequestHeaders["Authorization"] = $"Bearer {bearer}";
                    if (origin != null)
                        httpClient.DefaultRequestHeaders["Origin"] = origin;
                    httpClient.DefaultRequestHeaders["Connection"] = "Keep-Alive";
                    using (var request = await LOGIN_POSTAsync(httpClient, path, content)) {
                        returnString = ecd == null ?
                            (await CastStreamResultToStringAsync(request, false)).ToString() :
                            (await CastStreamResultToStringAsync(request, ecd)).ToString();
                    }
                }
            } catch (COMException ex) { // it is obvious that the internrt connect goes wrong.
                Debug.WriteLine("\nCOMException -- Failed：\n" + ex.StackTrace);
                return null;
            } catch (Exception ex) { // unkown error, report it.
                Debug.WriteLine("\nException -- Failed：\n" + ex.StackTrace);
                return null;
            }
            return returnString;
        }

        public static async Task<string> GetAPIResponseAsync(
            string path,
            string host,
            string reffer,
            bool isMobileDevice = true,
            Encoding ecd = null) {
            var returnString = default(string);
            try {
                using (var httpClient = new HttpClient()) {
                    httpClient.DefaultRequestHeaders.Host = new Windows.Networking.HostName(host);
                    if (reffer != null)
                        httpClient.DefaultRequestHeaders.Referer = new Uri(reffer);
                    httpClient.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
                    using (var request = GET(path)) {
                        var result = await httpClient.SendRequestAsync(request);
                        returnString = ecd == null ? (await CastStreamResultToStringAsync(result, false)).ToString() : (await CastStreamResultToStringAsync(result, ecd)).ToString();
                    }
                }
            } catch (COMException ex) { // it is obvious that the internrt connect goes wrong.
                Debug.WriteLine("\nCOMException -- Failed：\n" + ex.StackTrace);
                return null;
            } catch (Exception ex) { // unkown error, report it.
                Debug.WriteLine("\nException -- Failed：\n" + ex.StackTrace);
                return null;
            }
            return returnString;
        }

        #endregion

    }
}
