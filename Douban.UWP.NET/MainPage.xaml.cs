﻿#region Using
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
using Windows.UI.Xaml.Media.Imaging;
using Douban.UWP.NET.Tools;
using System.Reflection;
using Windows.ApplicationModel.Background;
using Windows.UI.Xaml.Navigation;
#endregion

namespace Douban.UWP.NET {

    public sealed partial class MainPage : Page {
        public MainPage() {
            this.InitializeComponent();
            Current = this;
        }

        #region Methods

        private void InitMainPageState() {
            doubanRing.SetVisibility(true);
            NavigateManager.BackRequested += OnBackRequested;
            StatusBarInit.InitInnerDesktopStatusBar(true);
            Window.Current.SetTitleBar(BasePartBorder);
            var isDarkOrNot = (bool?)SettingsHelper.ReadSettingsValue(SettingsConstants.IsDarkThemeOrNot) ?? true;
            RequestedTheme = isDarkOrNot ? ElementTheme.Dark : ElementTheme.Light;
            RegisterAllTaskAsync();
        }

        private void SetControlAccessEnabled() {
            NavigateTitleBlock = this.navigateTitlePath;
            HamburgerBox = this.HamburgerListBox;
            MainLeftPartFrame = this.BasePartFrame;
            MainContentFrame = this.ContentFrame;
            MainLoginFrame = this.LoginPopupFrame;
            BaseListRing = this.baseListRing;
            MainLoginPopup = this.ImagePopup;
            DoubanLoading = this.doubanRing;
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

        private async Task TryLoginAsync(bool isInit = false) {
            try {
                if (isInit) {
                    var userId = SettingsHelper.ReadSettingsValue(SettingsSelect.UserID) as string;
                    if (userId == null || userId == "LOGOUT" )
                        return;
                    LoginResult = await DoubanWebProcess.GetDoubanResponseAsync("https://douban.com/mine/", false);
                    if (LoginResult == null) {
                        ReportHelper.ReportAttention(GetUIString("WebActionError"));
                        return;
                    }
                    var doc = new HtmlDocument();
                    doc.LoadHtml(LoginResult);
                    if (doc.DocumentNode.SelectSingleNode("//div[@class='top-nav-info']") == null) {
                        SettingsHelper.SaveSettingsValue(SettingsSelect.UserID, "LOGOUT");
                        return;
                    }
                    try {
                        if (!IsLogined)
                            await SetUserStatusAsync(userId);
                    } catch { /* Ignore. */ }
                } else {
                    if (!IsLogined) {
                        NavigateToBase?.Invoke(null, null, GetFrameInstance(NavigateType.Login), GetPageType(NavigateType.Login));
                        ImagePopup.IsOpen = true;
                    } else
                        NavigateToUserInfoPage();
                }
            } catch { ReportHelper.ReportAttention(GetUIString("WebActionError")); }
        }

        public static void OpenLoginPopup() {
            Current.ImagePopup.IsOpen = true;
        }

        private void NavigateToUserInfoPage() {
            NavigateToBase?.Invoke(null, null, GetFrameInstance(NavigateType.UserInfo), GetPageType(NavigateType.UserInfo));
        }

        public static void SetUserStatus(HtmlDocument doc) {
            LoginStatus = GlobalHelpers.GetLoginStatus(doc);
            Current.LoginUserBlock.Text = LoginStatus.UserName;
            Current.LoginUserText.SetVisibility(false);
            Current.LoginUserIcon.Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri(LoginStatus.ImageUrl)) };
        }

