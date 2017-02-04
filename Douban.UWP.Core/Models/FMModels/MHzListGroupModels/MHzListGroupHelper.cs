using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.FMModels {
    public static class MHzListGroupHelper {

        public static void AddRelease(MHzListSong song, JToken jo_release) {
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

        public static void AddSingerEachOne(MHzListSong song, JToken singers) {
            if (singers != null && singers.HasValues)
                singers.Children().ToList().ForEach(jo_singer => song.Singers.Add(CreateSingerInstance(jo_singer)));
        }

        public static MHzListSinger CreateSingerInstance(JToken jo_singer) {
            return new MHzListSinger {
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

        public static MHzListSong CreateDefaultSongInstance(JToken jo_song) {
            return new MHzListSong {
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
                Singers = new List<MHzListSinger>(),
            };
        }

        public static MHzListGroup CreateDefaultListGroup(JObject jo) {
            return new MHzListGroup {
                R = jo["r"].Value<int>(),
                IsShowQuickStart = jo["is_show_quick_start"].Value<int>(),
                VersionMAX = jo["version_max"].Value<int>(),
                Songs = new List<MHzListSong>()
            };
        }

    }
}
