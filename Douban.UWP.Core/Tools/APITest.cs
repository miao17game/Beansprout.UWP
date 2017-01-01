using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Windows.Web.Http;

namespace Douban.UWP.Core.Tools {
    public static class APITest {

        public static async void TestAsync() {
            var result = await DoubanWebProcess.GetMDoubanResponseAsync("https://m.douban.com/rexxar/api/v2/movie/26382689/interests?count=4&order_by=hot&start=0&ck=17fl&for_mobile=1");
            JObject jo = JObject.Parse(result);
            Debug.WriteLine(jo.ToString());
        }

        public static async void TestAsync(bool moreMessage) {
            var result = await DoubanWebProcess.GetMDoubanResponseAsync(
                string.Format("https://frodo.douban.com/jsonp/subject_collection/music_chinese/items?os=windows&callback=jsonp1&start=0&count=8&loc_id=0&_={0}", (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds),
                "frodo.douban.com",
                "https://m.douban.com/music/");
            JObject jo = JObject.Parse(result.Substring(7, result.Length - 8));
            Debug.WriteLine(jo.ToString());
        }

        public static async void TestAsync(bool moreMessage, bool isMobile, string path, string host, string reffer, HttpFormUrlEncodedContent content) {
            var result = await DoubanWebProcess.PostDoubanResponseAsync(
                path ,
                host,
                reffer,
                content,
                isMobile);
            JObject jo  = JObject.Parse(result);
            Debug.WriteLine(jo.ToString());
        }

    }
}
