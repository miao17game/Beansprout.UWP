using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Douban.UWP.Core.Tools {
    public static class APITest {

        public static async void Test() {
            var result = await DoubanWebProcess.GetMDoubanResponseAsync("https://frodo.douban.com/jsonp/subject_collection/movie_showing/items?os=windows&callback=jsonp1&start=0&count=8&loc_id=108288&_=1482936121486");
            JObject jo = JObject.Parse(result);
            Debug.WriteLine(jo.ToString());
        }

        public static async void Test(bool isT) {
            var result = await DoubanWebProcess.GetMDoubanResponseAsync(
                string.Format("https://frodo.douban.com/jsonp/subject_collection/movie_showing/items?os=windows&callback=jsonp1&start=0&count=8&loc_id=108288&_={0}", (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds),
                "frodo.douban.com",
                "https://m.douban.com/movie/");
            JObject jo = JObject.Parse(result.Substring(7, result.Length - 8));
            Debug.WriteLine(jo.ToString());
        }

    }
}
