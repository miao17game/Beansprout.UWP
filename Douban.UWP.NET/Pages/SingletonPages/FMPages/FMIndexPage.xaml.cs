using static Wallace.UWP.Helpers.Tools.UWPStates;
using static Douban.UWP.NET.Resources.AppResources;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Douban.UWP.Core.Tools;
using Douban.UWP.Core.Models.FMModels;
using Newtonsoft.Json.Linq;
using Wallace.UWP.Helpers;
using Douban.UWP.Core.Models;
using System.Threading.Tasks;
using Douban.UWP.NET.Tools;
using Windows.Media.Playback;

namespace Douban.UWP.NET.Pages.SingletonPages.FMPages {

    public sealed partial class FMIndexPage : Page {
        public FMIndexPage() {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            uid = UserID ?? "";
            bearer = AccessToken ?? "";
            if (UserID == null)
                return;
            Method02Async();
        }

        private async void Method02Async() {
            var result = await DoubanWebProcess.GetMDoubanResponseAsync(
                path: $"{"https://"}api.douban.com/v2/fm/app_channels?version=644&app_name=radio_android&apikey={api_key}",
                host: "api.douban.com",
                reffer: null,
                bearer: bearer,
                userAgt: @"api-client/2.0 com.douban.radio/4.6.4(464) Android/18 TCL_P306C TCL TCL-306C");
            var list = GroupsInit(result);
            ListResources.Source = list;
        }

        #region Main List Resources

        private IList<ChannelGroup> GroupsInit(string value) {
            IList<ChannelGroup> list = new List<ChannelGroup>();
            try {
                var JO = JObject.Parse(value);
                var groups = JO["groups"];
                if (groups != null && groups.HasValues) {
                    groups.Children().ToList().ForEach(singleton => {
                        var groupItem = CreateDefaultGroupInstance(singleton);
                        var chls = singleton["chls"];
                        if (chls != null && chls.HasValues) {
                            chls.Children().ToList().ForEach(everyChl => {
                                var item = CreateDefaultChannelInstance(everyChl);
                                CreateStyle(item, everyChl["style"]);
                                CreateRelation(item, everyChl["channel_relation"]);
                                groupItem.CHLS.Add(item);
                            });
                        }
                        list.Add(groupItem);
                    });
                }
            } catch { /* Ingore. */ } finally { IncrementalLoadingBorder.SetVisibility(false); }
            return list;
        }

        private static void CreateRelation(ChannelsItem item, JToken relation) {
            if (relation == null || !relation.HasValues)
                return;
            item.Relation = new ChannelRelation();
            var artist = relation["artist"];
            var song = relation["song"];
            if (artist != null)
                item.Relation.ArtistID = artist["id"].Value<string>();
            if (song != null) {
                item.Relation.SongID = song["id"].Value<string>();
                item.Relation.SSID = song["ssid"].Value<string>();
            }
        }

        private static ChannelGroup CreateDefaultGroupInstance(JToken singleton) {
            return new ChannelGroup {
                CHLS = new List<ChannelsItem>(),
                GroupId = singleton["group_id"].Value<int>(),
                GroupName = singleton["group_name"].Value<string>() != "" ? singleton["group_name"].Value<string>() : "Douban FM",
            };
        }

        private static ChannelsItem CreateDefaultChannelInstance(JToken everyChl) {
            var num = 0;
            if (everyChl["song_num"] != null)
                num = everyChl["song_num"].Value<int>();
            return new ChannelsItem {
                Collected = everyChl["collected"].Value<string>(),
                Cover = everyChl["cover"].Value<string>(),
                Id = everyChl["id"].Value<int>(),
                Intro = everyChl["intro"].Value<string>(),
                Name = everyChl["name"].Value<string>(),
                SongNumber = num
            };
        }

        private static void CreateStyle(ChannelsItem item, JToken style) {
            string text = null;
            if (style["display_text"] != null)
                text = style["display_text"].Value<string>();
            if (style.HasValues)
                item.Style = new ChannelStyle {
                    BgColor = style["bg_color"].Value<string>(),
                    BgImage = style["bg_image"].Value<string>(),
                    LayoutType = style["layout_type"].Value<int>(),
                    DisplayText = text
                };
        }

        #endregion

        private async void ListView_ItemClickAsync(object sender, ItemClickEventArgs e) {
            var item = e.ClickedItem as ChannelsItem;
            if (item == null)
                return;
            var succeed = Service.ChangeServiceChoice(MusicServiceType.MHz);
            if (!succeed)
                return;
            Service.MHzChannelID = item.Id;
            Service.ActionForMHz = GetNewMHzEachTurnAsync;
            var song = await InsertSongsToMHzListAsync();
            if (song == null)
                return;
            if (MainUpContentFrame.Content != null)
                (MainUpContentFrame.Content as FM_SongBoardPage)?.UnregisterServiceEvents();
            Service.MoveToAnyway(song);
            Service.PlayAnyway();
            NavigateToBase?.Invoke(
                null,
                new MusicBoardParameter {
                    SID = song.SID,
                    SSID = song.SSID,
                    AID = song.AID,
                    SHA256 = song.SHA256,
                    FrameType = FrameType.UpContent
                },
                GetFrameInstance(FrameType.UpContent),
                GetPageType(NavigateType.MusicBoard));
        }

        private async void GetNewMHzEachTurnAsync() {
            await Dispatcher.UpdateUI(async () => {
                if (Service.FindItemIndex(Service.PlaybackList.CurrentItem) +1 >= Service.PlaybackList.Items.Count - 1) {
                    var song = await InsertSongsToMHzListAsync();
                    if (song == null)
                        return;
                }
            });
        }

        private async Task<MHzSongBase> InsertSongsToMHzListAsync() {
            var songs = await MHzListGroupHelper.FetchMHzSongsAsync(Service.MHzChannelID, api_key, bearer);
            if (songs == null || songs.Count == 0)
                return null;
            songs.ToList().ForEach(song => Service.InsertItem(song));
            if (Service.PlaybackList.Items.Count <= 1) { // more songs to prepare, at least 2.
                var songs_add = await MHzListGroupHelper.FetchMHzSongsAsync(Service.MHzChannelID, api_key, bearer);
                if (songs_add != null && songs_add.Count != 0)
                    songs_add.ToList().ForEach(song => Service.InsertItem(song));
            }
            return songs[0];
        } 

        #region Properties
        string uid;
        string bearer;
        const string sdk_version = "1.0.14";
        const string api_key = "02f7751a55066bcb08e65f4eff134361";
        #endregion

    }
}
