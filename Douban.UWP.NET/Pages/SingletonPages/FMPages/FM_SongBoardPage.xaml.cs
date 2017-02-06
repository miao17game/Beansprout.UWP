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
            await InitMusicBoardAsync(args);
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

            Service.PlayList.CurrentItemChanged += OnCurrentItemChangedAsync;
            Service.Player.BufferingEnded += OnBufferingEnded;

            MusicBoardVM.BackImage = image;
            MusicBoardVM.CurrentTime = Service.Player.PlaybackSession.Position;
            MusicBoardVM.Duration = Service.Player.PlaybackSession.NaturalDuration;
            MusicBoardVM.LrcList = await LrcProcessHelper.ReadLRCFromWebAsync(title, artist, Colors.White);

            if (MusicBoardVM.LrcList == null)
                return;

            SetLrcAnimation(MusicBoardVM.LrcList);
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e) {
            GlobalHelpers.SetChildPageMargin(this, matchNumber: VisibleWidth, isDivideScreen: IsDivideScreen);
        }

        private void BaseHamburgerButton_Click(object sender, RoutedEventArgs e) {
            PageSlideOutStart(VisibleWidth > FormatNumber ? false : true);
        }

        private void LrcCanvas_SizeChanged(object sender, SizeChangedEventArgs e) {
            LrcListView.Width = LrcCanvas.ActualWidth;
            LrcListView.Height = LrcCanvas.ActualHeight;
        }

        private void LrcListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {

        }

        private void LrcCanvas_Loaded(object sender, RoutedEventArgs e) {

        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e) {
            if (Service.Player.PlaybackSession.PlaybackState == MediaPlaybackState.Playing) {
                Service.Player.Pause();
                PlayPauseButton.Content = char.ConvertFromUtf32(0xE768);
            } else if (Service.Player.PlaybackSession.PlaybackState == MediaPlaybackState.Paused) {
                Service.Player.Play();
                PlayPauseButton.Content = char.ConvertFromUtf32(0xE769);
            }
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e) {
            Service.PlayList.MovePrevious();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e) {
            Service.PlayList.MoveNext();
        }

        #region Method

        protected override void InitPageState() {
            base.InitPageState();
            GlobalHelpers.DivideWindowRange(this, DivideNumber, isDivideScreen: IsDivideScreen);
        }

        public override void DoWorkWhenAnimationCompleted() {
            if (VisibleWidth > FormatNumber && IsDivideScreen && MainMetroFrame.Content == null)
                MainMetroFrame.Navigate(typeof(MetroPage));
            timer?.Stop();
            if (timer != null)
                timer.Tick -= DispatcherTimerEventAsync;
            Service.PlayList.CurrentItemChanged -= OnCurrentItemChangedAsync;
            GetFrameInstance(frameType).Content = null;
        }

        private void OnBufferingEnded(MediaPlayer sender, object args) {
            PlayPauseButton.Content = char.ConvertFromUtf32(0xE769);
        }

        private async void OnCurrentItemChangedAsync(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args) {
            var newItem = args.NewItem;
            if (newItem == null)
                return;
            var arg = newItem.Source.CustomProperties["Message"] as MusicBoardParameter;
            if (arg == null)
                return;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => {
                await InitMusicBoardAsync(arg);
            });
        }

        #region Lrc Animations

        public void SetLrcAnimation(IList<LrcInfo> list) {
            if ((lrc_list = list).Count > 0) {
                indexNew = -1;
                //infos = new LrcInfo[list.Count];
                //list.CopyTo(infos, 0);
                MusicBoardVM.Duration = Service.Player.PlaybackSession.NaturalDuration;
                if (timer != null)
                    return;
                timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 300) };
                timer.Tick += DispatcherTimerEventAsync;
                timer.Start();
            }
        }

        public async void DispatcherTimerEventAsync(object sender, object e) {
            MusicBoardVM.CurrentTime = Service.Player.PlaybackSession.Position;
            var newTurn = 0;
            var current = MusicBoardVM.CurrentTime.TotalMilliseconds;
            //for (int turn = 0; turn < infos.Length && infos[turn].LrcTime < MusicBoardVM.CurrentTime.TotalMilliseconds; turn++) {
            //    newTurn = turn;
            //}

            newTurn = lrc_list.Select(i => i.LrcTime).Where(i => i < current).Count();

            await SetLrcListSelectedAsync(newTurn);
        }

        private async Task SetLrcListSelectedAsync(int newTurn) {
            index = newTurn;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                try {
                    LrcListView.SelectedIndex = index;
                    if (index != indexNew) {
                        SetLrcAnimation(index);
                        indexNew = index;
                        System.Diagnostics.Debug.WriteLine(index + "      <==========Now Turn.");
                    }
                } catch (ArgumentException) { /* Ingore. */}
            });
        }

        private void SetLrcAnimation(int index) {
            Canvas.SetTop(LrcListView, 230 - index * 44);
            double milis = 0;
            double turn = -44;
            try {
                //milis = 1.0 * (infos[index + 1].LrcTime - infos[index].LrcTime);
                milis = 1.0 * (lrc_list[index + 1].LrcTime - lrc_list[index].LrcTime);
                turn *= 1.0;
            } catch (Exception) { /*  */ }
            SetAnimation(turn, milis);
        }
       
        public void SetAnimation(double turn, double milis) {
            if (sb != null) { sb.Stop(); }
            sb = new Storyboard();
            AnimationStyle = "None";
            switch (AnimationStyle) {
                case "ANIMATION_FAST_SLIDE":
                    sb.Children.Add(GetTimelineFastSlide(turn, milis));
                    break;
                case "ANIMATION_SMOOTH_SLIDE":
                    sb.Children.Add(GetTimelineSmoothSlide(turn, milis));
                    break;
                case "ANIMATION_FAST_ROLL":
                    sb.Children.Add(GetTimelineFastRoll(turn, milis));
                    break;
                default:
                    sb.Children.Add(GetTimelineSmoothRoll(turn, milis));
                    break;
            }
            sb.Begin();
        }

        #region Lrc animation stye collection

        private DoubleAnimationUsingKeyFrames GetTimelineSmoothRoll(double y, double milis) {
            DoubleAnimationUsingKeyFrames Translate = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetProperty(Translate, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)").Path);
            Storyboard.SetTarget(Translate, LrcListView);
            EasingDoubleKeyFrame keyframe = new EasingDoubleKeyFrame() {
                KeyTime = KeyTime.FromTimeSpan(
                    TimeSpan.FromMilliseconds(milis)), Value = y
            };
            //keyframe . EasingFunction = new CubicEase ( ) { EasingMode = EasingMode . EaseOut };
            Translate.KeyFrames.Add(keyframe);
            return Translate;
        }

        private DoubleAnimationUsingKeyFrames GetTimelineFastRoll(double y, double milis) {
            DoubleAnimationUsingKeyFrames Translate = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetProperty(Translate, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)").Path);
            Storyboard.SetTarget(Translate, LrcListView);
            EasingDoubleKeyFrame keyframe = new EasingDoubleKeyFrame() {
                KeyTime = KeyTime.FromTimeSpan(
                    TimeSpan.FromMilliseconds(0.6 * milis)), Value = y
            };
            //keyframe . EasingFunction = new CubicEase ( ) { EasingMode = EasingMode . EaseOut };
            Translate.KeyFrames.Add(keyframe);
            return Translate;
        }

        private DoubleAnimationUsingKeyFrames GetTimelineSmoothSlide(double y, double milis) {
            DoubleAnimationUsingKeyFrames Translate = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetProperty(Translate, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)").Path);
            Storyboard.SetTarget(Translate, LrcListView);
            EasingDoubleKeyFrame keyframe = new EasingDoubleKeyFrame() {
                KeyTime = KeyTime.FromTimeSpan(
                    TimeSpan.FromMilliseconds(0.35 * milis)), Value = y
            };
            keyframe.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Translate.KeyFrames.Add(keyframe);
            return Translate;
        }

        private DoubleAnimationUsingKeyFrames GetTimelineFastSlide(double y, double milis) {
            DoubleAnimationUsingKeyFrames Translate = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetProperty(Translate, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)").Path);
            Storyboard.SetTarget(Translate, LrcListView);
            EasingDoubleKeyFrame keyframe = new EasingDoubleKeyFrame() {
                KeyTime = KeyTime.FromTimeSpan(
                    TimeSpan.FromMilliseconds(0.15 * milis)), Value = y
            };
            keyframe.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Translate.KeyFrames.Add(keyframe);
            return Translate;
        }

        #endregion

        #endregion

        #endregion

        #region Properties

        string sid;
        string ssid;
        string aid;
        string title;
        string artist;
        string image;
        FrameType frameType;
        static int index = 0;
        static int indexNew = -1;
        string AnimationStyle;
        //LrcInfo[] infos;
        IList<LrcInfo> lrc_list;
        Storyboard sb;
        DispatcherTimer timer;

        #endregion

    }
}
