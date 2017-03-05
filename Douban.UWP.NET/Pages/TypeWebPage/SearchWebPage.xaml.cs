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
using HtmlAgilityPack;
using Douban.UWP.Core.Tools;
using Wallace.UWP.Helpers.Tools;
using Wallace.UWP.Helpers;
using Douban.UWP.NET.Controls;
using Windows.UI.Xaml.Media.Animation;
using System.Text.RegularExpressions;

namespace Douban.UWP.NET.Pages.TypeWebPage {

    public sealed partial class SearchWebPage : BaseContentPage {
        public SearchWebPage() {
            this.InitializeComponent();
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e) {
            GlobalHelpers.SetChildPageMargin(this, matchNumber: VisibleWidth, isDivideScreen: IsDivideScreen);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            var args = e.Parameter as NavigateParameter;
            frameType = args.FrameType;
            if (isFromInfoClick = args.IsFromInfoClick)
                GlobalHelpers.SetChildPageMargin(this, matchNumber: VisibleWidth, isDivideScreen: IsDivideScreen);
            if (args == null)
                return;
            isNative = args.IsNative;
            currentUri = args.ToUri;
        }

        private async void WebView_DOMContentLoadedAsync(WebView sender, WebViewDOMContentLoadedEventArgs args) {
            IncrementalLoadingBorder.SetVisibility(false);
            DefineJsFunction(out var js);
            await sender.InvokeScriptAsync("eval", new[] { js });
        }

        private void WebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args) {

        }

        private void WebView_ContentLoading(WebView sender, WebViewContentLoadingEventArgs args) {

        }

        private void WebView_SizeChanged(object sender, SizeChangedEventArgs e) {

        }

        private void WebView_ScriptNotify(object sender, NotifyEventArgs e) {
            var callBack = JsonHelper.FromJson<string>(e.Value);

            //System.Diagnostics.Debug.WriteLine("Notify CallBack ---->  :  " + e.Value);

            var movie_mess = new Regex(@"(<?content>/movie/subject/.+)/").Match(e.Value).Groups["content"].Value;
            if (movie_mess != "") {
                var target_path = "https://m.douban.com" + movie_mess;
                var succeed = Uri.TryCreate(target_path, UriKind.Absolute, out var uri);
                if (!succeed)
                    return;
                NavigateToBase?.Invoke(
                    null,
                    new NavigateParameter {
                        ToUri = uri,
                        Title = "LINK CONTENT",
                        IsNative = true,
                        FrameType = FrameType.UpContent
                    },
                    GetFrameInstance(FrameType.UpContent),
                    GetPageType(NavigateType.MovieContent));
            } else {
                var content_mess = new Regex(@"actionlink:(?<content>.+/)").Match(e.Value).Groups["content"].Value;
                if (content_mess == "")
                    return;
                var target_path = "https://m.douban.com" + content_mess;
                var succeed = Uri.TryCreate(target_path, UriKind.Absolute, out var uri);
                if (!succeed)
                    return;
                NavigateToBase?.Invoke(
                    null,
                    new NavigateParameter { Title = "LINK CONTENT", ToUri = uri, FrameType = FrameType.UpContent },
                    GetFrameInstance(FrameType.UpContent),
                    GetPageType(NavigateType.Undefined));
            }

        }

        private void Search_Grid_Loaded(object sender, RoutedEventArgs e) {
            var trams = (Search_Grid.RenderTransform = new TranslateTransform()) as TranslateTransform;
            Search_Grid.Margin = new Thickness(0, WebGrid.ActualHeight / 2 - 80, 0, 0);
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e) {
            if (SearchBox.Text == null || SearchBox.Text == "")
                return;
            SetPageLoadingStatus();
            StartShadowSearchPanelAnimations();
            var path = $"{"https://"}m.douban.com/search/?query={SearchBox.Text}";
            var succeed = Uri.TryCreate(path, UriKind.Absolute, out var uri);
            if (!succeed)
                return;
            webView.SetVisibility(true);
            if (isNative)
                SetWebViewSourceAsync(currentUri = uri);
            else
                webView.Source = currentUri = uri;
        }

        private void BaseHamburgerButton_Click(object sender, RoutedEventArgs e) {
            if (webView.CanGoBack)
                webView.GoBack();
            else
                PageSlideOutStart(VisibleWidth > FormatNumber ? false : true);
        }

        private void FullContentBtn_Click(object sender, RoutedEventArgs e) {
            NavigateToBase?.Invoke(
                null,
                new NavigateParameter { FrameType = FrameType.UpContent, ToUri = currentUri, Title = GetUIString("LinkContent") },
                GetFrameInstance(FrameType.UpContent),
                GetPageType(NavigateType.Undefined));
        }

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

