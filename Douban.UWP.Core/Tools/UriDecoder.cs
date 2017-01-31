using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

        public static string UriToEncode(string strToEncode, string encodeString = ToastFromDefault, string title = "Link") {
            return encodeString + BeansDevide + strToEncode + TitleDevide + title;
        }

        public static string UriToDecode(string strToDecode, string encodeString = ToastFromDefault) {
            var result = new Regex(@"(?<format>.+)" + BeansDevide + @"(?<content>.+)").Match(strToDecode);
            if (result == null || result.Groups["format"].Value == "")
                return null;
            return result.Groups["content"].Value;
        }

        public static string UriToDecodeTitle(string strToDecodeTitle, TitleEncodeEnum type = TitleEncodeEnum.title) {
            var match = new Regex(@"(?<uri>.+)" + TitleDevide + @"(?<title>.+)").Match(strToDecodeTitle);
            return match.Groups[type.ToString()].Value;
        }

        public static string GetApiFromDispatch(string uriDispatch) {
            var compath = new Regex(@"dispatch\?uri=(?<com_path>.+)").Match(uriDispatch).Groups["com_path"].Value;
            if (compath != "")
                return apiFormat + compath;
            return uriDispatch;
        }

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

        const string headFormat_web = "https://m.";
        const string apiFormat = "https://m.douban.com/rexxar/api/v2";

        public static UriType GetUriType(string key) { return TypeReflections.ContainsKey(key) ? TypeReflections[key] : UriType.Unknown; }
        private static IDictionary<string, UriType> typeReflections;
        public static IDictionary<string, UriType> TypeReflections {
            get {
                return typeReflections ?? (typeReflections = new Dictionary<string, UriType> {
                    { "subject_collection", UriType.SubjectCollection },
                    { "book", UriType.SingletonBook },
                    { "movie", UriType.SingletonMovie },
                    { "music", UriType.SingletonMusic },
                });
            }
        }

        public const string BeansDevide = "@BeansFormat@";
        public const string TitleDevide = "@TitleFormat@";
        public const string ToatFromInfosList = "INFOSLIST_TOAST";
        public const string ToastFromDefault = "DEFAULT";

    }

    public enum UriType {
        Unknown,
        SubjectCollection,
        SingletonBook,
        SingletonMovie,
        SingletonMusic,
    }

    public enum TitleEncodeEnum {
        uri,
        title,
    }

}
