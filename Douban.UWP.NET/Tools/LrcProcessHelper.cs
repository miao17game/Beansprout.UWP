using Douban.UWP.Core.Models.FMModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI;

namespace Douban.UWP.NET.Tools {

    public class LrcMetaData {
        public string ID { get; set; }
        public string Name { get; set; }
        public string[] Alias { get; set; }
        public double Popularity { get; set; }

        public string AliasShow { get { return string.Join("/", Alias); } }

    }

    public static class LrcProcessHelper {

        const string urlSongInfor = "http://music.163.com/api/search/pc"; /// POST地址
        const string urlLrcInfor = "http://music.163.com/api/song/media?id={0}";

        public static async Task<IList<LrcInfo>> ReadLRCFromWebAsync(string FileName, string Artist, Color TextColor, string lrcString = null) {
            IList<LrcInfo> lrcList = new List<LrcInfo>();
            try {
                var lrcFileString = lrcString ?? await GetSongLrcStringFromYunFluentlyAsync(FileName, Artist);
                if (string.IsNullOrEmpty(lrcFileString)) 
                    return null;
                else
                    ReadFromLrcStringFluently9Number(TextColor, lrcFileString, lrcList);
            } catch (Exception E) {
                Debug.WriteLine("EXEPTION:"+E.Message.ToString());
            }
            return lrcList;
        }

        public static async Task<IEnumerable<LrcMetaData>> GetSongMessageListAsync(string FileName, string Artist) {
            var url = string.Format(urlSongInfor, FileName, Artist); /// url地址
            return await GetPostReturnMessageAsync(url, FileName, Artist); /// 获取歌曲信息
        }

        public static async Task<string> GetSongLrcStringFromYunFluentlyAsync(string FileName, string Artist) {
            var content = await GetSongMessageListAsync(FileName, Artist);
            if (content == null)
                return null;
            int songCount = content.Count();
            string MuiscId = content.ToArray()[0].ID;
            var jSonLrcString = await FetchLrcByIdAsync(MuiscId);
            if (string.IsNullOrEmpty(jSonLrcString))
                return null;
            try {
                var jo = JObject.Parse(jSonLrcString);
                return (string)jo["lyric"];
            } catch (InvalidOperationException IOE) { Debug.WriteLine("INVALID_EXEPTION:" + IOE.Message.ToString()); return null; }
        }

        public static async Task<string> FetchLrcByIdAsync(string MuiscId) {
            var lrc_url = string.Format(urlLrcInfor, MuiscId);
            try {
                var jSonLrcString = await GetContentFluentlyAsync(lrc_url);
                var jo = JObject.Parse(jSonLrcString);
                return (string)jo["lyric"];
            } catch (Exception e) { Debug.WriteLine("FetchLrcById Error:" + e.StackTrace); return null; }
        }

        private static async Task<string> GetContentFluentlyAsync(string url) {
            StringBuilder LrcStringBuider = new StringBuilder("");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            try {
                HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
                var stream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
                while (streamReader.Peek() >= 0) {
                    LrcStringBuider.Append(streamReader.ReadLine() + "\n");
                }
                response.Dispose();
            } catch (WebException ex) { Debug.WriteLine("\n网络连接超时：\n" + ex.Message); return null; }
            request.Abort();
            return LrcStringBuider.ToString();
        }

        public static async Task<IEnumerable<LrcMetaData>> GetPostReturnMessageAsync(string url, string FileName, string Artist) {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            request.Headers[HttpRequestHeader.Referer] = "http://music.163.com/";
            string postString = "s={0}&offset=0&limit=20&type=1";
            postString = string.Format(postString, FileName + Artist);
            byte[] postData = Encoding.UTF8.GetBytes(postString);
            request.Headers[HttpRequestHeader.ContentLength] = Convert.ToString(postData.Length);
            try {
                using (var requestStream = await request.GetRequestStreamAsync()) {
                    requestStream.Write(postData, 0, postData.Length);
                    using (var response = (HttpWebResponse)await request.GetResponseAsync()) {
                        using (var responseStream = response.GetResponseStream()) {
                            using (var streamReader = new StreamReader(responseStream, Encoding.UTF8)) {
                                string retString = streamReader.ReadToEnd();
                                try {
                                    var jo = JObject.Parse(retString);
                                    var songs = jo["result"]["songs"];
                                    return songs.Children().Select(item => new LrcMetaData {
                                        ID = (string)item["id"],
                                        Name = (string)item["name"],
                                        Alias = item["alias"].Children().Select(i => i.Value<string>()).ToArray(),
                                        Popularity = (double)item["popularity"],
                                    });
                                } catch (InvalidOperationException IOE) { Debug.WriteLine(IOE.Message.ToString()); return null; }
                            }
                        }
                    }
                }
            } catch (Exception e) { Debug.WriteLine("GetPostReturnMessage Error:" + e.StackTrace);  return new List<LrcMetaData>(); }
        }