        private void SetPageLoadingStatus() {
            IncrementalLoadingBorder.SetVisibility(true);
            IncrementalLoading.SetVisibility(true);
        }

        private void StartShadowSearchPanelAnimations() {
            Storyboard sb = new Storyboard();
            var to_top = Search_Grid.Margin.Top;
            if (to_top == 0)
                return;
            doubleAnimation = new DoubleAnimation {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut },
                Duration = new Duration(TimeSpan.FromMilliseconds(1020)),
                From = 0,
                To = -to_top,
            };
            doubleAnimation.Completed += DoublAnimation_Completed;
            Storyboard.SetTarget(doubleAnimation, Search_Grid.RenderTransform as TranslateTransform);
            Storyboard.SetTargetProperty(doubleAnimation, "Y");
            sb.Children.Add(doubleAnimation);
            Out_ani.SpeedRatio = 0.5;
            SB.Begin();
            sb.Begin();
        }

        private void DoublAnimation_Completed(object sender, object e) {
            doubleAnimation.Completed -= DoublAnimation_Completed;
            var trams = Search_Grid.RenderTransform as TranslateTransform;
            ForeBackRec.SetVisibility(false);
            Search_Grid.Margin = new Thickness(0);
            trams.Y = 0;
        }

        private async void SetWebViewSourceAsync(Uri uri) {
            try {
                var result = htmlReturn = await DoubanWebProcess.GetMDoubanResponseAsync(
                    path: uri.ToString(),
                    host: "m.douban.com",
                    reffer: "https://m.douban.com/");
                var doc = new HtmlDocument();
                doc.LoadHtml(result);
                var shouldNative = IfCanGetContent(doc.DocumentNode);
                webView.NavigateToString(shouldNative ? GetContent(doc.DocumentNode) : RemoveString(doc.DocumentNode));
            } catch {
                webView.Source = uri;
                isNative = false;
            }
        }

        /// <summary>
        /// Get the native-done web string.
        /// </summary>
        /// <param name="node">htmlRoot</param>
        /// <returns></returns>
        private string GetContent(HtmlNode node) {
            return /*node.ContainsFormat("div", "class", "search-bd") ? NativeStringConnect(node, node.GetHtmlFormat("div", "class", "search-bd")) :*/
                RemoveString(node);
        }

        private bool IfCanGetContent(HtmlNode node) {
            return /*node.ContainsFormat("div", "class", "search-bd") ? true :*/false;
        }

        /// <summary>
        /// Connect body-string and anyother useful html-nodes
        /// </summary>
        /// <param name="node"></param>
        /// <param name="content">body-content</param>
        /// <returns></returns>
        private string NativeStringConnect(HtmlNode node, string content) {
            return nativeString = WebStringNative(content);
        }

        /// <summary>
        /// Global Native-work for the web-string.
        /// </summary>
        /// <param name="value">web string contrnt</param>
        /// <returns></returns>
        private string WebStringNative(string value) {
            //var maxWidthPercent = "100";
            //return XHtmlHelpers.CreateDefaultHtml(value.Replace("\n", "<br/>"));
            return XHtmlHelpers.CreateHtml(value.Replace("\n", "<br/>"), IsGlobalDark);
                //.Replace(@"<img data-src", $@"<img style='max-width:{maxWidthPercent}%' src")  // adapt image size
                //.Replace(@"<div class='cc'>", @"<div>")  // commen div class
                //.Replace(@"href=""/", $@"href=""{htmlFormatHead}")  // correct url fprmat
                //.Replace(@"<div class=""like-btn""", @"<div class=""like-btn"" id='yeslike-btn'")
                //.Replace(@"<div class=""like-btn """, @"<div class=""like-btn"" id='yeslike-btn'")  // add id to like-btn
                //.Replace(@"<div class=""like-btn active""", @"<div class=""like-btn active"" id='dislike-btn'");  // add id to dislike-btn
        }

        /// <summary>
        /// If can not native-done, try to remove something useless.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Design the javascript for webview when first DOMCompleted.
        /// </summary>
        /// <param name="js">scripts</param>
        private void DefineJsFunction(out string js) {
            var bag = new NativeJavascriptBag();
            bag.AppendScript(NativeJavascriptHelper.WindowExternalSender)
                .AppendScript(NativeJavascriptHelper.ActionLinkExpand)
                .AppendScript(NativeJavascriptHelper.ImageClick);
            js = bag.Script;
        }

        #region States
        bool isNative;
        bool isFromInfoClick;
        string htmlReturn;
        string nativeString;
        FrameType frameType;
        DoubleAnimation doubleAnimation;
        #endregion

    }
}
