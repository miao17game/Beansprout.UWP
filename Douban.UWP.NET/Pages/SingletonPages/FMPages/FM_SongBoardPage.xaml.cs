﻿using static Wallace.UWP.Helpers.Tools.UWPStates;
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
            RegisterServiceEvents();
            await InitMusicBoardAsync(args);
            await SetDefaultLrcAndAnimationsAsync();
        }

        private async Task InitMusicBoardAsync(MusicBoardParameter args) {
            sid = args.SID;
            ssid = args.SSID;
            aid = args.AID;
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

            MusicBoardVM.BackImage = image;
            MusicBoardVM.LrcTitle = title;
            MusicBoardVM.ListCount = Service.ServiceType == MusicServiceType.SongList ? Service.SongList.Count : Service.MHzSongList.Count;

            RandomButton.Content = 
                Service.PlayType == MusicServicePlayType.ShufflePlay? char.ConvertFromUtf32(0xE8B1):
                Service.PlayType == MusicServicePlayType.AutoRepeat ? char.ConvertFromUtf32(0xE8EE) :
                Service.PlayType == MusicServicePlayType.SingletonPlay ? char.ConvertFromUtf32(0xE8ED) :
                char.ConvertFromUtf32(0xE84F);

            MusicBoardVM.CurrentTime = Service.Session.Position;
            MusicBoardVM.Duration = Service.Session.NaturalDuration;

            if (Service.Session.PlaybackState == MediaPlaybackState.Playing)
                DoWorkWhenMusicPlayed();
            else if (Service.Session.PlaybackState == MediaPlaybackState.Paused)
                DoWorkWhenMusicPaused();

            songMessCollection = (await LrcProcessHelper.GetSongMessageListAsync(title, artist)) ?? new List<LrcMetaData>();

            InitCDStoryboard();
        }

        private async void OnBufferingEndedAsync(MediaPlaybackSession sender, object args) {
            await Dispatcher.UpdateUI(() => {
                PlayPauseButton.Content = char.ConvertFromUtf32(0xE769);
                MusicBoardVM.CurrentTime = Service.Session.Position;
                MusicBoardVM.Duration = Service.Session.NaturalDuration;
            });
        }

        private async void OnPlaybackStateChangedAsync(MediaPlaybackSession sender, object args) {
            var status = sender.PlaybackState;
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
            await Dispatcher.UpdateUI(async () => {
                await InitMusicBoardAsync(arg);
                await SetDefaultLrcAndAnimationsAsync();
            });
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
                Service.Player.Pause();
            } else if (Service.Session.PlaybackState == MediaPlaybackState.Paused) {
                Service.Player.Play();
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
            //var newValue = e.NewValue;
            //System.Diagnostics.Debug.WriteLine(newValue);
        }

        private void LrcButton_Click(object sender, RoutedEventArgs e) {
            OpenInnerContent();
            ContentFrame.Navigate(typeof(FM_LrcChoosePage), songMessCollection);
        }

        private void LikeButton_Click(object sender, RoutedEventArgs e) {
            ReportHelper.ReportAttentionAsync(GetUIString("StillInDeveloping"));
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e) {
            ReportHelper.ReportAttentionAsync(GetUIString("StillInDeveloping"));
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

        private void RegisterServiceEvents() {
            Service.PlayList.CurrentItemChanged += OnCurrentItemChangedAsync;
            Service.MHzList.CurrentItemChanged += OnCurrentItemChangedAsync;
            Service.Session.BufferingEnded += OnBufferingEndedAsync;
            Service.Session.PlaybackStateChanged += OnPlaybackStateChangedAsync;
            Service.Player.MediaEnded += OnMediaEndedAsync;
        }

        private void UnregisterServiceEvents() {
            Service.PlayList.CurrentItemChanged -= OnCurrentItemChangedAsync;
            Service.MHzList.CurrentItemChanged -= OnCurrentItemChangedAsync;
            Service.Session.BufferingEnded -= OnBufferingEndedAsync;
            Service.Session.PlaybackStateChanged -= OnPlaybackStateChangedAsync;
            Service.Player.MediaEnded -= OnMediaEndedAsync;
        }

        public void OpenInnerContent() {
            InnerContentPanel.IsOpen = true;
            PopupBackBorder.SetVisibility(true);
            EnterPopupBorder.Begin();
        }

        public void DoWorkWhenMusicPaused() {
            PlayPauseButton.Content = char.ConvertFromUtf32(0xE768);
            cd_sb?.Stop();
        }

        public void DoWorkWhenMusicPlayed() {
            PlayPauseButton.Content = char.ConvertFromUtf32(0xE769);
            cd_sb?.Begin();
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
            if (songMessCollection.Count() == 0)
                return;
            if (is_default_lrc) {
                var lrc = await LrcProcessHelper.FetchLrcByIdAsync(songMessCollection.ToArray()[0].ID);
                MusicBoardVM.LrcList = await LrcProcessHelper.ReadLRCFromWebAsync(title, artist, Colors.White, lrc);
            }
            LrcListView.SetLrcAnimation(
                list: MusicBoardVM.LrcList ?? new List<LrcInfo>(), 
                vm: ref MusicBoardVM,
                anima_style: animation_style);
        }

        #region CD Animations

        private void InitCDStoryboard() {
            if (cd_sb != null)
                return;
            var comp = CDImage.RenderTransform as CompositeTransform;
            if (comp == null)
                CDImage.RenderTransform = comp = new CompositeTransform();
            var Translate = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetProperty(Translate, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.Rotation)").Path);
            Storyboard.SetTarget(Translate, CDImage);
            Translate.KeyFrames.Add(new EasingDoubleKeyFrame {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(20)),
                Value = 360,
            });
            comp.CenterX = CDImage.ActualWidth / 2;
            comp.CenterY = CDImage.ActualHeight / 2;
            cd_sb = new Storyboard();
            cd_sb.Children.Add(Translate);
            cd_sb.RepeatBehavior = new RepeatBehavior { Type = RepeatBehaviorType.Forever };
            cd_sb.Begin();
        }

        private void CDImage_SizeChanged(object sender, SizeChangedEventArgs e) {
            var comp = CDImage.RenderTransform as CompositeTransform;
            if (comp == null)
                CDImage.RenderTransform = comp = new CompositeTransform();
            comp.CenterX = CDImage.ActualWidth / 2;
            comp.CenterY = CDImage.ActualHeight / 2;
        }

        #endregion

        #endregion

        #region Properties

        string sid;
        string ssid;
        string aid;
        string title;
        string artist;
        string image;
        Storyboard cd_sb;
        FrameType frameType;
        IEnumerable<LrcMetaData> songMessCollection;
        LrcListViewAnimationStyle animation_style = LrcListViewAnimationStyle.FastSlide;

        public VisualBoardVM VMForPublic { get { return MusicBoardVM; } }

        #endregion

    }
}
