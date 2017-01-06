using static Wallace.UWP.Helpers.Tools.UWPStates;
using static Douban.UWP.NET.Resources.AppResources;

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
            InitPageState();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            GridViewResources.Source = HamburgerResList;
            if (!IsLogined)
                await TryLoginAsync(true);
            SetUserStatus();
        }

        private void InitPageState() {
            GlobalHelpers.SetChildPageMargin(this, matchNumber: VisibleWidth, isDivideScreen: IsDivideScreen);
            GlobalHelpers.DivideWindowRange(this, DivideNumber, isDivideScreen: IsDivideScreen);
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e) {
            GlobalHelpers.SetChildPageMargin(this, matchNumber: VisibleWidth, isDivideScreen: IsDivideScreen);
        }

        private void MetroGridView_ItemClick(object sender, ItemClickEventArgs e) {
            var item = e.ClickedItem as NavigationBar;
            if (item == null)
                return;
            DoubanLoading.SetVisibility(true);
            NavigateTitleBlock.Text = item.Title;
            NavigateToBase?.Invoke(
                sender,
                new NavigateParameter { ToUri = item != null ? item.PathUri : null },
                GetFrameInstance(item.NaviType),
                GetPageType(item.NaviType));
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e) {
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
                        await MainPage.SetUserStatusAsync(userId);
                    } catch { /* Ignore. */ }
                    IsLogined = true;
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

    }
}
