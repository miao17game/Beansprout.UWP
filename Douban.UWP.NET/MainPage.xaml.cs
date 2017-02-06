#region Using
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
using Douban.UWP.BackgroundTasks;
using Windows.ApplicationModel.Activation;
#endregion

namespace Douban.UWP.NET {

    public sealed partial class MainPage : Page {
        public MainPage() {
            this.InitializeComponent();
            Current = this;
        }

        #region Methods

        private void InitMainPageState() {
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
            MainUserInfosFrame = this.UserInfosFrame;
            MainMetroFrame = this.MetroFrame;
            MainUpContentFrame = this.UpContentFrame;
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

        private async Task TryLoginAsync(bool isInit = false) {
            try {
                if (isInit) {
                    var userId = SettingsHelper.ReadSettingsValue(SettingsSelect.UserID) as string;
                    if (userId == null || userId == "LOGOUT" )
                        return;
                    LoginResult = await DoubanWebProcess.GetDoubanResponseAsync("https://douban.com/mine/", false);
                    if (LoginResult == null) {
                        ReportHelper.ReportAttentionAsync(GetUIString("WebActionError"));
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
                        NavigateToBase?.Invoke(null, null, GetFrameInstance(FrameType.Login), GetPageType(NavigateType.Login));
                        ImagePopup.IsOpen = true;
                    } else
                        NavigateToUserInfoPage();
                }
            } catch { ReportHelper.ReportAttentionAsync(GetUIString("WebActionError")); }
        }

        public static void OpenLoginPopup() {
            Current.ImagePopup.IsOpen = true;
        }

        private void NavigateToUserInfoPage() {
            NavigateToBase?.Invoke(null, null, GetFrameInstance(FrameType.UserInfos), GetPageType(NavigateType.UserInfo));
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
                var succeed = Uri.TryCreate(LoginStatus.APIUserinfos.LargeAvatar, UriKind.Absolute, out var img_uri);
                if(!succeed)
                    succeed = Uri.TryCreate(LoginStatus.APIUserinfos.Avatar, UriKind.Absolute, out img_uri);
                if(succeed)
                    Current.LoginUserIcon.Fill = new ImageBrush { ImageSource = new BitmapImage(img_uri) };
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
            HasFMExtensions = await WindowsStoreHelpers.GetProductInfoAsync(Windows.Services.Store.StoreContext.GetDefault(), id: "9mzf5cp1mf83");
        }

        #endregion

        #region Events

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            var args = e.Parameter as ToastNotificationActivatedEventArgs;
            toastUri = args?.Argument;
            PrepareFrame.Navigate(typeof(PreparePage));
            SetControlAccessEnabled();
            InitMainPageState();
            AdapteVitualNavigationBarIfNeed();
            InitSlideRecState();
            GetResourcesAsync();
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e) {
            if (UserInfosFrame.Content != null) {
                OutFramePage(UserInfosFrame);
            } else {
                if (UpContentFrame.Content != null) {
                    OutFramePage(UpContentFrame);
                } else {
                    if (ContentFrame.Content != null) {
                        OutFramePage(ContentFrame);
                    } else {
                        if (MetroFrame.Content != null) {
                            if (!isNeedClose) { InitCloseAppTask(); } else { Application.Current.Exit(); }
                        } else {
                            MetroFrame.Navigate(typeof(MetroPage));
                        }
                    }
                }
            }
            e.Handled = true;
        }

        private void OutFramePage(Frame frame) {
            var cont_pg = frame.Content;
            if (cont_pg == null)
                return;
            if (cont_pg.GetType().GetTypeInfo().BaseType.Name == typeof(BaseContentPage).Name)
                (cont_pg as BaseContentPage).PageSlideOutStart(VisibleWidth > FormatNumber ? false : true);
        }

        private void HamburgerListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var item = (sender as ListBox).SelectedItem as NavigationBar;
            if (item == null)
                return;
            navigateTitlePath.Text = item.Title;
            if (item.NaviType == NavigateType.FM || item.NaviType == NavigateType.FM_Extensions) {
                //  TO DO WORK FOR STORE EXTENSIONS
                if (IsLogined && HasFMExtensions)
                    item.NaviType = NavigateType.FM_Extensions;
                else
                    item.NaviType = NavigateType.FM;
            }
            NavigateToBase?.Invoke(
                sender, 
                new NavigateParameter { ToUri = item?.PathUri }, 
                GetFrameInstance(item.FrameType),
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
                GetFrameInstance(FrameType.LeftPart),
                GetPageType(NavigateType.Settings));
            HamburgerListBox.SelectedIndex = -1;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e) {
            NavigationSplit.IsPaneOpen = false;
            await TryLoginAsync();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e) {
            NavigateToBase?.Invoke(
                null,
                new NavigateParameter { Title = GetUIString("SEARCH"), ToUri = new Uri("https://m.douban.com/search/") },
                GetFrameInstance(FrameType.Content),
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
            if (UserInfosFrame.Content == null && UpContentFrame.Content == null && ContentFrame.Content == null && MetroFrame.Content == null) {
                if (VisibleWidth > FormatNumber && IsDivideScreen)
                    MetroFrame.Navigate(typeof(MetroPage));
            }
        }

        private void ImagePopup_SizeChanged(object sender, SizeChangedEventArgs e) {
            ImagePopupBorder.Width = (sender as Popup).ActualWidth;
            ImagePopupBorder.Height = (sender as Popup).ActualHeight;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e) {
            if (VisibleWidth > FormatNumber && IsDivideScreen)
                MetroFrame.Navigate(typeof(MetroPage));
            if (toastUri != null) {
                var decode = JsonHelper.FromJson<ToastParameters>(toastUri);
                if (decode != null) {
                    NavigateToBase?.Invoke(
                        null,
                        new NavigateParameter {
                            ToUri = new Uri(decode.Uri),
                            Title = decode.Title,
                            IsFromInfoClick = true,
                            IsNative = true,
                            FrameType = FrameType.Content
                        },
                        GetFrameInstance(FrameType.Content),
                        GetPageType(NavigateType.ItemClickNative));
                }
            }
        }

        private void MetroFrame_Navigated(object sender, NavigationEventArgs e) {
            ContentFrame.Content = null;
            UpContentFrame.Content = null;
            UserInfosFrame.Content = null;
        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e) {
            UpContentFrame.Content = null;
            UserInfosFrame.Content = null;
        }

        private void UpContentFrame_Navigated(object sender, NavigationEventArgs e) {
            UserInfosFrame.Content = null;
        }

        private void UserInfosFrame_Navigated(object sender, NavigationEventArgs e) {

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

        private async void RegisterAllTaskAsync() {
            var succeed = await TaskHelper.IfAccessNotDeniedAsync();
            if (!succeed)
                return;

            TaskHelpers.FindTask(TaskConstants.ServiceComplete)?.Unregister(true);
            TaskHelpers.FindTask(TaskConstants.ToastBackground)?.Unregister(true);
            TaskHelpers.FindTask(TaskConstants.LiveTitle)?.Unregister(true);

            TaskHelpers.RegisterServiceCompleteTask();
            TaskHelpers.RegisterLiveTitleTask();
            TaskHelpers.RegisterToastBackgroundTask();
        }

        #endregion

        #region Properties and state

        bool isNeedClose = false;
        string toastUri;
        const string HomeHost = "https://www.douban.com/";
        const string HomeHostInsert = "https://www.douban.com";

        #endregion

    }
}
