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
#endregion

namespace Douban.UWP.NET.Pages {

    public sealed partial class FMWebViewPage : BaseContentPage {

        #region Constructor

        public FMWebViewPage() {
            this.InitializeComponent();
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

    }
}
