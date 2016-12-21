using static Wallace.UWP.Helpers.Tools.UWPStates;
using static Douban.UWP.NET.Resources.AppResources;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Douban.UWP.NET.Resources;
using Douban.UWP.Core.Models;
using Douban.UWP.NET.Pages;
using Wallace.UWP.Helpers.Tools;
using Wallace.UWP.Helpers;
using Windows.UI.Core;
using Douban.UWP.NET.Controls;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Primitives;
using Douban.UWP.Core.Tools;
using HtmlAgilityPack;

namespace Douban.UWP.NET {

    public sealed partial class MainPage : Page {
        public MainPage() {
            this.InitializeComponent();
            PrepareFrame.Navigate(typeof(PreparePage));
            SetControlAccessEnabled();
            InitMainPageState();
            AdapteVitualNavigationBarIfNeed();
            InitSlideRecState();
            GetResources();
        }

        #region Methods

        private void InitMainPageState() {
            baseListRing.IsActive = true;
            NavigateManager.BackRequested += OnBackRequested;
            StatusBarInit.InitInnerDesktopStatusBar(true);
            Window.Current.SetTitleBar(BasePartBorder);
            var isDarkOrNot = (bool?)SettingsHelper.ReadSettingsValue(SettingsConstants.IsDarkThemeOrNot) ?? true;
            RequestedTheme = isDarkOrNot ? ElementTheme.Dark : ElementTheme.Light;
        }

        private void SetControlAccessEnabled() {
            Current = this;
            HamburgerBox = this.HamburgerListBox;
            MainLeftPartFrame = this.BasePartFrame;
            MainContentFrame = this.ContentFrame;
            MainLoginFrame = this.LoginPopupFrame;
            BaseListRing = this.baseListRing;
            MainLoginPopup = this.ImagePopup;
        }

        private void InitCloseAppTask() {
            isNeedClose = true;
            new ToastSmooth(GetUIString("ClickTwiceToExit")).Show();
            Task.Run(async () => {
                await Task.Delay(2000);
                isNeedClose = false;
            });
        }

        private void AdapteVitualNavigationBarIfNeed() {
            if (IsMobile) {
                AppView.VisibleBoundsChanged += (s, e) => { AdapteVitualNavigationBarWithoutStatusBar(this); };
                AdapteVitualNavigationBarWithoutStatusBar(this);
            }
        }

        #endregion

        #region Events

        private void OnBackRequested(object sender, BackRequestedEventArgs e) {
            if (ContentFrame.Content == null) {
                if (!isNeedClose) { InitCloseAppTask(); } else { Application.Current.Exit(); }
                e.Handled = true;
                return;
            } else {
                // do nothing...
            }
            e.Handled = true;
        }

        private void HamburgerListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {

        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e) {
            OpenOrClosePane();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e) {
            NavigateToBase?.Invoke(
                sender,
                null,
                GetFrameInstance(NavigateType.Settings),
                GetPageType(NavigateType.Settings));
            HamburgerListBox.SelectedIndex = -1;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e) {
            BaseListRing.IsActive = true;
            var result = await DoubanWebProcess.GetDoubanResponseAsync("https://douban.com/");
            var doc = new HtmlDocument();
            doc.LoadHtml(result);
            if (doc.DocumentNode.SelectSingleNode("//div[@class='mod isay isay-disable has-commodity ']") != null) {
                NavigateToBase?.Invoke(
                sender,
                new NavigateParameter { SpecialParameter = result },
                GetFrameInstance(NavigateType.UserInfo),
                GetPageType(NavigateType.UserInfo));
                return;
            }
            NavigateToBase?.Invoke(
                sender,
                null,
                GetFrameInstance(NavigateType.Login),
                GetPageType(NavigateType.Login));
            ImagePopup.IsOpen = true;
        }

        private void NavigationSplit_PaneClosed(SplitView sender, object args) {
            SlideAnimaRec.SetVisibility(true);
            OutBorder.Completed += OnOutBorderOut;
            OutBorder.Begin();
        }

        private void OnOutBorderOut(object sender, object e) {
            OutBorder.Completed -= OnOutBorderOut;
            DarkDivideBorder.SetVisibility(false);
        }

        /// <summary>
        /// Start the dark animation when hamburger menu opened.
        /// </summary>
        private void OnPaneIsOpened() {
            DarkDivideBorder.SetVisibility(true);
            EnterBorder.Begin();
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e) {
            ImagePopup.Width = (sender as Grid).ActualWidth;
            ImagePopup.Height = (sender as Grid).ActualHeight;
        }

        private void ImagePopup_SizeChanged(object sender, SizeChangedEventArgs e) {
            ImagePopupBorder.Width = (sender as Popup).ActualWidth;
            ImagePopupBorder.Height = (sender as Popup).ActualHeight;
        }

        #endregion

        #region Slide Animations

        private Storyboard slideStory;
        private TranslateTransform slideTranslateT;

        private void InitSlideRecState() {
            SlideAnimaRec.ManipulationMode = ManipulationModes.TranslateX;
            SlideAnimaRec.ManipulationCompleted += SlideAnimaRec_ManipulationCompleted;
            SlideAnimaRec.ManipulationDelta += SlideAnimaRec_ManipulationDelta;
            slideTranslateT = SlideAnimaRec.RenderTransform as TranslateTransform;
            if (slideTranslateT == null)
                SlideAnimaRec.RenderTransform = slideTranslateT = new TranslateTransform();
        }

        private void SlideAnimaRec_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs args) {
            if (slideTranslateT.X + args.Delta.Translation.X < 0) {
                slideTranslateT.X = 0;
                return;
            }
            slideTranslateT.X += args.Delta.Translation.X;
        }

        private void SlideAnimaRec_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e) {
            double abs_delta = Math.Abs(e.Cumulative.Translation.X);
            double speed = Math.Abs(e.Velocities.Linear.X);
            double delta = e.Cumulative.Translation.X;
            double to = 0;

            if (abs_delta < SlideAnimaRec.ActualWidth / 2 && speed < 0.7) {
                slideTranslateT.X = 0;
                return;
            }

            if (delta > 0)
                to = SlideAnimaRec.ActualWidth;
            else if (delta < 0)
                return;

            slideStory = new Storyboard();
            var doubleanimation = new DoubleAnimation() { Duration = new Duration(TimeSpan.FromMilliseconds(0)), From = slideTranslateT.X, To = 0 };
            doubleanimation.Completed += Doubleanimation_Completed;
            Storyboard.SetTarget(doubleanimation, slideTranslateT);
            Storyboard.SetTargetProperty(doubleanimation, "X");
            slideStory.Children.Add(doubleanimation);
            slideStory.Begin();
        }

        private void Doubleanimation_Completed(object sender, object e) {
            OpenOrClosePane();
        }

        private void OpenOrClosePane() {
            NavigationSplit.IsPaneOpen = !NavigationSplit.IsPaneOpen;
            SlideAnimaRec.SetVisibility(!NavigationSplit.IsPaneOpen);
            OnPaneIsOpened();
        }

        private void GetResources() {
            NaviBarResouces.Source = AppResources.HamburgerResList;
        }

        #endregion

        #region Properties and state

        private bool isNeedClose = false;
        private bool isNeedLogin = true;
        public const string HomeHost = "https://www.douban.com/";
        public const string HomeHostInsert = "https://www.douban.com";

        #endregion

    }
}