        public static async Task SetUserStatusAsync(string uid) {
            try {
                var result = await DoubanWebProcess.GetAPIResponseAsync(
                path: "https://m.douban.com/rexxar/api/v2/user/" + uid,
                host: "m.douban.com",
                reffer: "https://m.douban.com/mine/");
                LoginStatus = GlobalHelpers.GetLoginStatus(result);
                Current.LoginUserBlock.Text = LoginStatus.UserName;
                Current.LoginUserText.SetVisibility(false);
                Current.LoginUserIcon.Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri(LoginStatus.APIUserinfos.LargeAvatar)) };
                IsLogined = true;
            } catch {
                SettingsHelper.SaveSettingsValue(SettingsSelect.UserID, "LOGOUT");
                IsLogined = false;
            }
        }

        public void ResetUserStatus() {
            IsLogined = false;
            LoginStatus = new LoginStatusBag();
            LoginUserBlock.Text = GetUIString("LoginPanelCode");
            LoginUserText.SetVisibility(true);
            LoginUserIcon.Fill = new SolidColorBrush(Windows.UI.Colors.Gray);
        }

        private async void GetResourcesAsync() {
            NaviBarResouces.Source = HamburgerResList;
            await TryLoginAsync(true);
        }

        #endregion

        #region Events

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            PrepareFrame.Navigate(typeof(PreparePage));
            SetControlAccessEnabled();
            InitMainPageState();
            AdapteVitualNavigationBarIfNeed();
            InitSlideRecState();
            GetResourcesAsync();
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e) {
            if (ContentFrame.Content == null) {
                if (VisibleWidth <= 800) {
                    ContentFrame.Navigate(typeof(MetroPage));
                } else {
                    if (!isNeedClose)
                        InitCloseAppTask();
                    else
                        Application.Current.Exit();
                }
                e.Handled = true;
                return;
            } else {
                var cont_pg = ContentFrame.Content;
                if (cont_pg.GetType().GetTypeInfo().BaseType.Name == typeof(BaseContentPage).Name) {
                    (cont_pg as BaseContentPage).PageSlideOutStart(VisibleWidth > 800 ? false : true);
                } else if (cont_pg.GetType().GetTypeInfo().Name == typeof(MetroPage).Name) {
                    if (!isNeedClose) { InitCloseAppTask(); } else { Application.Current.Exit(); }
                    e.Handled = true;
                    return;
                }
            }
            e.Handled = true;
        }

        private void HamburgerListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var item = (sender as ListBox).SelectedItem as NavigationBar;
            if (item == null)
                return;
            doubanRing.SetVisibility(true);
            navigateTitlePath.Text = item.Title;
            NavigateToBase?.Invoke(
                sender, 
                new NavigateParameter { ToUri = item != null ? item.PathUri : null }, 
                GetFrameInstance(item.NaviType),
                GetPageType(item.NaviType));
            NavigationSplit.IsPaneOpen = false;
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e) {
            OpenOrClosePane();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e) {
            NavigationSplit.IsPaneOpen = false;
            navigateTitlePath.Text = GetUIString("Settings");
            NavigateToBase?.Invoke(
                sender,
                null,
                GetFrameInstance(NavigateType.Settings),
                GetPageType(NavigateType.Settings));
            HamburgerListBox.SelectedIndex = -1;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e) {
            DoubanLoading.SetVisibility(true);
            NavigationSplit.IsPaneOpen = false;
            await TryLoginAsync();
            DoubanLoading.SetVisibility(false);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e) {
            NavigateToBase?.Invoke(
                null,
                new NavigateParameter { Title = GetUIString("SEARCH"), ToUri = new Uri("https://m.douban.com/search/") },
                GetFrameInstance(NavigateType.Search),
                GetPageType(NavigateType.Search));
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
            if (ContentFrame.Content == null) {
                if (VisibleWidth > 800 && IsDivideScreen)
                    ContentFrame.Navigate(typeof(MetroPage));
            } else {
                var cont_pg = ContentFrame.Content;
                if (cont_pg.GetType().GetTypeInfo().BaseType.Name == typeof(BaseContentPage).Name) {
                    return;
                } else if (cont_pg.GetType().GetTypeInfo().Name == typeof(MetroPage).Name) {
                    if(VisibleWidth < 800)
                        ContentFrame.Content = null;
                }
            }
        }

        private void ImagePopup_SizeChanged(object sender, SizeChangedEventArgs e) {
            ImagePopupBorder.Width = (sender as Popup).ActualWidth;
            ImagePopupBorder.Height = (sender as Popup).ActualHeight;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e) {
            if (VisibleWidth > 800 && IsDivideScreen)
                ContentFrame.Navigate(typeof(MetroPage));
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

        #endregion

        #region BackgroundTasks Methods
        private const string liveTitleTask = "LIVE_TITLE_TASK";
        private const string ToastBackgroundTask = "TOAST_BACKGROUND_TASK";
        private const string ServiceCompleteTask = "SERVICE_COMPLETE_TASK";

        private async void RegisterAllTaskAsync() {
            var status = await BackgroundExecutionManager.RequestAccessAsync();
            if (status == BackgroundAccessStatus.Unspecified || status == BackgroundAccessStatus.DeniedBySystemPolicy || status == BackgroundAccessStatus.DeniedByUser) { return; }
            foreach (var item in BackgroundTaskRegistration.AllTasks) {
                if (item.Value.Name == liveTitleTask)
                    item.Value.Unregister(true);
                if (item.Value.Name == ToastBackgroundTask)
                    item.Value.Unregister(true);
                if (item.Value.Name == ServiceCompleteTask)
                    item.Value.Unregister(true);
            }
            RegisterServiceCompleteTask();
            RegisterLiveTitleTask();
        }

        private void RegisterLiveTitleTask() {
            var taskBuilder = new BackgroundTaskBuilder {
                Name = liveTitleTask,
                TaskEntryPoint = typeof(BackgroundTasks.TitleBackgroundUpdateTask).FullName
            };
            taskBuilder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));
            taskBuilder.SetTrigger(new TimeTrigger(60, false));
            var register = taskBuilder.Register();
        }

        private void RegisterServiceCompleteTask() {
            var taskBuilder = new BackgroundTaskBuilder {
                Name = ServiceCompleteTask,
                TaskEntryPoint = typeof(BackgroundTasks.ServicingComplete).FullName
            };
            taskBuilder.SetTrigger(new SystemTrigger(SystemTriggerType.ServicingComplete, false));
            var register = taskBuilder.Register();
        }

        public static BackgroundTaskRegistration FindTask(string taskName) {
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
                if (cur.Value.Name == taskName)
                    return (BackgroundTaskRegistration)(cur.Value);
            return null;
        }
        #endregion

        #region Properties and state

        private bool isNeedClose = false;
        public const string HomeHost = "https://www.douban.com/";
        public const string HomeHostInsert = "https://www.douban.com";

        #endregion

    }
}
