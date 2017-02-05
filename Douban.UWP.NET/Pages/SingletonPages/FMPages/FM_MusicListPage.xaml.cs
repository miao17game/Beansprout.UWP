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

namespace Douban.UWP.NET.Pages.SingletonPages.FMPages {

    public sealed partial class FM_MusicListPage : Page {
        public FM_MusicListPage() {
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
                path: $"{"https://"}api.douban.com/v2/fm/songlist/selections?version=644&start=0&app_name=radio_android&limit=10&apikey={APIKey}",
                host: "api.douban.com",
                reffer: null,
                bearer: bearer,
                userAgt: @"api-client/2.0 com.douban.radio/4.6.4(464) Android/18 TCL_P306C TCL TCL-306C");
            var list = GroupsInit(result);
            ListResources.Source = list;
        }

        private IList<FMListGroup> GroupsInit(string value) {
            IList<FMListGroup> list = new List<FMListGroup>();
            try {
                var JO = JObject.Parse("{groups:" + value + "}");
                var groups = JO["groups"];
                if (groups != null && groups.HasValues) {
                    groups.Children().ToList().ForEach(singleton => {
                        var groupItem = CreateDefaultGroupInstance(singleton);
                        var chls = singleton["programmes"];
                        if (chls != null && chls.HasValues) {
                            chls.Children().ToList().ForEach(everyProg => {
                                var item = CreateDefaultProgrammeInstance(everyProg);
                                CreateCreator(item, everyProg["creator"]);
                                groupItem.Programmes.Add(item);
                            });
                        }
                        list.Add(groupItem);
                    });
                }
            } catch { /* Ingore. */ } finally { IncrementalLoadingBorder.SetVisibility(false); }
            return list;
        }

        private static FMListGroup CreateDefaultGroupInstance(JToken singleton) {
            return new FMListGroup {
                Programmes = new List<FMListProgramme>(),
                GroupID = singleton["group_id"].Value<int>(),
                GroupName = singleton["group_name"].Value<string>() != "" ? singleton["group_name"].Value<string>() : "Douban FM",
            };
        }

        private static FMListProgramme CreateDefaultProgrammeInstance(JToken everyProg) {
            return new FMListProgramme {
                IsCollected = everyProg["is_collected"].Value<bool>(),
                Cover = everyProg["cover"].Value<string>(),
                ID = everyProg["id"].Value<int>(),
                Description = everyProg["description"].Value<string>(),
                Title = everyProg["title"].Value<string>(),
                CanPlay= everyProg["can_play"].Value<bool>(),
                CollectedCount = everyProg["collected_count"].Value<int>(),
                CoverType = everyProg["cover_type"].Value<int>(),
                Duration = everyProg["duration"].Value<int>(),
                IsPublic = everyProg["is_public"].Value<bool>(),
                RecommandReason = everyProg["rec_reason"].Value<string>(),
                ShowNotPlayable = everyProg["show_not_playable"].Value<bool>(),
                SongsCount = everyProg["songs_count"].Value<int>(),
                Type = everyProg["type"].Value<int>(),
                UpdateTime = everyProg["updated_time"].Value<string>(),
            };
        }

        private static void CreateCreator(FMListProgramme item, JToken creator) {
            if (creator.HasValues)
                item.Creator = new FMListCreater {
                    ID = creator["id"].Value<string>(),
                    Name = creator["name"].Value<string>(),
                    Type = creator["type"].Value<string>(),
                    Picture = creator["picture"].Value<string>(),
                    SonglistsCount = creator["songlists_count"].Value<int>(),
                    Url = creator["url"].Value<string>(),
                };
        }

        #region Properties
        string uid;
        string bearer;
        #endregion

        private void GridView_ItemClick(object sender, ItemClickEventArgs e) {
            var item = e.ClickedItem as FMListProgramme;
            if (item == null)
                return;
            NavigateToBase?.Invoke(
                null, 
                new NavigateParameter { ID = item.ID, FrameType = FrameType.Content}, 
                GetFrameInstance(FrameType.Content),
                GetPageType(NavigateType.FM_MHzSongList));
        }

    }
}
