using static Wallace.UWP.Helpers.Tools.UWPStates;
using static Douban.UWP.NET.Resources.AppResources;

using Douban.UWP.Core.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Douban.UWP.Core.Models;
using Douban.UWP.Core.Models.FMModels;
using Douban.UWP.NET.Controls;
using Douban.UWP.NET.Tools;
using System.Threading.Tasks;
using Douban.UWP.Core.Models.FMModels.MHzSongListModels;
using System.Collections.ObjectModel;
using Windows.UI;
using Windows.Web.Http;
using Wallace.UWP.Helpers;

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
            is_daily = args.IsDailyList;
            frameType = args.FrameType;
            listid = Convert.ToInt64(args.ID);
            app_did = new Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation().Id;
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

        private async void DownLoadAllSongList_ClickAsync(object sender, RoutedEventArgs e) {
            ReportHelper.ReportAttentionAsync(GetUIString("Download_Start"));
            foreach(var item in inner_list) {
                await Task.Run(async () => {
                    await Dispatcher.UpdateUI(()=>Downloader.DownloadMusicNotWaitAsync(item, DownloadNotifType.Null, DownloadReportType.ShowListButNotReport));
                    await Task.Delay(10);
                });
            }
        }

        private async void DownloadButton_ClickAsync(object sender, RoutedEventArgs e) {
            var result = await Downloader.DownloadMusicAsync(
                song: ((sender as Button).CommandParameter as MHzSong), 
                notif_type: DownloadNotifType.SuccessfullyNotification,
                report_type: DownloadReportType.ShowListAndReportStart);
            DownloadHelper.ReportByDownloadResoult(result);
            ChangeDownloadStatus((sender as Button), result == DownloadResult.Successfully ? true : false);
        }

        private async void LikeButton_ClickAsync(object sender, RoutedEventArgs e) {
            var item = (sender as Button).CommandParameter as MHzSongBase;
            if (item == null)
                return;
            var succeed = await MHzListGroupHelper.AddToRedHeartAsync(item, listid, APIKey, bearer, app_did, item.LikeCount == 0);
            if (succeed) {
                if(item.LikeCount == 0) {
                    item.LikeCount = 1;
                    ReportHelper.ReportAttentionAsync(GetUIString("Red_Heart_Added"));
                    (sender as Button).Foreground = new SolidColorBrush(Colors.Red);
                } else {
                    item.LikeCount = 0;
                    ReportHelper.ReportAttentionAsync(GetUIString("Red_Heart_Removed"));
                    (sender as Button).Foreground = new SolidColorBrush(Color.FromArgb(128, 128, 128, 128));
                }
            }
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
            var result = is_daily?
                await DoubanWebProcess.GetMDoubanResponseAsync(
                    path: $"{"https://"}api.douban.com/v2/fm/songlist/user_daily/?version=644&app_name=radio_android&kbps=192&apikey={APIKey}",
                    host: "api.douban.com",
                    reffer: null,
                    bearer: bearer,
                    userAgt: @"api-client/2.0 com.douban.radio/4.6.4(464) Android/18 TCL_P306C TCL TCL-306C"):
                await DoubanWebProcess.PostDoubanResponseAsync(
                    path: $"{"https://"}api.douban.com/v2/fm/songlist/{list_id}/detail",
                    host: "api.douban.com",
                    reffer: null,
                    userAgent: @"api-client/2.0 com.douban.radio/4.6.4(464) Android/18 TCL_P306C TCL TCL-306C",
                    content: new HttpFormUrlEncodedContent(new List<KeyValuePair<string, string>>{
                        new KeyValuePair<string, string>( "version", "644" ),
                        new KeyValuePair<string, string>( "kbps", "320" ),
                        new KeyValuePair<string, string>( "app_name", "radio_android" ),
                        new KeyValuePair<string, string>( "apikey", APIKey ),
                    }), bearer: bearer);
            try {
                var song_list = JsonHelper.FromJson<MHzSongList>(result);
                if (song_list.Cover == "")
                    song_list.Cover = "ms-appx:///Assets/231.jpg";
                inner_list = new ObservableCollection<MHzSong>();
                song_list.Songs.ToList().ForEach(i => inner_list.Add(i));
                var query = await StorageHelper.GetAllStorageFilesByExtensionAsync(StorageHelper.JsonExtension);
                foreach(var item in inner_list) {
                    item.IsCached = StorageHelper.IsExistLocalJsonBySHA256(MHzSongBaseHelper.GetIdentity(item), query);
                }
                ListResources.Source = inner_list;
                SetListHeader(song_list);
            } catch { /* Ignore */ } finally { IncrementalLoadingBorder.SetVisibility(false); }
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
            button.Click -= DownloadButton_ClickAsync;
        }

        #endregion

        #region Properties

        const string udid = "8841d56cb5b24b808de35702b112d75ec6fdcf24";
        const string did = "d0166bef2e066290987be5eb123cd2a0080fb654";

        bool is_daily;
        long listid;
        string uid;
        string bearer;
        Guid app_did;
        FrameType frameType;
        ObservableCollection<MHzSong> inner_list;

        #endregion

    }
}
