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
using Windows.Media.Playback;
using Douban.UWP.NET.Models;
using Windows.UI;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Core;

namespace Douban.UWP.NET.Pages.SingletonPages.FMPages {

    public sealed partial class FM_SongBoardPage : BaseContentPage {
        public FM_SongBoardPage() {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            var args = e.Parameter as MusicBoardParameter;
            if (args == null)
                return;
            frameType = args.FrameType;
            var num = Service.GetVolumn();
            RadialGauge.Value = 100 * num;
            SetCenterControlGridRotation(num);
            await InitMusicBoardAsync(args);
        }

        private async Task InitMusicBoardAsync(MusicBoardParameter args) {
            try {
                sid = args.SID;
                ssid = args.SSID;
                aid = args.AID;
                path_url = args.Url;
                identity_song = MHzSongBaseHelper.GetIdentity(args);
                THIS_SONG = args.Song;
                var result = await DoubanWebProcess.GetMDoubanResponseAsync(
                    path: $"{"https://"}api.douban.com/v2/fm/song/{sid + "g" + ssid}/?version=644&start=0&app_name=radio_android&apikey={APIKey}",
                    host: "api.douban.com",
                    reffer: null,
                    bearer: AccessToken,
                    userAgt: @"api-client/2.0 com.douban.radio/4.6.4(464) Android/18 TCL_P306C TCL TCL-306C");

                var jo = JObject.Parse(result);
                title = jo["title"].Value<string>();
                artist = jo["artist"].Value<string>();
                image = jo["related_channel"]["cover"].Value<string>();

                UnregisterServiceEvents();
                RegisterServiceEvents();

                MusicSlider.ValueChanged -= MusicSlider_ValueChanged;
                MusicBoardVM.CurrentTime = Service.Session.Position;
                MusicBoardVM.Duration = Service.Session.NaturalDuration;
                MusicSlider.ValueChanged += MusicSlider_ValueChanged;

                MusicBoardVM.BackImage = image;
                MusicBoardVM.LrcTitle = title;
                MusicBoardVM.ListCount = Service.CurrentSongList.Count;
                MusicBoardVM.CurrentItem = Service.CurrentItem;

                RandomButton.Content =
                    Service.PlayType == MusicServicePlayType.ShufflePlay ? char.ConvertFromUtf32(0xE8B1) :
                    Service.PlayType == MusicServicePlayType.AutoRepeat ? char.ConvertFromUtf32(0xE8EE) :
                    Service.PlayType == MusicServicePlayType.SingletonPlay ? char.ConvertFromUtf32(0xE8ED) :
                    char.ConvertFromUtf32(0xE84F);

                SongListButton.Content = Service.ServiceType == MusicServiceType.SongList ?
                    char.ConvertFromUtf32(0xE142) :
                    char.ConvertFromUtf32(0xE93E);

                if (Service.Session.PlaybackState == MediaPlaybackState.Playing)
                    DoWorkWhenMusicPlayed();
                else if (Service.Session.PlaybackState == MediaPlaybackState.Paused)
                    DoWorkWhenMusicPaused();

                MusicBoardVM.LrcList = null;
                ChangeDownloadStatus(is_cached = await StorageHelper.IsExistLocalJsonBySHA256Async(MHzSongBaseHelper.GetIdentity(args)));
                songMessCollection = (await LrcProcessHelper.GetSongMessageListAsync(title, artist)) ?? new List<LrcMetaData>();
            } catch {
                ReportHelper.ReportAttentionAsync(GetUIString("MediaSource_EEEEEEEError"));
            } finally { await SetDefaultLrcAndAnimationsAsync(); }

        }

        private async void OnBufferingEndedAsync(MediaPlaybackSession sender, object args) {
            await Dispatcher.UpdateUI(() => {
                PlayPauseButton.Content = char.ConvertFromUtf32(0xE769);
                MusicBoardVM.CurrentTime = Service.Session.Position;
                MusicBoardVM.Duration = Service.Session.NaturalDuration;
            });
        }

        private async void OnPlaybackStateChangedAsync(MediaPlaybackSession sender, object args) {
            var status = Service.Session.PlaybackState;
            if (status == MediaPlaybackState.Playing)
                await Dispatcher.UpdateUI(() => DoWorkWhenMusicPlayed());
            else if (status == MediaPlaybackState.Paused)
                await Dispatcher.UpdateUI(() => DoWorkWhenMusicPaused());
            else if (status == MediaPlaybackState.Buffering) { /* SHOULD SHOW BUFFERING MESSAGE. */ }
        }

        private async void OnMediaEndedAsync(MediaPlayer sender, object args) {
            await Dispatcher.UpdateUI(() => {
                LrcListView.Timer?.Stop();
                PlayPauseButton.Content = char.ConvertFromUtf32(0xE768);
            });
        }

        private async void OnCurrentItemChangedAsync(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args) {
            var newItem = args.NewItem;
            if (newItem == null)
                return;
            var arg = newItem.Source.CustomProperties["Message"] as MusicBoardParameter;
            if (arg == null)
                return;
            await Dispatcher.UpdateUI(async () => await InitMusicBoardAsync(arg));
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e) {
            GlobalHelpers.SetChildPageMargin(this, matchNumber: VisibleWidth, isDivideScreen: IsDivideScreen);
        }