        public static void ReadFromLrcStringFluently9Number(Color TextColor, string lrcFileString, IList<LrcInfo> lrcListOld) {
            IList<LrcInfo> lrcList = new List<LrcInfo>();

            string[] timeBlock = new string[4];
            timeBlock[0] = @"[^\]]{0}[\n]{0}\[[^a-z]{7,9}\][^\[^\n]+"; /// 匹配连续1块时间块+歌词句子的正则表达式
            timeBlock[1] = @"[^\]]{0}[\n]{0}\[[^a-z]{7,9}\]\[[^a-z]{7,9}\][^\[^\n]+"; /// 匹配连续2块时间块+歌词句子的正则表达式
            timeBlock[2] = @"[^\]]{0}[\n]{0}\[[^a-z]{7,9}\]\[[^a-z]{7,9}\]\[[^a-z]{7,9}\][^\[^\n]+"; /// 匹配连续3块时间块+歌词句子的正则表达式
            timeBlock[3] = @"[^\]]{0}[\n]{0}\[[^a-z]{7,9}\]\[[^a-z]{7,9}\]\[[^a-z]{7,9}\]\[[^a-z]{7,9}\][^\[^\n]+"; /// 匹配连续4块时间块+歌词句子的正则表达式

            /// 获取时间块歌词
            for (int n = 0; n < 4; n++) {
                Regex regex = new Regex(timeBlock[n]);
                var collection = regex.Matches(lrcFileString);
                ForeachRegexResult(TextColor, lrcList, collection, false);
            }

            var NewlrcList = lrcList.OrderBy(model => model.LrcTime);
            foreach (var item in NewlrcList) {
                var index = lrcListOld.ToList().FindIndex(m => m.LrcTime == item.LrcTime);
                if (index == -1)
                    lrcListOld.Add(item);
            }
        }

        private static void ForeachRegexResult(Color TextColor, IList<LrcInfo> lrcList, MatchCollection collection, bool Is2time) {
            foreach (Match item in collection) {
                string newLine = item.Value.Replace("[", "");
                newLine = newLine.Replace("]", "@");
                string[] splitLrcData = newLine.Split('@');
                var max = splitLrcData.Length;
                for (int turn = 0; turn < max - 1; turn++) {
                    LrcInfo mLrcContent = new LrcInfo();
                    try {
                        mLrcContent.LrcString = splitLrcData[max - 1].Replace(@"\n"," ");
                        if (Is2time)
                            mLrcContent.LrcTime = Time2Str(splitLrcData[turn]);
                        else
                            mLrcContent.LrcTime = Time3Str(splitLrcData[turn]);
                        mLrcContent.Color = TextColor;
                        if (mLrcContent.LrcTime != 0 && !string.IsNullOrEmpty(mLrcContent.LrcString))
                            lrcList.Add(mLrcContent);
                    } catch (IndexOutOfRangeException) {
                        Debug.WriteLine("索引超范围异常");
                    } catch (NullReferenceException) {
                        Debug.WriteLine("错误  还未检测到歌词正文");
                    } catch (ArgumentNullException) {
                        Debug.WriteLine("算数空值异常  内容错误");
                    }
                }
            }
        }

        private static int Time2Str(string timeStr) {
            timeStr = timeStr.Replace(":", "@");
            timeStr = timeStr.Replace(".", "@");
            string[] timeData = timeStr.Split('@');
            int currentTime = 1;
            if (timeData.Length > 2 && !string.IsNullOrEmpty(timeData[2])) {
                try {
                    int minute = Convert.ToInt32(timeData[0]);
                    int second = Convert.ToInt32(timeData[1]);
                    int millisecond = Convert.ToInt32(timeData[2]);
                    currentTime = (minute * 60 + second) * 1000 + millisecond * 10;
                } catch (FormatException) {
                    Debug.WriteLine("歌词文件作者又搞大新闻  格式转换异常");
                }
            } else if (timeData.Length <= 2) {
                currentTime = 0;
            }
            return currentTime;
        }

        private static int Time3Str(string timeStr) {
            timeStr = timeStr.Replace(":", "@");
            timeStr = timeStr.Replace(".", "@");
            string[] timeData = timeStr.Split('@');
            int currentTime = 1;
            if (timeData.Length > 2 && !string.IsNullOrEmpty(timeData[2])) {
                try {
                    int minute = Convert.ToInt32(timeData[0]);
                    int second = Convert.ToInt32(timeData[1]);
                    int millisecond = Convert.ToInt32(timeData[2]);
                    currentTime = (minute * 60 + second) * 1000 + millisecond;
                } catch (FormatException) {
                    Debug.WriteLine("歌词文件作者又搞大新闻  格式转换异常");
                }
            } else if (timeData.Length <= 2) {
                currentTime = 0;
            }
            return currentTime;
        }

    }
}
