using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Wallace.UWP.Helpers;

namespace Douban.UWP.Core.Tools {
    public static class UriDecoder {

        public static string EditKeyWordsForBooks(string oldkeys) {
            return "subject_collection/filter_book_" +
                oldkeys
                .Replace("novel", "fiction")
                .Replace("bio", "biography")
                .Replace("motivation", "inspiration")
                .Replace("business", "economic") +
                "_hot/";
        }

        public static string CreateJson<T>(T obj) {
            return JsonHelper.ToJson(obj);
        }

        /// <summary>
        /// Create api path from Douban-uri-dispatch string.
        /// </summary>
        /// <param name="uriDispatch">dispatch-string.</param>
        /// <returns></returns>
        public static string GetApiFromDispatch(string uriDispatch) {
            var compath = new Regex(@"dispatch\?uri=(?<com_path>.+)").Match(uriDispatch).Groups["com_path"].Value;
            if (compath != "")
                return apiFormat + compath;
            return uriDispatch;
        }

        /// <summary>
        /// Get url from the from Douban-uri-dispatch string.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string GetUrlFromUri(string uri) {
            if (uri == null)
                return uri;
            var path = new Regex(@"https://.+").Match(uri).Value;
            if (path != "")
                return path;
            path = new Regex(@"douban://(?<com_path>.+)").Match(uri).Groups["com_path"].Value;
            if (path != "")
                return headFormat_web + path;
            return uri;
        }

        public static string GetApiFromUri(string uri) {
            return uri;
        }

        public static UriType TypeDispatch(string apiPath) {
            return GetUriType(new Regex(@"rexxar/api/v2/(?<api_keywords>.+?)/)").Match(apiPath).Groups["api_keywords"].Value);
        }

        public static UriType GetUriType(string key) { return TypeReflections.ContainsKey(key) ? TypeReflections[key] : UriType.Unknown; }
        private static IDictionary<string, UriType> _typeReflections;
        private static IDictionary<string, UriType> TypeReflections {
            get {
                return _typeReflections ?? (_typeReflections = new Dictionary<string, UriType> {
                    { "subject_collection", UriType.SubjectCollection },
                    { "book", UriType.SingletonBook },
                    { "movie", UriType.SingletonMovie },
                    { "music", UriType.SingletonMusic },
                });
            }
        }

        #region Constants Field
        const string headFormat_web = "https://m.";
        const string apiFormat = "https://m.douban.com/rexxar/api/v2";
        #endregion

    }

    /// <summary>
    /// The uri format of Douban uri-source.
    /// </summary>
    public enum UriType {
        Unknown,
        SubjectCollection,
        SingletonBook,
        SingletonMovie,
        SingletonMusic,
    }

    /// <summary>
    /// The type of returns
    /// </summary>
    public enum TitleEncodeEnum {
        uri,
        title,
    }

}
