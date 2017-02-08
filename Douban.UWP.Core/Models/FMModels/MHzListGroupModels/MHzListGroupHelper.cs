using Douban.UWP.Core.Tools;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.FMModels {
    public static class MHzListGroupHelper {

        public static async Task<IList<MHzSongBase>> FetchMHzSongsAsync(int list_id, string api_key, string bearer) {
            var result = await DoubanWebProcess.GetMDoubanResponseAsync(
                path: $"{"https://"}api.douban.com/v2/fm/playlist?channel={list_id}&formats=null&from=&type=n&version=644&start=0&app_name=radio_android&limit=10&apikey={api_key}",
                host: "api.douban.com",
                reffer: null,
                bearer: bearer,
                userAgt: @"api-client/2.0 com.douban.radio/4.6.4(464) Android/18 TCL_P306C TCL TCL-306C");
            try {
                var jo = JObject.Parse(result);
                var songs = jo["song"];
                var group = CreateDefaultListGroup(jo);
                if (songs != null && songs.HasValues) {
                    songs.Children().ToList().ForEach(jo_song => {
                        try {
                            var song = CreateDefaultSongInstance(jo_song);
                            AddSingerEachOne(song, jo_song["singers"]);
                            AddRelease(song, jo_song["release"]);
                            group.Songs.Add(song);
                        } catch { /* Ingore */ }
                    });
                }
                return group.Songs;
            } catch { return null; }
        }

        public static void AddRelease(MHzSongBase song, JToken jo_release) {
            if (jo_release == null)
                return;
            song.Release = CreateDefaultReleaseInstance(jo_release);
        }

        public static MHzListRelease CreateDefaultReleaseInstance(JToken jo_release) {
            return new MHzListRelease {
                SSID = jo_release["ssid"].Value<string>(),
                ID = jo_release["id"].Value<string>(),
                Link = jo_release["link"].Value<string>(),
            };
        }

        public static void AddSingerEachOne(MHzSongBase song, JToken singers) {
            if (singers != null && singers.HasValues)
                singers.Children().ToList().ForEach(jo_singer => song.Singers.Add(CreateSingerInstance(jo_singer)));
        }

        public static MHzSingerBase CreateSingerInstance(JToken jo_singer) {
            return new MHzSingerBase {
                Avatar = jo_singer["avatar"].Value<string>(),
                Genre = jo_singer["genre"].Children().Select(i => i.Value<string>()).ToList(),
                ID = jo_singer["id"].Value<string>(),
                IsSiteArtist = jo_singer["is_site_artist"].Value<bool>(),
                Name = jo_singer["name"].Value<string>(),
                NameUsual = jo_singer["name_usual"].Value<string>(),
                Region = jo_singer["region"].Children().Select(i => i.Value<string>()).ToList(),
                RelatedSiteId = jo_singer["related_site_id"].Value<int>(),
            };
        }

        public static MHzSongBase CreateDefaultSongInstance(JToken jo_song) {
            return new MHzSongBase {
                AID = jo_song["aid"].Value<string>(),
                Album = jo_song["album"].Value<string>(),
                AlbumTitle = jo_song["albumtitle"].Value<string>(),
                AlertMessage = jo_song["alert_msg"].Value<string>(),
                Artist = jo_song["artist"].Value<string>(),
                FileExtensionName = jo_song["file_ext"].Value<string>(),
                IsRoyal = jo_song["is_royal"].Value<bool>(),
                Kbps = jo_song["kbps"].Value<string>(),
                Length = jo_song["length"].Value<int>(),
                LikeCount = jo_song["like"].Value<int>(),
                Picture = jo_song["picture"].Value<string>(),
                PublicTime = jo_song["public_time"].Value<string>(),
                SHA256 = jo_song["sha256"].Value<string>(),
                SID = jo_song["sid"].Value<string>(),
                SSID = jo_song["ssid"].Value<string>(),
                Status = jo_song["status"].Value<int>(),
                SubType = jo_song["subtype"].Value<string>(),
                Title = jo_song["title"].Value<string>(),
                UpdateTime = jo_song["update_time"].Value<long>(),
                Url = jo_song["url"].Value<string>(),
                Singers = new List<MHzSingerBase>(),
            };
        }

        public static MHzGroupBase CreateDefaultListGroup(JObject jo) {
            return new MHzGroupBase {
                R = jo["r"].Value<int>(),
                IsShowQuickStart = jo["is_show_quick_start"].Value<int>(),
                VersionMAX = jo["version_max"].Value<int>(),
                Songs = new List<MHzSongBase>()
            };
        }

    }
}
