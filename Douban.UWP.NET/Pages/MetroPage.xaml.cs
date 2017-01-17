using static Wallace.UWP.Helpers.Tools.UWPStates;
using static Douban.UWP.NET.Resources.AppResources;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Douban.UWP.NET.Tools;
using Douban.UWP.Core.Models;
using System.Threading.Tasks;
using Wallace.UWP.Helpers;
using Douban.UWP.Core.Tools;
using HtmlAgilityPack;

namespace Douban.UWP.NET.Pages {

    public sealed partial class MetroPage : Page {
        public MetroPage() {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            InitPageState();
            await Task.Delay(500);
            InitContentResourcesAsync();
            SetUserStatus();
            if (!IsLogined)
                await TryLoginAsync(true);
        }

        private void InitPageState() {
            GlobalHelpers.SetChildPageMargin(this, matchNumber: VisibleWidth, isDivideScreen: IsDivideScreen);
            GlobalHelpers.DivideWindowRange(this, DivideNumber, isDivideScreen: IsDivideScreen);
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e) {
            GlobalHelpers.SetChildPageMargin(this, matchNumber: VisibleWidth, isDivideScreen: IsDivideScreen);
        }

        private async void MetroGridView_ItemClickAsync(object sender, ItemClickEventArgs e) {
            var item = e.ClickedItem as NavigationBar;
            if (item == null)
                return;
            if(item.NaviType == NavigateType.A_D_T) {
                await ReadCacheAsync();
                OpenInnerContent();
            } else {
                CloseContentFrameIfNeed();
                DoubanLoading.SetVisibility(true);
                NavigateTitleBlock.Text = item.Title;
                NavigateToBase?.Invoke(
                    sender,
                    new NavigateParameter { ToUri = item != null ? item.PathUri : null },
                    GetFrameInstance(item.NaviType),
                    GetPageType(item.NaviType));
            }
        }

        private async Task ReadCacheAsync() {
            var res = HamburgerResList.ToList();
            var result = await GetMetroListAsync();
            var cache = default(List<string>);
            if (result == null || result == "")
                cache = null;
            else
                try { cache = result == null ? null : JsonHelper.FromJson<List<string>>(result); } catch { cache = null; }
            if (cache == null) {
                SelectResources.Source = res
                    .Select(singleton => new MetroChangeItem {
                        Title = singleton.Title,
                        Token = singleton.IdentityToken,
                        Selected = true
                    });
            } else {
                SelectResources.Source = res
                    .Select(singleton => new MetroChangeItem {
                        Title = singleton.Title,
                        Token = singleton.IdentityToken,
                        Selected = cache.Contains(singleton.IdentityToken)
                    });
            }
            forCache = (SelectResources.Source as IEnumerable<MetroChangeItem>).ToList();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e) {
            CloseContentFrameIfNeed();
            NavigateTitleBlock.Text = GetUIString("Settings");
            NavigateToBase?.Invoke(
                sender,
                null,
                GetFrameInstance(NavigateType.Settings),
                GetPageType(NavigateType.Settings));
        }

        private async void LoginButton_ClickAsync(object sender, RoutedEventArgs e) {
            DoubanLoading.SetVisibility(true);
            await TryLoginAsync();
            DoubanLoading.SetVisibility(false);
        }

        private async Task TryLoginAsync(bool isInit = false) {
            try {
                if (isInit) {
                    var userId = SettingsHelper.ReadSettingsValue(SettingsSelect.UserID) as string;
                    if (userId == null || userId == "LOGOUT")
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
                            await MainPage.SetUserStatusAsync(userId);
                    } catch { /* Ignore. */ }
                } else {
                    if (!IsLogined) {
                        NavigateToBase?.Invoke(null, null, GetFrameInstance(NavigateType.Login), GetPageType(NavigateType.Login));
                        MainPage.OpenLoginPopup();
                    } else
                        NavigateToBase?.Invoke(null, null, GetFrameInstance(NavigateType.UserInfo), GetPageType(NavigateType.UserInfo));
                }
            } catch { ReportHelper.ReportAttention(GetUIString("WebActionError")); }
        }

        public void SetUserStatus() {
            if (LoginStatus.APIUserinfos == null)
                return;
            LoginUserBlock.Text = LoginStatus.UserName;
            LoginUserText.SetVisibility(false);
            if (LoginStatus.APIUserinfos.Avatar != null)
                LoginUserIcon.Fill = new ImageBrush { ImageSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(LoginStatus.APIUserinfos.Avatar)) };
        }

        private async void InitContentResourcesAsync() {
            var res = HamburgerResList.ToList();
            var result = await GetMetroListAsync();
            var cache = default(List<string>);
            if (result == null || result == "")
                cache = null;
            else
                try { cache = result == null ? null : JsonHelper.FromJson<List<string>>(result); } catch { cache = null; }
            if (cache != null) 
                res = res.Where(i => cache.Contains(i.IdentityToken)).ToList();     
            res.Add(new NavigationBar { Title = GetUIString("AddMetroItem"), NaviType = NavigateType.A_D_T });
            GridViewResources.Source = res;
        }

        private void PopupAllComments_SizeChanged(object sender, SizeChangedEventArgs e) {
            InnerGrid.Width = (sender as Popup).ActualWidth;
            InnerGrid.Height = (sender as Popup).ActualHeight;
        }

        private void CloseAllComsBtn_Click(object sender, RoutedEventArgs e) {
            InnerContentPanel.IsOpen = false;
        }

        private void PopupAllComments_Closed(object sender, object e) {
            OutPopupBorder.Completed += OnOutPopupBorderOut;
            OutPopupBorder.Begin();
        }

        private void OnOutPopupBorderOut(object sender, object e) {
            OutPopupBorder.Completed -= OnOutPopupBorderOut;
            PopupBackBorder.SetVisibility(false);
        }

        public void OpenInnerContent() {
            InnerContentPanel.IsOpen = true;
            PopupBackBorder.SetVisibility(true);
            EnterPopupBorder.Begin();
        }

        private void CloseContentFrameIfNeed() {
            if (VisibleWidth <= 800) 
                MainContentFrame.Content = null;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e) {
            var name = (sender as CheckBox).CommandParameter as string;
            if ((sender as CheckBox).IsChecked != null && (sender as CheckBox).IsChecked.Value)
                forCache.Add(new MetroChangeItem { Token = name, Selected = true });
            else
                forCache.RemoveAll(i => i.Token == name);
        }

        private async void Submit_ClickAsync(object sender, RoutedEventArgs e) {
            var value = JsonHelper.ToJson(forCache.Where(i => i.Selected == true).Select(i => i.Token).ToList());
            await CacheHelpers.SaveSpecificCacheValueAsync(CacheSelect.MetroList, value);
            SetMetroList(value);
            InnerContentPanel.IsOpen = false;
            InitContentResourcesAsync();
        }

        List<MetroChangeItem> forCache = new List<MetroChangeItem>();

    }

    public class MetroChangeItem {
        public string Title { get; set; }
        public string Token { get; set; }
        public bool Selected { get; set; }
    }

}
