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
                    UnRedirectHttpFilter = new HttpBaseProtocolFilter();
                    UnRedirectHttpFilter.AllowAutoRedirect = false;
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
            RedirectHttpFilter = new HttpBaseProtocolFilter();
            RedirectHttpFilter.AllowAutoRedirect = true;
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

        private async static Task<HttpResponseMessage> LOGIN_POST(HttpClient client, string urlString) {
            return await client.PostAsync(new Uri(urlString), null);
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

        #endregion

        #region Douban Methods

        /// <summary>
        /// command from left tab.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task<string> GetDoubanResponseAsync(string path, bool allowToRedirect = true, HttpClient client = null) {
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
                        returnString = (await CastStreamResultToStringAsync(result, false)).ToString();
                    } else {
                        returnString = await GetDoubanResponseAsync(result.Headers["Location"]);
                    }
                }
            } catch (ObjectDisposedException ex) { // when web connect recovery , recreate a new instance to implemente a recursive function to solve the problem.
                Debug.WriteLine("\nObjectDisposedException -- Failed：\n" + ex.StackTrace);
                return await LNULogOutCallback(new HttpClient(), path);
            } catch (COMException ex) { // it is obvious that the internrt connect goes wrong.
                Debug.WriteLine("\nCOMException -- Failed：\n" + ex.StackTrace);
                return "Connect Error.";
            } catch (Exception ex) { // unkown error, report it.
                Debug.WriteLine("\nException -- Failed：\n" + ex.StackTrace);
                return "Unknown Error.";
            }
            return returnString;
        }

        #endregion

        #region LNU Methods

        /// <summary>
        /// Login into LNU, should only call it when login-cookie is gone.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static async Task<LoginReturnBag> PostLNULoginCallback(HttpClient client, string user, string password) {

            /// Changes for Windows Store

            //var urlString = string.Format("http://jwgl.lnu.edu.cn/pls/wwwbks/bks_login2.login?stuid={0}&pwd={1}", user, password);

            var urlString = string.Format("https://notificationhubforuwp.azurewebsites.net/LNU/Redirect?user={0}&psw={1}", user, password);
            var bag = new LoginReturnBag();
            try { // do not dispose, so that the global undirect httpclient will stay in referenced. dispose it when you need.
                var httpClient = client;

                //httpClient.DefaultRequestHeaders.Host = new Windows.Networking.HostName("jwgl.lnu.edu.cn");
                //httpClient.DefaultRequestHeaders.Referer = new Uri("http://jwgl.lnu.edu.cn/zhxt_bks/xk_login.html");

                /// 
                
                using (var response = await LOGIN_POST(client, urlString)) {
                    var returnCookies = UnRedirectCookiesManager.GetCookies(new Uri("https://notificationhubforuwp.azurewebsites.net/"));
                    if (returnCookies.Count == 0) 
                        throw new AccessUnPassedException("Login Failed: no login-success cookie received.");

                    /// Changes for Windows Store
                    
                    string content = default(string);
                    var value = response.Headers.TryGetValue("Set-Cookie", out content);
                    if (value) {
                        content = content.Split(',')[0].Replace(";", "@").Split('@')[0].Replace("=", "@").Split('@')[1];
                        HttpCookie cookie = new HttpCookie("ACCOUNT", "jwgl.lnu.edu.cn", "/pls/wwwbks/");
                        cookie.Value = content;

                        /// DEBUG Method

                        Debug.WriteLine("DEBUG ----->   " + content );

                        ///

                        UnRedirectCookiesManager.SetCookie(cookie);
                        bag.CookieBag = cookie;
                    }

                    ///

                    using (var request = GET("http://jwgl.lnu.edu.cn/pls/wwwbks/bks_login2.loginmessage")) {
                        request.Headers.Host = new Windows.Networking.HostName("jwgl.lnu.edu.cn");
                        request.Headers.Referer = new Uri("http://jwgl.lnu.edu.cn/zhxt_bks/xk_login.html");
                        var result = await httpClient.SendRequestAsync(request);
                        bag.HtmlResouces = (await CastStreamResultToStringAsync(result)).ToString();
                    }
                }
            } catch (ObjectDisposedException ex) { // when web connect recovery , recreate a new instance to implemente a recursive function to solve the problem.
                Debug.WriteLine("\nFailed：\n" + ex.StackTrace);
                unRedirectHttpClient = null;
                return await PostLNULoginCallback(UnRedirectHttpClient, user, password);
            } catch (COMException ex) { // it is obvious that the internrt connect goes wrong.
                Debug.WriteLine("\nFailed：\n" + ex.StackTrace);
                return null;
            } catch (AccessUnPassedException ex) {
                Debug.WriteLine("\nFailed：\n" + ex.StackTrace);
                unRedirectHttpClient = null;
                return bag;
            } catch (Exception ex) { // unkown error, report it.
                Debug.WriteLine("\nFailed：\n" + ex.StackTrace);
                return null;
            }
            return bag;
        }

        /// <summary>
        /// Log out from LNU
        /// </summary>
        /// <param name="client"></param>
        /// <param name="logoutPath"></param>
        /// <returns></returns>
        public static async Task<string> LNULogOutCallback(HttpClient client, string logoutPath) {
            var bag = default(string);
            try { // do not dispose, so that the global undirect httpclient will stay in referenced. dispose it when you need.
                var httpClient = client;
                using (var request = GET(logoutPath)) {
                    request.Headers.Host = new Windows.Networking.HostName("jwgl.lnu.edu.cn");
                    request.Headers.Referer = new Uri("http://jwgl.lnu.edu.cn/zhxt_bks/zhxt_bks_left.html");
                    var result = await httpClient.SendRequestAsync(request);
                    bag = (await CastStreamResultToStringAsync(result)).ToString();
                }
            } catch (ObjectDisposedException ex) { // when web connect recovery , recreate a new instance to implemente a recursive function to solve the problem.
                Debug.WriteLine("\nFailed：\n" + ex.StackTrace);
                unRedirectHttpClient = null;
                return await LNULogOutCallback(UnRedirectHttpClient, logoutPath);
            } catch (COMException ex) { // it is obvious that the internrt connect goes wrong.
                Debug.WriteLine("\nFailed：\n" + ex.StackTrace);
                return null;
            } catch (Exception ex) { // unkown error, report it.
                Debug.WriteLine("\nFailed：\n" + ex.StackTrace);
                return null;
            }
            return bag;
        }

        /// <summary>
        /// command from left tab.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="logoutPath"></param>
        /// <returns></returns>
        public static async Task<string> GetLNUFromLeftRequest(HttpClient client, string logoutPath) {
            var bag = default(string);
            try { // do not dispose, so that the global undirect httpclient will stay in referenced. dispose it when you need.
                var httpClient = client;
                using (var request = GET(logoutPath)) {
                    request.Headers.Host = new Windows.Networking.HostName("jwgl.lnu.edu.cn");
                    request.Headers.Referer = new Uri("http://jwgl.lnu.edu.cn/zhxt_bks/zhxt_bks_left.html");
                    var result = await httpClient.SendRequestAsync(request);
                    bag = (await CastStreamResultToStringAsync(result)).ToString();
                }
            } catch (ObjectDisposedException ex) { // when web connect recovery , recreate a new instance to implemente a recursive function to solve the problem.
                Debug.WriteLine("\nFailed：\n" + ex.StackTrace);
                unRedirectHttpClient = null;
                return await LNULogOutCallback(UnRedirectHttpClient, logoutPath);
            } catch (COMException ex) { // it is obvious that the internrt connect goes wrong.
                Debug.WriteLine("\nFailed：\n" + ex.StackTrace);
                return null;
            } catch (Exception ex) { // unkown error, report it.
                Debug.WriteLine("\nFailed：\n" + ex.StackTrace);
                return null;
            }
            return bag;
        }

        /// <summary>
        /// change password.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="logoutPath"></param>
        /// <returns></returns>
        public static async Task<string> PostLNUChangePassword(HttpClient client, string logoutPath) {
            var bag = default(string);
            try { // do not dispose, so that the global undirect httpclient will stay in referenced. dispose it when you need.
                var httpClient = client;
                using (var request = POST(logoutPath)) {
                    request.Headers.Host = new Windows.Networking.HostName("jwgl.lnu.edu.cn");
                    request.Headers.Referer = new Uri("http://jwgl.lnu.edu.cn/pls/wwwbks/bks_login2.NewPass");
                    var result = await httpClient.SendRequestAsync(request);
                    bag = (await CastStreamResultToStringAsync(result)).ToString();
                }
            } catch (ObjectDisposedException ex) { // when web connect recovery , recreate a new instance to implemente a recursive function to solve the problem.
                Debug.WriteLine("\nFailed：\n" + ex.StackTrace);
                unRedirectHttpClient = null;
                return await PostLNUChangePassword(UnRedirectHttpClient, logoutPath);
            } catch (COMException ex) { // it is obvious that the internrt connect goes wrong.
                Debug.WriteLine("\nFailed：\n" + ex.StackTrace);
                return null;
            } catch (Exception ex) { // unkown error, report it.
                Debug.WriteLine("\nFailed：\n" + ex.StackTrace);
                return null;
            }
            return bag;
        }

        /// <summary>
        /// change password.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="logoutPath"></param>
        /// <returns></returns>
        public static async Task<string> PostLNURedirectPOSTMethod(HttpClient client, string logoutPath, HttpCookie cookie) {
            var bag = default(string);
            try { // do not dispose, so that the global undirect httpclient will stay in referenced. dispose it when you need.
                using (var request = POST(logoutPath)) {
                    request.Headers["Cookie"] = "ACCOUNT=" + cookie.Value + "; path=/pls/wwwbks/";
                    var result = await client.SendRequestAsync(request);
                    bag = (await CastStreamResultToStringAsync(result, false)).ToString();
                }
            } catch (ObjectDisposedException ex) { // when web connect recovery , recreate a new instance to implemente a recursive function to solve the problem.
                Debug.WriteLine("\nFailed：\n" + ex.StackTrace);
                unRedirectHttpClient = null;
                return await PostLNURedirectPOSTMethod(UnRedirectHttpClient, logoutPath, cookie);
            } catch (COMException ex) { // it is obvious that the internrt connect goes wrong.
                Debug.WriteLine("\nFailed：\n" + ex.StackTrace);
                return null;
            } catch (Exception ex) { // unkown error, report it.
                Debug.WriteLine("\nFailed：\n" + ex.StackTrace);
                return null;
            }
            return bag;
        }

        #endregion

    }
}