        private void BaseHamburgerButton_Click(object sender, RoutedEventArgs e) {
            PageSlideOutStart(VisibleWidth > FormatNumber ? false : true);
        }

        private void LrcCanvas_SizeChanged(object sender, SizeChangedEventArgs e) {
            LrcListView.Width = LrcCanvas.ActualWidth;
        }

        private void LrcListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {

        }

        private void LrcCanvas_Loaded(object sender, RoutedEventArgs e) {
            
        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e) {
            if (Service.Session.PlaybackState == MediaPlaybackState.Playing) {
                Service.PauseAnyway();
            } else if (Service.Session.PlaybackState == MediaPlaybackState.Paused) {
                Service.PlayAnyway();
                if (LrcListView.Timer != null && !LrcListView.Timer.IsEnabled) {
                    (LrcListView.RenderTransform as CompositeTransform).TranslateY = 0;
                    LrcListView.Timer.Start();
                }
            }
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e) {
            Service.MovePreviousAnyway();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e) {
            Service.MoveNextAnyway();
        }

        private void SongListButton_Click(object sender, RoutedEventArgs e) {
            OpenInnerContent();
            ContentFrame.Navigate(typeof(FM_PlayListPage));
        }

        private void RandomButton_Click(object sender, RoutedEventArgs e) {
            if (Service.ServiceType == MusicServiceType.MHz)
                return;
            var type = Service.ChangePlayStyle();
            RandomButton.Content =
                type == MusicServicePlayType.ShufflePlay ? char.ConvertFromUtf32(0xE8B1) :
                type == MusicServicePlayType.AutoRepeat ? char.ConvertFromUtf32(0xE8EE) :
                type == MusicServicePlayType.SingletonPlay ? char.ConvertFromUtf32(0xE8ED) :
                char.ConvertFromUtf32(0xE84F);
            SettingsHelper.SaveSettingsValue(SettingsSelect.BackPlayType, type.ToString());
        }

        private void MusicSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e) {
            if (Math.Abs(e.NewValue - e.OldValue) >= 3)
                Service.Session.Position = TimeSpan.FromSeconds(e.NewValue);
        }

        private void LrcButton_Click(object sender, RoutedEventArgs e) {
            OpenInnerContent();
            ContentFrame.Navigate(typeof(FM_LrcChoosePage), songMessCollection);
        }

        private void LikeButton_Click(object sender, RoutedEventArgs e) {
            ReportHelper.ReportAttentionAsync(GetUIString("StillInDeveloping"));
        }

        private async void DownloadButton_ClickAsync(object sender, RoutedEventArgs e) {
            if (is_cached)
                return;
            var identity_song_check = identity_song;
            var result = await Downloader.DownloadMusicAsync(THIS_SONG);
            DownloadHelper.ReportByDownloadResoult(result);
            if (identity_song_check == identity_song)
                ChangeDownloadStatus(result == DownloadResult.Successfully ? true : false);
        }

        private void FlowButton_Click(object sender, RoutedEventArgs e) {
            ReportHelper.ReportAttentionAsync(GetUIString("StillInDeveloping"));
        }

        private void CDClickButton_Click(object sender, RoutedEventArgs e) {
            ResetCanvasViewVisibility();
        }

        private void LrcListView_ItemClick(object sender, ItemClickEventArgs e) {
            ResetCanvasViewVisibility();
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e) {
            CDGrid.SetVisibility(true);
            LrcCanvas.SetVisibility(false);
        }

        private void VolumeShowBtn_Click(object sender, RoutedEventArgs e) {
            UnitsBorder.SetVisibility(true);
        }

        private void VolumeCloseBtn_Click(object sender, RoutedEventArgs e) {
            var num = RadialGauge.Value / 100;
            var succeed = Service.ChangeVolumnTo(num);
            if (succeed) {
                SettingsHelper.SaveSettingsValue(SettingsSelect.MusicServiceVolumn, num);
                SetCenterControlGridRotation(num);
            }
            UnitsBorder.SetVisibility(false);
        }

        private void InnerContentPanel_SizeChanged(object sender, SizeChangedEventArgs e) {
            InnerGrid.Width = (sender as Popup).ActualWidth;
            InnerGrid.Height = (sender as Popup).ActualHeight;
        }

        private void CloseAllComsBtn_Click(object sender, RoutedEventArgs e) {
            CloseInnerContentPanel();
        }

        private void InnerContentPanel_Closed(object sender, object e) {
            OutPopupBorder.Completed += OnOutPopupBorderOut;
            OutPopupBorder.Begin();
        }

        private void OnOutPopupBorderOut(object sender, object e) {
            OutPopupBorder.Completed -= OnOutPopupBorderOut;
            PopupBackBorder.SetVisibility(false);
        }

