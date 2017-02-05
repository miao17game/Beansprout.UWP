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
            scroll.ViewChanged += ScrollViewerChangedForSongHead;
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e) {
            ReportHelper.ReportAttentionAsync(GetUIString("StillInDeveloping"));
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e) {
            ReportHelper.ReportAttentionAsync(GetUIString("StillInDeveloping"));
        }

        private void LikeButton_Click(object sender, RoutedEventArgs e) {
            ReportHelper.ReportAttentionAsync(GetUIString("StillInDeveloping"));
        }

        private void AddButton_Click(object sender, RoutedEventArgs e) {
            ReportHelper.ReportAttentionAsync(GetUIString("StillInDeveloping"));
        }

        private void IndexList_ItemClick(object sender, ItemClickEventArgs e) {
            var item = e.ClickedItem as MHzSong;
            if (item == null)
                return;
            Service.InsertMusicItem(MusicServiceHelper.CreatePlayItem(
                url: item.Url,
                img: item.Picture,
                title: item.Title,
                artist: item.Artist,
                albumTitle: item.AlbumTitle,
                albunmArtist: item.SingerShow), index: 0);
            Service.Player.Play();
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
                    new KeyValuePair<string, string>( "kbps", "64" ),
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
                ListResources.Source = song_list.Songs;
                SetListHeader(song_list);
            } catch { } finally { IncrementalLoadingBorder.SetVisibility(false); }
        }

        private void SetListHeader(Core.Models.FMModels.MHzSongListModels.MHzSongList song_list) {
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

        private async void ScrollViewerChangedForSongHead(object sender, ScrollViewerViewChangedEventArgs e) {
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

        #endregion

        #region Properties
        string uid;
        string bearer;
        FrameType frameType;

        #endregion

    }
}
