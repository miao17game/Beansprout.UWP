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
using Windows.UI.Xaml;
using Douban.UWP.Core.Tools;
using HtmlAgilityPack;
#endregion

namespace Douban.UWP.NET.Pages {

    public sealed partial class WebContentPage : BaseContentPage {

        #region Constructor

        public WebContentPage() {
            this.InitializeComponent();
            DoubanLoading.SetVisibility(false);
        }

        #endregion

        #region Events

        public override void PageSlideOutStart(bool isToLeft) {
            base.PageSlideOutStart(isToLeft);
            WebView.Navigate(new Uri("https://www.none-wallace-767fc6vh7653df0jb.com/no_wallace_085sgdfg7447fddds65.jpg"));
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e) {
            GlobalHelpers.SetChildPageMargin(this, matchNumber: VisibleWidth, isDivideScreen: IsDivideScreen);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            SetPageLoadingStatus();
            var args = e.Parameter as NavigateParameter;
            frameType = args.FrameType;
            if (isFromInfoClick = args.IsFromInfoClick)
                GlobalHelpers.SetChildPageMargin(this, matchNumber: VisibleWidth, isDivideScreen: IsDivideScreen);
            if (args == null)
                return;
            if (args.Title != null)
                navigateTitlePath.Text = args.Title;
            if(isNative = args.IsNative)
                SetWebViewSourceAsync(currentUri = args.ToUri);
            else
                WebView.Source = currentUri = args.ToUri;
        }

        private void FullContentBtn_Click(object sender, RoutedEventArgs e) {
            WebView.Source = currentUri;
        }

        private void BaseHamburgerButton_Click(object sender, RoutedEventArgs e) {
            if (WebView.CanGoBack)
                WebView.GoBack();
            else
                PageSlideOutStart(VisibleWidth > FormatNumber ? false : true);
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

        #region Methods

        protected override void InitPageState() {
            base.InitPageState();
            GlobalHelpers.DivideWindowRange(this, DivideNumber, isDivideScreen: IsDivideScreen);
        }

        public override void DoWorkWhenAnimationCompleted() {
            if (isFromInfoClick) {
                (this.Parent as Frame).Content = null;
                return;
            }
            if (VisibleWidth > FormatNumber && IsDivideScreen && MainMetroFrame.Content == null)
                MainMetroFrame.Navigate(typeof(MetroPage));
            GetFrameInstance(frameType).Content = null;
        }

        private async void SetWebViewSourceAsync(Uri uri) {
            try {
                var result = await DoubanWebProcess.GetMDoubanResponseAsync(uri.ToString());
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(result);
                var root = doc.DocumentNode;
                ConnectString(RemoveString(doc));
            } catch {
                WebView.Source = uri;
            }
        }

        private string RemoveString(HtmlDocument doc) {
            return doc.DocumentNode
                .RemoveFormat("div", "id", "TalionNav")
                .RemoveFormat("div", "class", "tohomepage")
                .RemoveFormat("div", "class", "download-app")
                .RemoveFormat("section", "class", "promo_top_banner")
                .RemoveFormat("section", "class", "tags")
                .RemoveFormat("section", "class", "note-comments")
                .RemoveFormat("section", "class", "center")
                .RemoveFormat("section", "class", "user-notes")
                .RemoveFormat("section", "class", "related-more")
                .RemoveFormat("section", "id", "ThemesWidget")
                .OuterHtml;
        }

        private string ConnectString(string value) {
            return value;
        }

        private void SetPageLoadingStatus() {
            DoubanLoading.SetVisibility(false);
            IncrementalLoadingBorder.SetVisibility(true);
            IncrementalLoading.SetVisibility(true);
        }

        #endregion

        #region Properties
        bool isFromInfoClick = false;
        bool isNative = false;
        FrameType frameType;
        #endregion
    }
}
