using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace Douban.UWP.Core.Tools {
    public static class BeansproutRequestHelper {

        public static async Task<string> AccessOauth2Token(string email, string password) {
            return await DoubanWebProcess.PostDoubanResponseAsync(
                path: "https://frodo.douban.com/service/auth2/token",
                host: "frodo.douban.com",
                reffer: null,
                content:
                new HttpFormUrlEncodedContent(new List<KeyValuePair<string, string>> {
                            new KeyValuePair<string, string>("client_id","0dad551ec0f84ed02907ff5c42e8ec70"),
                            new KeyValuePair<string, string>("client_secret","9e8bb54dc3288cdf"),
                            new KeyValuePair<string, string>("redirect_uri","frodo://app/oauth/callback/"),
                            new KeyValuePair<string, string>("grant_type","password"),
                            new KeyValuePair<string, string>("username", email),
                            new KeyValuePair<string, string>("password", password),
                            new KeyValuePair<string, string>("os_rom","android"),
                }),
                isMobileDevice: true);
        }

        public static async Task<string> FetchTypeCollectionList(
            string headString, 
            string loc_id, 
            uint start, 
            uint count,
            double minised,
            SubjectType type = SubjectType.Books,
            RequestType request = RequestType.Groups) {
            var refer_sub_type =
                type == SubjectType.Books ? "book" :
                type == SubjectType.Movies ? "movie" :
                type == SubjectType.Music ? "music" :
                "tv";
            var formatPath = request == RequestType.SubjectCollection ? SubjectCollectionPath : TypePath;
            return await DoubanWebProcess.GetMDoubanResponseAsync(string.Format(formatPath, new object[] { headString, start, count, loc_id, minised }),
                "m.douban.com",
                $"{"https:"}//m.douban.com/{refer_sub_type}/");
        }

        const string TypePath = "https://m.douban.com/rexxar/api/v2/{0}items?os=android&start={1}&count={2}&loc_id={3}&_={4}";
        const string SubjectCollectionPath = "https://m.douban.com/rexxar/api/v2/subject_collection/{0}/items?os=android&start={1}&count={2}&loc_id={3}&_={4}";

    }

    public enum SubjectType { Books, Movies, Music, TVs }
    public enum RequestType { Groups, SubjectCollection }

}
