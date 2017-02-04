using Douban.UWP.Core.Models.FMModels.MHzSongListModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Douban.UWP.Core.Models.FMModels {
    public static class MHzSongListHelper {

        public static void AddSingerEachOne(MHzSong song, JToken singers) {
            if (singers != null && singers.HasValues)
                singers.Children().ToList().ForEach(jo_singer => song.Singers.Add(CreateSingerInstance(jo_singer)));
        }

        public static MHzSinger CreateSingerInstance(JToken jo_singer) {
            return new MHzSinger {
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

        public static void AddItemInfo(MHzSong song, JToken jo_item_info) {
            if (jo_item_info == null)
                return;
            song.ItemInfo = CreateDefaultItemInfoInstance(jo_item_info);
        }

        public static MHzSongItemInfo CreateDefaultItemInfoInstance(JToken jo_release) {
            return new MHzSongItemInfo {
                CreateTime = jo_release["created_time"].Value<string>(),
                ItemID = jo_release["item_id"].Value<string>(),
                Comment = jo_release["comment"].Value<string>(),
            };
        }

        public static void AddCreatorMessage(MHzSongList list, JToken creator_jo) {
            if (creator_jo == null)
                return;
            list.Creator = CreateDefaultCreatorInstance(creator_jo);
        }

        public static FMListCreater CreateDefaultCreatorInstance(JToken creator) {
            return new FMListCreater {
                ID = creator["id"].Value<string>(),
                Name = creator["name"].Value<string>(),
                Type = creator["type"].Value<string>(),
                Picture = creator["picture"].Value<string>(),
                SonglistsCount = creator["songlists_count"].Value<int>(),
                Url = creator["url"].Value<string>(),
            };
        }

        public static MHzSong CreateDefaultSongInstance(JToken jo_song) {
            return new MHzSong {
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
                TasteStatus = jo_song["taste_status"].Value<int>(),
                Singers = new List<MHzSinger>(),
            };
        }

        public static MHzSongList CreateDefaultSongList(JObject jo_main) {
            return new MHzSongList {
                IsCollected = jo_main["is_collected"].Value<bool>(),
                Cover = jo_main["cover"].Value<string>(),
                ID = jo_main["id"].Value<int>(),
                Description = jo_main["description"].Value<string>(),
                Title = jo_main["title"].Value<string>(),
                CanPlay = jo_main["can_play"].Value<bool>(),
                CollectedCount = jo_main["collected_count"].Value<int>(),
                CoverType = jo_main["cover_type"].Value<int>(),
                Duration = jo_main["duration"].Value<int>(),
                IsPublic = jo_main["is_public"].Value<bool>(),
                RecommandReason = jo_main["rec_reason"].Value<string>(),
                ShowNotPlayable = jo_main["show_not_playable"].Value<bool>(),
                SongsCount = jo_main["songs_count"].Value<int>(),
                Type = jo_main["type"].Value<int>(),
                UpdateTime = jo_main["updated_time"].Value<string>(),
                CreateTime = jo_main["created_time"].Value<string>(),
                CanCollect = jo_main["can_collect"].Value<bool>(),
                CommentsCount = jo_main["comments_count"].Value<int>(),
                Count = jo_main["count"].Value<int>(),
                Songs = new List<MHzSong>()
            };
        }

    }
}
