using static Wallace.UWP.Helpers.Tools.UWPStates;
using static Douban.UWP.NET.Resources.AppResources;

using Douban.UWP.Core.Models;
using Douban.UWP.NET.Controls;
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
using Wallace.UWP.Helpers;
using Douban.UWP.NET.Tools;
using Douban.UWP.Core.Tools;
using Douban.UWP.Core.Models.LifeStreamModels;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using Windows.UI.Xaml.Media.Animation;

namespace Douban.UWP.NET.Pages {

    public sealed partial class UserInfoPage : BaseContentPage {
        public UserInfoPage() {
            this.InitializeComponent();
        }

        protected override void InitPageState() {
            base.InitPageState();
            GlobalHelpers.SetChildPageMargin(this, matchNumber: VisibleWidth, isDivideScreen: IsDivideScreen);
            GlobalHelpers.DivideWindowRange(this, DivideNumber, isDivideScreen: IsDivideScreen);
        }

        public override void DoWorkWhenAnimationCompleted() {
            AdaptToClearContentAfterOnBackPressed();
        }

        public override void PageSlideOutStart(bool isToLeft) {
            if (DetailsFrame.Content != null) {
                var pg = DetailsFrame.Content;
                if (pg.GetType().GetTypeInfo().BaseType.Name == typeof(BaseContentPage).Name)
                    (pg as BaseContentPage).PageSlideOutStart(isToLeft);
                else
                    DetailsFrame.Content = null;
                return;
            }
            base.PageSlideOutStart(isToLeft);
        }

        #region Events

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);

            transform = Scroll.RenderTransform as TranslateTransform;
            if (transform == null)
                Scroll.RenderTransform = transform = new TranslateTransform();
            borderTrans = listBorder.RenderTransform as TranslateTransform;
            if (borderTrans == null)
                listBorder.RenderTransform = borderTrans = new TranslateTransform();

