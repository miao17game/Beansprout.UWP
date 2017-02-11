using static Wallace.UWP.Helpers.Tools.UWPStates;
using static Douban.UWP.NET.Resources.AppResources;

using Douban.UWP.Core.Tools;
using Newtonsoft.Json.Linq;
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
using Douban.UWP.Core.Models;
using Douban.UWP.Core.Models.FMModels;
using Wallace.UWP.Helpers;
using Douban.UWP.NET.Controls;
using Douban.UWP.NET.Tools;
using System.Threading.Tasks;
using Douban.UWP.Core.Models.FMModels.MHzSongListModels;
using Windows.Media.Playback;
using Windows.Media.Core;
using System.Collections.ObjectModel;
using Windows.UI;

namespace Douban.UWP.NET.Pages.SingletonPages.FMPages {

    public sealed partial class FM_MHzSongListPage : BaseContentPage {
        public FM_MHzSongListPage() {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            uid = UserID ?? "";
            bearer = AccessToken ?? "";
            navigateTitlePath.Text = GetUIString("MHzList_Content");
            var args = e.Parameter as NavigateParameter;
            if (args == null)
                return;
            frameType = args.FrameType;
            await InitListResourcesAsync(args.ID);
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e) {
            GlobalHelpers.SetChildPageMargin(this, matchNumber: VisibleWidth, isDivideScreen: IsDivideScreen);
        }

        private void BaseHamburgerButton_Click(object sender, RoutedEventArgs e) {
            PageSlideOutStart(VisibleWidth > FormatNumber ? false : true);
        }

        private void IndexList_Loaded(object sender, RoutedEventArgs e) {
            var scroll = GlobalHelpers.GetScrollViewer(IndexList);
            scroll.ViewChanged += ScrollViewerChangedForSongHeadAsync;
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e) {
            ReportHelper.ReportAttentionAsync(GetUIString("StillInDeveloping"));
        }

        private async void DownloadButton_ClickAsync(object sender, RoutedEventArgs e) {
            var result = await Downloader.DownloadMusicAsync(((sender as Button).CommandParameter as MHzSong), false);
            DownloadHelper.ReportByDownloadResoult(result);
            ChangeDownloadStatus((sender as Button), result == DownloadResult.Successfully ? true : false);
        }

        private void LikeButton_Click(object sender, RoutedEventArgs e) {
            ReportHelper.ReportAttentionAsync(GetUIString("StillInDeveloping"));
        }

        private async void AddButton_ClickAsync(object sender, RoutedEventArgs e) {
            var item = (sender as Button).CommandParameter as MHzSong;
            if (item == null)
                return;
            var succeed = await Service.InsertItemAsync(item);
            if (succeed)
                ReportHelper.ReportAttentionAsync(GetUIString("Music_added"));
        }

        private async void PlayAllBtn_ClickAsync(object sender, RoutedEventArgs e) {
            if (inner_list == null)
                return;
            var item = inner_list[0];
            var succeedss = Service.ChangeServiceChoice(MusicServiceType.SongList);
            if (!succeedss)
                return;
            var succeed = false;
            succeed = await Service.InsertItemAsync(item);
            if (!succeed)
                return;
            inner_list.ToList().ForEach(async i => succeed = await Service.InsertItemAsync(i));
            if (MainUpContentFrame.Content != null)
                (MainUpContentFrame.Content as FM_SongBoardPage)?.UnregisterServiceEvents();
            Service.SongListMoveTo(item);
            NavigateToBase?.Invoke(
                null,
                new MusicBoardParameter {
                    SID = item.SID,
                    SSID = item.SSID,
                    AID = item.AID,
                    SHA256 = item.SHA256,
                    Url = item.Url,
                    Song = item,
                    FrameType = FrameType.UpContent
                },
                GetFrameInstance(FrameType.UpContent),
                GetPageType(NavigateType.MusicBoard));
        }

