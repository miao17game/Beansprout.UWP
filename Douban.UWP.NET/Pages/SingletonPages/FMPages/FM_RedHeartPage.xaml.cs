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
using Douban.UWP.Core.Models.FMModels;
using Douban.UWP.NET.Tools;
using Douban.UWP.Core.Models;
using Douban.UWP.Core.Models.FMModels.MHzSongListModels;
using Douban.UWP.Core.Tools;
using System.Threading.Tasks;
using Wallace.UWP.Helpers;
using Douban.UWP.Core.Models.FMModels.ReadHeartModels;
using Newtonsoft.Json.Linq;
using Windows.Web.Http;
using System.Collections.ObjectModel;

namespace Douban.UWP.NET.Pages.SingletonPages.FMPages {

    public sealed partial class FM_RedHeartPage : Page {
        public FM_RedHeartPage() {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            bearer = AccessToken;
            app_did = new Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation().Id;

            try {

                string red_resource = await GetRedHeartBasicAsync();
                if (red_resource == null)
                    return;

                var red_list = JsonHelper.FromJson<RedHeartList>(red_resource);
                if (red_list == null || red_list.Songs == null || red_list.Songs.Count == 0)
                    return;

                var result_songs = await GetSongDetailsAsync(string.Join("|", red_list.Songs.Select(i => i.SID)));
                if (result_songs == null)
                    return;

                obse_list = JsonHelper.FromJson<ObservableCollection<SongBase>>(result_songs);
                ListResources.Source = obse_list;

            } catch {
                System.Diagnostics.Debug.WriteLine("fetch red-heart list error.");
            } finally { IncrementalLoadingBorder.SetVisibility(false); }
        }

        private static async Task<string> GetRedHeartBasicAsync() {
            try {
                return await DoubanWebProcess.GetMDoubanResponseAsync(
                    path: $"{"https://"}api.douban.com/v2/fm/redheart/basic?version=644&app_name=radio_android&apikey={APIKey}",
                    host: "api.douban.com",
                    reffer: null,
                    bearer: AccessToken,
                    userAgt: @"api-client/2.0 com.douban.radio/4.6.4(464) Android/18 TCL_P306C TCL TCL-306C");
            } catch {
                System.Diagnostics.Debug.WriteLine("fetch red-heart basic error.");
                return null;
            }
        }

        private static async Task<string> GetSongDetailsAsync(string sids) {
            try {
                return await DoubanWebProcess.PostDoubanResponseAsync(
                    path: $"{"https://"}api.douban.com/v2/fm/redheart/songs?version=644&app_name=radio_android&apikey={APIKey}",
                    host: "api.douban.com",
                    reffer: null,
                    userAgent: @"api-client/2.0 com.douban.radio/4.6.4(464) Android/18 TCL_P306C TCL TCL-306C",
                    bearer: AccessToken,
                    content: new HttpFormUrlEncodedContent(new List<KeyValuePair<string, string>>{
                        new KeyValuePair<string, string>( "sids", sids ),
                        new KeyValuePair<string, string>( "kbps", "192" ),
                    }));
            } catch {
                System.Diagnostics.Debug.WriteLine("fetch red-heart detials error.");
                return null;
            }
        }

        private void IndexList_Loaded(object sender, RoutedEventArgs e) {

        }

        private async void IndexList_ItemClickAsync(object sender, ItemClickEventArgs e) {
            var item = e.ClickedItem as SongBase;
            if (item == null)
                return;
            var succeedss = Service.ChangeServiceChoice(MusicServiceType.SongList);
            if (!succeedss)
                return;
            var song = new MHzSongBase(item);
            var succeed = await Service.InsertItemAsync(song);
            if (!succeed)
                return;
            if (MainUpContentFrame.Content != null)
                (MainUpContentFrame.Content as FM_SongBoardPage)?.UnregisterServiceEvents();
            Service.SongListMoveTo();
            NavigateToBase?.Invoke(
                null,
                new MusicBoardParameter {
                    SID = item.SID,
                    SSID = item.SSID,
                    AID = item.AID,
                    SHA256 = item.SHA256,
                    Url = item.Url,
                    Song = song,
                    FrameType = FrameType.UpContent
                },
                GetFrameInstance(FrameType.UpContent),
                GetPageType(NavigateType.MusicBoard));
        }

        private async void AddButton_ClickAsync(object sender, RoutedEventArgs e) {
            var item = (sender as Button).CommandParameter as SongBase;
            if (item == null)
                return;
            var song = new MHzSongBase(item);
            var succeed = await Service.InsertItemAsync(song);
            if (succeed)
                ReportHelper.ReportAttentionAsync(GetUIString("Music_added"));
        }

        private async void DeleteButton_ClickAsync(object sender, RoutedEventArgs e) {
            var item = (sender as Button).CommandParameter as SongBase;
            if (item == null)
                return;
            var succeed = await MHzListGroupHelper.AddToRedHeartAsync(item, listid, APIKey, bearer, app_did, false);
            if (!succeed)
                return;
            obse_list.Remove(item);
            ReportHelper.ReportAttentionAsync(GetUIString("Red_Heart_Removed"));
        }

        #region
        const string udid = "8841d56cb5b24b808de35702b112d75ec6fdcf24";
        const string did = "d0166bef2e066290987be5eb123cd2a0080fb654";

        long listid = 0;
        string uid;
        string bearer;
        Guid app_did;
        ObservableCollection<SongBase> obse_list;
        #endregion

    }
}
