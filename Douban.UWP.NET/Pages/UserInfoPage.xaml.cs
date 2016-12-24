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

        #region Events

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            DoubanLoading.SetVisibility(false);
            HeadUserImage.Fill = new ImageBrush { ImageSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(LoginStatus.BigHeadUrl) };
            UserNameBlock.Text = LoginStatus.UserName;
            LocationBlock.Text = LoginStatus.LocationString;
            DescriptionBlock.Text = LoginStatus.Description;
        }

        private void BaseHamburgerButton_Click(object sender, RoutedEventArgs e) {
            PageSlideOutStart(VisibleWidth > 800 ? false : true);
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e) {
            GlobalHelpers.SetChildPageMargin(this, matchNumber: VisibleWidth, isDivideScreen: IsDivideScreen);
            if(VisibleWidth > 800) {
                if (DescriptionGrid.Children.Contains(U_L_GRID)) {
                    DescriptionGrid.Children.Remove(U_L_GRID);
                    HeadContainerStack.Children.Add(U_L_GRID);
                    Grid.SetColumn(BTN_GRID, 0);
                    Grid.SetColumnSpan(BTN_GRID, 2);
                    U_L_GRID.HorizontalAlignment = HorizontalAlignment.Center;
                    UserNameBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
                }
            } else {
                if (HeadContainerStack.Children.Contains(U_L_GRID)) {
                    HeadContainerStack.Children.Remove(U_L_GRID);
                    DescriptionGrid.Children.Add(U_L_GRID);
                    Grid.SetColumn(BTN_GRID, 1);
                    Grid.SetColumnSpan(BTN_GRID, 1);
                    U_L_GRID.HorizontalAlignment = HorizontalAlignment.Stretch;
                    UserNameBlock.Foreground = IsGlobalDark ? new SolidColorBrush(Windows.UI.Colors.White) : new SolidColorBrush(Windows.UI.Colors.Black);
                }
            }
        }

        #endregion

        #region Methods

        #endregion

        #region Properties and state
        private enum ContentType { None = 0, String = 1, Image = 2, Gif = 3, Video = 4, Flash = 5, SelfUri = 6 }
        #endregion

        private void TalkButton_Click(object sender, RoutedEventArgs e) {

        }

        private void WatchButton_Click(object sender, RoutedEventArgs e) {

        }

        private void FlowButton_Click(object sender, RoutedEventArgs e) {

        }
    }
}
