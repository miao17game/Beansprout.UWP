#region Using
using static Wallace.UWP.Helpers.Tools.UWPStates;
using static Douban.UWP.NET.Resources.AppResources;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Douban.UWP.NET.Controls;
using Douban.UWP.Core.Models;
using Douban.UWP.NET.Tools;
using Windows.Services.Store;
using Douban.UWP.Core.Tools;
#endregion

namespace Douban.UWP.NET.Pages {

    public sealed partial class FMWebViewPage : BaseContentPage {

        #region Constructor

        public FMWebViewPage() {
            this.InitializeComponent();
            LOGIN_FIRST_TEXT.SetVisibility(!IsLogined);
        }

        #endregion

        #region Events

        protected override void InitPageState() {
            base.InitPageState();
            
        }

        private void Grid_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e) {
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            IncrementalLoadingBorder.SetVisibility(true);
            IncrementalLoading.SetVisibility(true);
            var args = e.Parameter as NavigateParameter;
            currentUri = args.ToUri;
            Scroll.Source = currentUri;
        }

        #region Web Events

        private void Scroll_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args) {

        }

        private void Scroll_ContentLoading(WebView sender, WebViewContentLoadingEventArgs args) {

        }

        private void Scroll_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args) {
            IncrementalLoadingBorder.SetVisibility(false);
        }

        #endregion

        #endregion

        private void Abort_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            AddGrid.SetVisibility(false);
        }

        private async void Submit_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            if (!HasFMExtensions) {
                var context = StoreContext.GetDefault();
                var result = await WindowsStoreHelpers.PurchaseAddOnAsync(context, "9mzf5cp1mf83");
                HasFMExtensions = result == PurchasAddOnReturn.Successful ? true : false;
                if (!HasFMExtensions)
                    return;
            }
            NavigateToBase?.Invoke(
                    sender,
                    null,
                    GetFrameInstance(FrameType.LeftPart),
                    GetPageType(NavigateType.FM_Extensions));
        }

        private void FreeTryBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            NavigateToBase?.Invoke(
                    sender,
                    null,
                    GetFrameInstance(FrameType.LeftPart),
                    GetPageType(NavigateType.FM_Extensions));
        }
    }
}