        private async void IndexList_ItemClickAsync(object sender, ItemClickEventArgs e) {
            var item = e.ClickedItem as MHzSong;
            if (item == null)
                return;
            var succeedss = Service.ChangeServiceChoice(MusicServiceType.SongList);
            if (!succeedss)
                return;
            var succeed = await Service.InsertItemAsync(item);
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
                    Song = item,
                    FrameType = FrameType.UpContent
                },
                GetFrameInstance(FrameType.UpContent),
                GetPageType(NavigateType.MusicBoard));
        }

        #region Method

        protected override void InitPageState() {
            base.InitPageState();
            GlobalHelpers.DivideWindowRange(this, DivideNumber, isDivideScreen: IsDivideScreen);
        }

        public override void DoWorkWhenAnimationCompleted() {
            if (VisibleWidth > FormatNumber && IsDivideScreen && MainMetroFrame.Content == null)
                MainMetroFrame.Navigate(typeof(MetroPage));
            GetFrameInstance(frameType).Content = null;
        }

        private async Task InitListResourcesAsync(int list_id) {
            var result = await DoubanWebProcess.PostDoubanResponseAsync(
                $"{"https://"}api.douban.com/v2/fm/songlist/{list_id}/detail",
                "api.douban.com",
                null,
                userAgent: @"api-client/2.0 com.douban.radio/4.6.4(464) Android/18 TCL_P306C TCL TCL-306C",
                content: new Windows.Web.Http.HttpFormUrlEncodedContent(new List<KeyValuePair<string, string>>{
                    new KeyValuePair<string, string>( "version", "644" ),
                    new KeyValuePair<string, string>( "kbps", "320" ),
                    new KeyValuePair<string, string>( "app_name", "radio_android" ),
                    new KeyValuePair<string, string>( "apikey", APIKey ),
                }));
            try {
                var jo = JObject.Parse(result);
                var songs = jo["songs"];
                var creator = jo["creator"];
                var song_list = MHzSongListHelper.CreateDefaultSongList(jo);
                MHzSongListHelper.AddCreatorMessage(song_list, jo["creator"]);
                if (songs != null && songs.HasValues) {
                    songs.Children().ToList().ForEach(jo_song => {
                        try {
                            var song = MHzSongListHelper.CreateDefaultSongInstance(jo_song);
                            MHzSongListHelper.AddSingerEachOne(song, jo_song["singers"]);
                            MHzSongListHelper.AddItemInfo(song, jo_song["item_info"]);
                            song_list.Songs.Add(song);
                        } catch { /* Ingore */ }
                    });
                }
                inner_list = new ObservableCollection<MHzSong>();
                song_list.Songs.ToList().ForEach(i => inner_list.Add(i));
                var query = await StorageHelper.GetAllStorageFilesByExtensionAsync(StorageHelper.JsonExtension);
                foreach(var item in inner_list) {
                    item.IsCached = StorageHelper.IsExistLocalJsonBySHA256(MHzSongBaseHelper.GetIdentity(item), query);
                }
                ListResources.Source = inner_list;
                SetListHeader(song_list);
            } catch { } finally { IncrementalLoadingBorder.SetVisibility(false); }
        }

        private void SetListHeader(MHzSongList song_list) {
            try {
                var succeed = Uri.TryCreate(song_list.Cover, UriKind.Absolute, out var img_cover);
                TitleBackImage.Source = succeed ? new Windows.UI.Xaml.Media.Imaging.BitmapImage(img_cover) : null;
                SongListImage.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(succeed ? img_cover : new Uri("ms-appx:///Assets/star006.png"));
                SongListTitle.Text = song_list.Title;
                SongListCount.Text = song_list.Count.ToString() + GetUIString("MusicCount_Tail");
                CollectedCount.Text = song_list.CollectedCount.ToString() + GetUIString("Collect_Tail");
                var succeed2 = Uri.TryCreate(song_list.Creator?.Picture, UriKind.Absolute, out var img_cover2);
                CreatorImage.ImageSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(succeed2 ? img_cover2 : new Uri("ms-appx:///Assets/star006.png"));
                CreatorName.Text = song_list.Creator?.Name;
            } catch { /* Ingore */ }
        }

        private async void ScrollViewerChangedForSongHeadAsync(object sender, ScrollViewerViewChangedEventArgs e) {
            var scroll = sender as ScrollViewer;
            try {
                if (scroll.VerticalOffset <= 30)
                    ShadowRect.Opacity = BackRec.Opacity = (scroll.VerticalOffset) / 30;
                else if (BackRec.Opacity < 1)
                    ShadowRect.Opacity = BackRec.Opacity = 1;
                await Task.Delay(5);
                if (scroll.VerticalOffset <= 160)
                    TitleHeaderGrid.Margin = new Thickness(0, -(sender as ScrollViewer).VerticalOffset, 0, 0);
                else
                    TitleHeaderGrid.Margin = new Thickness(0, -160, 0, 0);
            } catch { System.Diagnostics.Debug.WriteLine("Save scroll positions error."); }
        }

        private void ChangeDownloadStatus(Button button, bool is_ok) {
            if (!is_ok)
                return;
            button.Content = char.ConvertFromUtf32(0xE10B);
            button.Foreground = Application.Current.Resources["DoubanForeground"] as SolidColorBrush;
        }

        #endregion

        #region Properties
        string uid;
        string bearer;
        FrameType frameType;
        ObservableCollection<MHzSong> inner_list;

        #endregion

    }
}
