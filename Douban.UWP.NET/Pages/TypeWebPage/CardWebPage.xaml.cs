using static Wallace.UWP.Helpers.Tools.UWPStates;
using static Douban.UWP.NET.Resources.AppResources;

using Douban.UWP.Core.Tools;
using HtmlAgilityPack;
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
using Douban.UWP.NET.Controls;
using Douban.UWP.NET.Tools;
using Douban.UWP.Core.Models;
using System.Threading.Tasks;

namespace Douban.UWP.NET.Pages.TypeWebPage {

    public sealed partial class CardWebPage : BaseContentPage {

        public CardWebPage() {
            this.InitializeComponent();
            DoubanLoading.SetVisibility(false);
        }

        #region Events

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e) {
            GlobalHelpers.SetChildPageMargin(this, matchNumber: VisibleWidth, isDivideScreen: IsDivideScreen);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            SetPageLoadingStatus();
            var args = e.Parameter as NavigateParameter;
            if (!(isFromInfoClick = args.IsFromInfoClick))
                GlobalHelpers.SetChildPageMargin(this, matchNumber: VisibleWidth, isDivideScreen: IsDivideScreen);
            if (args == null)
                return;
            if (args.Title != null)
                navigateTitlePath.Text = args.Title;
            if (isNative = args.IsNative)
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
                PageSlideOutStart(VisibleWidth > 800 ? false : true);
        }

        private void RelativePanel_Loaded(object sender, RoutedEventArgs e) {
            defaultHeight = (sender as RelativePanel).ActualHeight;
        }

        #endregion

        #region Web Events

        private void Scroll_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args) {

        }

        private void Scroll_ContentLoading(WebView sender, WebViewContentLoadingEventArgs args) {

        }

        private async void Scroll_DOMContentLoadedAsync(WebView sender, WebViewDOMContentLoadedEventArgs args) {
            IncrementalLoadingBorder.SetVisibility(false);
            isDOMLoaded = true;
            var js = @"
                     document.body.style.overflow = 'hidden';
                     window.external.notify(JSON.stringify(document.body.scrollHeight));";
            await sender.InvokeScriptAsync("eval", new[] { js });
        }

        private void WebView_SizeChangedAsync(object sender, SizeChangedEventArgs e) {
            if (!isDOMLoaded)
                return;
            isNeedChange = false;
            if (isChangeFinished) {
                isChangeFinished = false;
                return;
            }
            WebViewHeightTimerInit();
        }

        private void WebView_ScriptNotify(object sender, NotifyEventArgs e) {
            var webView = sender as WebView;
            webView.Height = Convert.ToDouble(e.Value);
        }

        #endregion

        #region Methods

        protected override void InitPageState() {
            base.InitPageState();
            GlobalHelpers.DivideWindowRange(this, DivideNumber, isDivideScreen: IsDivideScreen);
        }

        private void WebViewHeightTimerInit() {
            if (timerForWebView == null) {
                timerForWebView = new DispatcherTimer();
                timerForWebView.Tick += (obj, args) => {
                    if (isNeedChange) {
                        timerForWebView.Stop();
                        ChangeWebViewHeightAsync();
                        timerForWebView = null;
                        isChangeFinished = true;
                    }
                    isNeedChange = true;
                };
                timerForWebView.Interval = new TimeSpan(0, 0, 0, 0, 800);
                timerForWebView.Start();
            }
        }

        public override void DoWorkWhenAnimationCompleted() {
            if (isFromInfoClick) {
                (this.Parent as Frame).Content = null;
                return;
            }
            if (VisibleWidth > 800) {
                if (IsDivideScreen)
                    MainContentFrame.Navigate(typeof(MetroPage));
                else
                    MainContentFrame.Content = null;
            } else
                MainContentFrame.Content = null;
        }

        private async void ChangeWebViewHeightAsync() {
            var js = @"window.external.notify(JSON.stringify(document.body.scrollHeight));";
            await WebView.InvokeScriptAsync("eval", new[] { js });
        }

        private async void SetWebViewSourceAsync(Uri uri) {
            try {
                var result = await DoubanWebProcess.GetMDoubanResponseAsync(uri.ToString());
                var doc = new HtmlDocument();
                doc.LoadHtml(result);
                WebView.NavigateToString(GetBodyContent(doc.DocumentNode));
            } catch {
                WebView.Source = uri;
            }
        }

        private string GetBodyContent(HtmlNode node) {
            return node.ContainsFormat("div", "class", "rich-note") ? NativeStringConnect(node, node.GetHtmlFormat("div", "class", "rich-note")) :
                node.ContainsFormat("div", "class", "full") ? NativeStringConnect(node, node.GetHtmlFormat("div", "class", "full")) :
                ConnectString(RemoveString(node));
        }

        private string NativeStringConnect(HtmlNode node, string content) {
            return WebStringNative(
                GetSectionByClass(node, "header").Replace("  ", "") +
                content +
                GetSectionByClass(node, "author").Replace("  ", "") );
        }

        private string GetSectionByClass(HtmlNode node, string className) {
            return node.GetHtmlFormat("section", "class", className);
        }

        private string GetDivByClass(HtmlNode node, string className) {
            return node.GetHtmlFormat("div", "class", className);
        }

        private string WebStringNative(string value) {
            return HtmlXHelperExtensions.CreateHtml(value.Replace("\n", "<br/>"), IsGlobalDark)
                .Replace(@"<img data-src", @"<img style='max-width:100%' src")
                .Replace(@"<div class='cc'>", @"<div>")
                .Replace(@"<table>", @"<table>")
                .Replace(@"href=""/", @"href=""https://m.douban.com/");
        }

        private string RemoveString(HtmlNode node) {
            return node
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
        bool isDOMLoaded = false;
        bool isNeedChange = false;
        bool isChangeFinished = false;
        double defaultHeight;
        DispatcherTimer timerForWebView;
        #endregion

    }
}
