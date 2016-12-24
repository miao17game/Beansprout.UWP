﻿#region Using
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
#endregion

namespace Douban.UWP.NET.Pages {

    public sealed partial class WebViewPage : BaseContentPage {

        #region Constructor

        public WebViewPage() {
            this.InitializeComponent();
            DoubanLoading.SetVisibility(false);
        }

        #endregion

        #region Events

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
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
            doubanRing.SetVisibility(false);
        }

        #endregion

        #endregion

    }
}