            var args = e.Parameter as NavigateParameter;
            if (args == null)
                return;
            UserId = args.UserUid;
            frameType = args.FrameType;
        }

        private void DoublAnimationSlideIn_Completed(object sender, object e) {
            //throw new NotImplementedException();
        }

        private async void RelativePanel_LoadedAsync(object sender, RoutedEventArgs e) {
            UserInfoDetails = this.DetailsFrame;
            UserInfoPopup = this.InnerContentPanel;
            if (UserId == null || UserId == LoginStatus.UserId) {
                UserNameBlock.Text = LoginStatus.UserName ?? "";
                LocationBlock.Text = LoginStatus.LocationString ?? "";
                DescriptionBlock.Text = (LoginStatus.Description?.Replace("\n", " ")) ?? "";
                if (LoginStatus.APIUserinfos != null)
                    SetStateByLoginStatus();
            } else {
                try {
                    var result = await DoubanWebProcess.GetAPIResponseAsync(
                        path: "https://m.douban.com/rexxar/api/v2/user/" + UserId,
                        host: "m.douban.com",
                        reffer: "https://m.douban.com/mine/");
                    var resultBag = GlobalHelpers.GetLoginStatus(result);
                    UserNameBlock.Text = resultBag.UserName ?? "";
                    LocationBlock.Text = resultBag.LocationString ?? "";
                    DescriptionBlock.Text = resultBag.Description.Replace("\n", " ");
                    if (resultBag.APIUserinfos != null)
                        SetStateByLoginStatus(resultBag);
                } catch { /* Ignore */ }
            }
            listBorder.Margin = new Thickness(0, 20 + Scroll.ActualHeight, 0, 0);
            listScroll = GlobalHelpers.GetScrollViewer(ContentList);
            listScroll.ViewChanged += ListScroll_OnViewChanged;
        }

        private void BaseHamburgerButton_Click(object sender, RoutedEventArgs e) {
            PageSlideOutStart(VisibleWidth > FormatNumber ? false : true);
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e) {
            GlobalHelpers.SetChildPageMargin(this, matchNumber: VisibleWidth, isDivideScreen: IsDivideScreen);
            AdaptForScreenSize();
        }

        private void TalkButton_Click(object sender, RoutedEventArgs e) {
            ReportHelper.ReportAttentionAsync(GetUIString("StillInDeveloping"));
        }

        private void WatchButton_Click(object sender, RoutedEventArgs e) {
            ReportHelper.ReportAttentionAsync(GetUIString("StillInDeveloping"));
        }

        private void FlowButton_Click(object sender, RoutedEventArgs e) {
            ReportHelper.ReportAttentionAsync(GetUIString("StillInDeveloping"));
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            if (is_delta)
                return;
            RunButtonClick((sender as Button).Name);
            OpenAllComsBtn.SetVisibility(true);
            OpenInnerContent();
        }

        private async void LogoutButton_ClickAsync(object sender, RoutedEventArgs e) {
            var path = "https://www.douban.com/accounts/logout?source=main";
            await DoubanWebProcess.GetDoubanResponseAsync(path);
            SettingsHelper.SaveSettingsValue(SettingsSelect.UserID, "LOGOUT");
            GlobalHelpers.ResetLoginStatus();
            PageSlideOutStart(VisibleWidth > FormatNumber ? false : true);
        }

        private void ContentList_ItemClick(object sender, ItemClickEventArgs e) {
            var iten = e.ClickedItem as LifeStreamItem;
            if (iten == null)
                return;
            Uri.TryCreate(iten.PathUrl, UriKind.RelativeOrAbsolute, out var uri);
            NavigateToBase?.Invoke(
                null, 
                new NavigateParameter { Title = iten.Content?.Title, ToUri = uri, IsFromInfoClick = true }, 
                DetailsFrame,
                GetPageType(NavigateType.InfoItemClick));
        }

        private void PopupAllComments_SizeChanged(object sender, SizeChangedEventArgs e) {
            InnerGrid.Width = (sender as Popup).ActualWidth;
            InnerGrid.Height = (sender as Popup).ActualHeight;
        }

        private void CloseAllComsBtn_Click(object sender, RoutedEventArgs e) {
            InnerContentPanel.IsOpen = false;
        }

        private void OpenAllComsBtn_Click(object sender, RoutedEventArgs e) {
            OpenInnerContent();
        }

        private void PopupAllComments_Closed(object sender, object e) {
            OutPopupBorder.Completed += OnOutPopupBorderOut;
            OutPopupBorder.Begin();
        }

        private void OnOutPopupBorderOut(object sender, object e) {
            OutPopupBorder.Completed -= OnOutPopupBorderOut;
            PopupBackBorder.SetVisibility(false);
        }

        //private  void Scroll_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e) {
        //    var scroll = (sender as ScrollViewer);
        //    try {
        //        if (scroll.VerticalOffset <= 300)
        //            TitleBackRec.Opacity = (scroll.VerticalOffset) / 300;
        //        else if (TitleBackRec.Opacity < 1)
        //            TitleBackRec.Opacity = 1;
        //        if ((scroll.ScrollableHeight - scroll.VerticalOffset < 100)) {
        //            listSource?.HasMoreItemsOrNot(true);
        //            listSource?.LoadMoreItemsAsync(0);
        //        }
        //    } catch { /* Ignore */ }
        //}

        #endregion

        #region Methods

        private void SetStateByLoginStatus() {
            ReadMessageFromAPIInfos(LoginStatus);
        }

        private void SetStateByLoginStatus(LoginStatusBag bag) {
            ReadMessageFromAPIInfos(bag);
        }

        private void ReadMessageFromAPIInfos(LoginStatusBag bag) {
            var status = bag.APIUserinfos;
            try {
                BroadcastNumber.Text = status.StatusesCount.ToString();
                PhotosNumber.Text = status.PhotoAlbumsCount.ToString();
                DiaryNumber.Text = status.NotesCount.ToString();
                GroupsNumber.Text = status.JoinedGroupCount.ToString();
                BookMovieNumber.Text = status.CollectedSubjectsCount.ToString();
                FollowingNumber.Text = status.FollowingCount.ToString();
                FollowersNumber.Text = status.FollowersCount.ToString();
                GenderBlock.Foreground = status.Gender == "M" ?
                    new SolidColorBrush(Windows.UI.Color.FromArgb(255, 69, 90, 172)) :
                    new SolidColorBrush(Windows.UI.Color.FromArgb(255, 217, 6, 94));
                if (status.ProfileBannerLarge != null)
                    BackgroundImage.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(bag.APIUserinfos.ProfileBannerLarge));
                else if (status.ProfileBannerNormal != null)
                    BackgroundImage.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(bag.APIUserinfos.ProfileBannerNormal));
                Uri.TryCreate(bag.BigHeadUrl, UriKind.Absolute, out var head_uri);
                if (head_uri == null)
                    Uri.TryCreate(bag.ImageUrl, UriKind.Absolute, out head_uri);
                HeadUserImage.Fill = new ImageBrush { ImageSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(head_uri ?? new Uri(NoPictureUrl)), Stretch = Stretch.UniformToFill };
                UserId = status.ID;
            } catch (Exception e) {
                Debug.WriteLine(e.StackTrace + "\n" + e.Message);
            } finally { ListResources.Source = listSource = new DoubanLazyLoadContext<LifeStreamItem>(FetchMoreResourcesAsync); }
        }

        #region Adapt Methods

        private void AdaptForHightMobileMode() {
            HeadContainerStack.Children.Remove(U_L_GRID);
            DescriptionGrid.Children.Add(U_L_GRID);
            Grid.SetColumn(BTN_GRID, 1);
            Grid.SetColumnSpan(BTN_GRID, 1);
            U_L_GRID.HorizontalAlignment = HorizontalAlignment.Stretch;
            UserNameBlock.Foreground = IsGlobalDark ? new SolidColorBrush(Windows.UI.Colors.White) : new SolidColorBrush(Windows.UI.Colors.Black);
        }

        private void AdaptForWidePCMode() {
            DescriptionGrid.Children.Remove(U_L_GRID);
            HeadContainerStack.Children.Add(U_L_GRID);
            Grid.SetColumn(BTN_GRID, 0);
            Grid.SetColumnSpan(BTN_GRID, 2);
            U_L_GRID.HorizontalAlignment = HorizontalAlignment.Center;
            UserNameBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
        }

        #endregion

        #region Set Infos List

        private async Task<IList<LifeStreamItem>> FetchMoreResourcesAsync() {
            if (next_filter == "SHOULD_STOP")
                return empty;
            IncrementalLoadingBorder.SetVisibility(true);
            return await SetListResourcesAsync(UserId);
        }

        private async Task<IList<LifeStreamItem>> SetListResourcesAsync(string uid) {
            var newList = new List<LifeStreamItem>();
            try {
                var returns = await APIForFetchLifeStreamAsync(uid);
                var one = JsonHelper.FromJson<ListStreamOne>(returns);
                next_filter = one.Items == null || one.Items.Count() < 10 ? 
                    "SHOULD_STOP" : 
                    one.NextFilter;
                return one.Items.OrderByDescending(i => i.TimeForOrder).ToList();
            } catch(Exception e) {
                Debug.WriteLine(e.Message);
                Debug.WriteLine("SetListResourcesAsync ERROR");
                return new List<LifeStreamItem>();
            } finally { IncrementalLoadingBorder.SetVisibility(false); }
        }

        private async Task<string> APIForFetchLifeStreamAsync(string uid) {
            return await DoubanWebProcess.GetMDoubanResponseAsync(
                path: string.Format(APIFormat, uid, "1970-1", next_filter, "10"),
                host: "m.douban.com",
                reffer: string.Format("https://m.douban.com/people/{0}/", uid));
        }

        #endregion

        private void AdaptToClearContentAfterOnBackPressed() {
            if (VisibleWidth > FormatNumber && IsDivideScreen && MainMetroFrame.Content == null)
                MainMetroFrame.Navigate(typeof(MetroPage));
            GetFrameInstance(frameType).Content = null;
        }

        private void AdaptForScreenSize() {
            if (VisibleWidth > FormatNumber) {
                if (DescriptionGrid.Children.Contains(U_L_GRID))
                    AdaptForWidePCMode();
            } else {
                if (HeadContainerStack.Children.Contains(U_L_GRID))
                    AdaptForHightMobileMode();
            }
        }

        public void OpenInnerContent() {
            InnerContentPanel.IsOpen = true;
            PopupBackBorder.SetVisibility(true);
            EnterPopupBorder.Begin();
        }

        public string  CreateAPITargetByUid(string format, string uid) {
            return string.Format(format, uid);
        }

        public void NavigateTo(Type type) {
            ContentFrame.Navigate(type);
        }

        #endregion

        #region Properties and state

        private void RunButtonClick(string name) { if (EventMap.ContainsKey(name)) EventMap[name].Invoke(); }
        private IDictionary<string, Action> eventMap;
        private IDictionary<string, Action> EventMap {
            get {
                return eventMap ?? new Func<IDictionary<string, Action>>(() => {
                    return eventMap = new Dictionary<string, Action> {
                        {BroadcastButton.Name, () => ContentFrame.Navigate(typeof(MyStatusPage), new NavigateParameter{ UserUid = UserId??LoginStatus.UserId })},
                        {DiaryButton.Name, () => ContentFrame.Navigate(typeof(MyDiariesPage), new NavigateParameter{ UserUid = UserId??LoginStatus.UserId  })},
                        {BookMovieButton.Name, () => ReportHelper.ReportAttentionAsync(GetUIString("StillInDeveloping"))},
                        {PhotosButton.Name, () => ReportHelper.ReportAttentionAsync(GetUIString("StillInDeveloping"))},
                        {GroupsButton.Name, () => ReportHelper.ReportAttentionAsync(GetUIString("StillInDeveloping"))},
                        {FollowersButton.Name, () => ReportHelper.ReportAttentionAsync(GetUIString("StillInDeveloping"))},
                        {FollowingButton.Name, () => ReportHelper.ReportAttentionAsync(GetUIString("StillInDeveloping"))},
                    };
                }).Invoke();
            }
        }

        private const string APIFormat = "https://m.douban.com/rexxar/api/v2/user/{0}/lifestream?slice=recent-{1}&hot=false&filter_after={2}&count={3}&for_mobile=1";
        private const string NoPictureUrl = "https://www.none-wallace-767fc6vh7653df0jb.com/no_wallace_085sgdfg7447fddds65.jpg";

        string UserId;
        string next_filter = "";
        bool is_delta = false;
        bool still_rolling = false;
        ScrollViewer listScroll;
        TranslateTransform transform;
        TranslateTransform borderTrans;
        DoubleAnimation anima;
        Storyboard board;
        DoubanLazyLoadContext<LifeStreamItem> listSource;
        List<LifeStreamItem> empty = new List<LifeStreamItem>();
        FrameType frameType = FrameType.UserInfos;

        #endregion

        private void Scroll_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e) {
            is_delta = true;
            if (transform.Y + e.Delta.Translation.Y >= 0) {
                transform.Y = 0;
                return;
            }
            transform.Y += e.Delta.Translation.Y;
            //Debug.WriteLine(transform.Y);
            Scroll.Margin = new Thickness(0, transform.Y, 0, 0);
            listBorder.Margin = new Thickness(0, 20 + Scroll.ActualHeight + Scroll.Margin.Top*2, 0, 0);
            listBorder.Height = ActualHeight - Scroll.ActualHeight - Scroll.Margin.Top * 2;
            TitleBackRec.Opacity = -(Scroll.Margin.Top) / 200;
            if (transform.Y < -200 && allSlide.Visibility == Visibility.Visible) {
                allSlide.SetVisibility(false);
                var nowheight = listBorder.ActualHeight;
                listBorder.Height = ActualHeight - 70;
                borderTrans.Y = listBorder.Margin.Top - 70;
                listBorder.Margin = new Thickness(0, 70, 0, 0);
                GoPanelUp(nowheight);
            }
        }

        private async void Scroll_ManipulationCompletedAsync(object sender, ManipulationCompletedRoutedEventArgs e) {
            await Task.Delay(500);
            is_delta = false;
        }

        private void Scroll_SizeChanged(object sender, SizeChangedEventArgs e) {

        }

        private void ListScroll_OnViewChanged(object sender, ScrollViewerViewChangedEventArgs e) {
            if ((listScroll.ScrollableHeight - listScroll.VerticalOffset < 100)) {
                listSource?.HasMoreItemsOrNot(true);
                listSource?.LoadMoreItemsAsync(0);
            }
        }

        private void AllSlide_Tapped(object sender, TappedRoutedEventArgs e) {
            allSlide.SetVisibility(false);
            listBorder.Height = ActualHeight - 70;
            listBorder.Margin = new Thickness(0, 70, 0, 0);
            GoPanelAnimation();
        }

        private void GoPanelUp(double now) {
            Scroll.ManipulationDelta -= Scroll_ManipulationDelta;
            board = new Storyboard();
            anima = new DoubleAnimation {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
                Duration = new Duration(TimeSpan.FromMilliseconds(1000)),
                From = Scroll.Margin.Top/2,
                To = -Scroll.ActualHeight
            };
            anima.Completed += (sender, e)=> {
                Scroll.SetVisibility(false);
                allSlide.SetVisibility(false);
                TitleBackRec.Opacity = 1;
                Scroll.ManipulationDelta += Scroll_ManipulationDelta;
            };
            Storyboard.SetTarget(anima, transform);
            Storyboard.SetTargetProperty(anima, "Y");
            board.Children.Add(anima);
            anima = new DoubleAnimation {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
                Duration = new Duration(TimeSpan.FromMilliseconds(1000)),
                From = listBorder.ActualHeight - now,
                To = 0
            };
            anima.Completed += (sender, e)=> {
                listBorder.Margin = new Thickness(0, 70, 0, 0);
                borderTrans.Y = 0;
            };
            Storyboard.SetTarget(anima, borderTrans);
            Storyboard.SetTargetProperty(anima, "Y");
            board.Children.Add(anima);
            board.Completed += (sender, e) => board.Stop();
            board.Begin();
        }

        private void GoPanelAnimation() {
            board = new Storyboard();
            anima = new DoubleAnimation {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
                Duration = new Duration(TimeSpan.FromMilliseconds(1000)),
                From = -Scroll.Margin.Top,
                To = -Scroll.ActualHeight
            };
            anima.Completed += (sender,e)=> {
                listBorder.Margin = new Thickness(0, -500, 0, 0);
                transform.Y = 0;
                Scroll.SetVisibility(false);
            };
            Storyboard.SetTarget(anima, transform);
            Storyboard.SetTargetProperty(anima, "Y");
            board.Children.Add(anima);
            anima = new DoubleAnimation {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
                Duration = new Duration(TimeSpan.FromMilliseconds(1000)),
                From = Scroll.ActualHeight,
                To = 0
            };
            anima.Completed += (sender, e)=> {
                listBorder.Margin = new Thickness(0, 70, 0, 0);
                borderTrans.Y = 0;
            };
            Storyboard.SetTarget(anima, borderTrans);
            Storyboard.SetTargetProperty(anima, "Y");
            board.Children.Add(anima);
            board.Completed += (sender, e) => board.Stop();
            board.Begin();
            TitleBackRec.Opacity = 1;
        }

        private void UpToTopButton_Click(object sender, RoutedEventArgs e) {
            if (Scroll.Visibility == Visibility.Visible)
                return;
            Scroll.SetVisibility(true);
            Scroll.Margin = new Thickness(0, -Scroll.ActualHeight, 0, 0);
            board = new Storyboard();
            anima = new DoubleAnimation {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
                Duration = new Duration(TimeSpan.FromMilliseconds(1000)),
                From = 0,
                To = Scroll.ActualHeight
            };
            anima.Completed += (obj, args) => {
                Scroll.Margin = new Thickness(0, 0, 0, 0);
                transform.Y = 0;
            };
            Storyboard.SetTarget(anima, transform);
            Storyboard.SetTargetProperty(anima, "Y");
            board.Children.Add(anima);
            anima = new DoubleAnimation {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
                Duration = new Duration(TimeSpan.FromMilliseconds(1000)),
                From = 0,
                To = Scroll.ActualHeight
            };
            anima.Completed += (obj, args) => {
                listBorder.Height = ActualHeight - Scroll.ActualHeight - 20;
                listBorder.Margin = new Thickness(0, 20 + Scroll.ActualHeight, 0, 0);
                borderTrans.Y = 0;
                allSlide.SetVisibility(true);
            };
            Storyboard.SetTarget(anima, borderTrans);
            Storyboard.SetTargetProperty(anima, "Y");
            board.Children.Add(anima);
            board.Completed += (obj, args) => board.Stop();
            board.Begin();
            TitleBackRec.Opacity = 0;
        }
    }
}