        private void CenterControlGrid_SizeChanged(object sender, SizeChangedEventArgs e) {
            var comp = CenterControlGrid.RenderTransform as CompositeTransform;
            if (comp == null)
                CenterControlGrid.RenderTransform = comp = new CompositeTransform();
            CenterControlGrid.RenderTransform = comp;
            comp.CenterX = CenterControlGrid.ActualWidth / 2;
            comp.CenterY = CenterControlGrid.ActualHeight / 2;
        }

        #region Method

        protected override void InitPageState() {
            base.InitPageState();
            GlobalHelpers.DivideWindowRange(this, DivideNumber, isDivideScreen: IsDivideScreen);
        }

        public override void DoWorkWhenAnimationCompleted() {
            if (VisibleWidth > FormatNumber && IsDivideScreen && MainMetroFrame.Content == null)
                MainMetroFrame.Navigate(typeof(MetroPage));
            LrcListView.Timer?.Stop();
            if (LrcListView.Timer != null)
                LrcListView.Timer.Tick -= LrcListView.DispatcherTimerEventAsync;
            UnregisterServiceEvents();
            GetFrameInstance(frameType).Content = null;
        }

        public void RegisterServiceEvents() {
            Service.PlaybackList.CurrentItemChanged += OnCurrentItemChangedAsync;
            Service.Session.BufferingEnded += OnBufferingEndedAsync;
            Service.Session.PlaybackStateChanged += OnPlaybackStateChangedAsync;
            Service.BindingOnMediaEnded(OnMediaEndedAsync, true);
        }

        public void UnregisterServiceEvents() {
            Service.PlaybackList.CurrentItemChanged -= OnCurrentItemChangedAsync;
            Service.Session.BufferingEnded -= OnBufferingEndedAsync;
            Service.Session.PlaybackStateChanged -= OnPlaybackStateChangedAsync;
            Service.BindingOnMediaEnded(OnMediaEndedAsync, false);
        }

        public void OpenInnerContent() {
            InnerContentPanel.IsOpen = true;
            PopupBackBorder.SetVisibility(true);
            EnterPopupBorder.Begin();
        }

        public void DoWorkWhenMusicPaused() {
            PlayPauseButton.Content = char.ConvertFromUtf32(0xE768);
            RollStory?.AnimationStop();
        }

        public void DoWorkWhenMusicPlayed() {
            PlayPauseButton.Content = char.ConvertFromUtf32(0xE769);
            RollStory?.AnimationPlay();
        }

        public void CloseInnerContentPanel() {
            InnerContentPanel.IsOpen = false;
        }

        public void ResetCanvasViewVisibility() {
            if (LrcCanvas.Visibility == Visibility.Collapsed) {
                CDGrid.SetVisibility(false);
                LrcCanvas.SetVisibility(true);
            } else {
                CDGrid.SetVisibility(true);
                LrcCanvas.SetVisibility(false);
            }
        }

        public async Task SetDefaultLrcAndAnimationsAsync(bool is_default_lrc = true) { 
            if (is_default_lrc) {
                var local_lrc = await StorageHelper.FetchLocalLrcBySHA256Async(identity_song);
                if (local_lrc != null) {
                    MusicBoardVM.LrcList = local_lrc;
                } else if (songMessCollection != null && songMessCollection.Count() > 0) {
                    var lrc = await LrcProcessHelper.FetchLrcByIdAsync(songMessCollection.ToArray()[0].ID);
                    MusicBoardVM.LrcList = await LrcProcessHelper.ReadLRCFromWebAsync(title, artist, Colors.White, lrc);
                }
            }
            LrcListView.SetLrcAnimation(
                list: MusicBoardVM.LrcList ?? new List<LrcInfo>(), 
                vm: ref MusicBoardVM,
                anima_style: animation_style);
            if (MusicBoardVM.LrcList != null && identity_song != null) {
                var succeed = await Downloader.CreateBLRCAsync(identity_song, MusicBoardVM.LrcList);
            }
        }

        private void SetCenterControlGridRotation(double num) {
            var comp = CenterControlGrid.RenderTransform as CompositeTransform;
            if (comp == null)
                CenterControlGrid.RenderTransform = comp = new CompositeTransform();
            CenterControlGrid.RenderTransform = comp;
            comp.CenterX = CenterControlGrid.ActualWidth / 2;
            comp.CenterY = CenterControlGrid.ActualHeight / 2;
            comp.Rotation = num * 360;
        }

        private void ChangeDownloadStatus(bool is_ok) {
            DownloadButton.Content = is_ok ? char.ConvertFromUtf32(0xE10B) : char.ConvertFromUtf32(0xE896);
            DownloadButton.Foreground = is_ok ? Application.Current.Resources["DoubanForeground"] as SolidColorBrush : new SolidColorBrush(Colors.White);
        }

        #endregion

        #region Properties

        string sid;
        string ssid;
        string aid;
        string title;
        string artist;
        string image;
        FrameType frameType;
        IEnumerable<LrcMetaData> songMessCollection;
        LrcListViewAnimationStyle animation_style = LrcListViewAnimationStyle.FastSlide;
        bool is_cached;
        string identity_song;
        string path_url;
        MHzSongBase THIS_SONG;

        public VisualBoardVM VMForPublic { get { return MusicBoardVM; } }

        #endregion

    }
}
