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

namespace Douban.UWP.NET.Pages.TypeWebPage {

    public sealed partial class GenericNativeWebPage : BaseContentPage {
        public GenericNativeWebPage() {
            this.InitializeComponent();
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
            if (isNative = args.IsNative)
                SetWebViewSourceAsync(currentUri = args.ToUri);
            else
                webView.Source = currentUri = args.ToUri;
        }

        private async void WebView_DOMContentLoadedAsync(WebView sender, WebViewDOMContentLoadedEventArgs args) {
            IncrementalLoadingBorder.SetVisibility(false);
            isDOMLoaded = true;
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

            System.Diagnostics.Debug.WriteLine("Notify CallBack ---->  :  " + e.Value);

            //SetScroolHeight(callBack);
            //SetScroolTop(callBack);
            //SetActionLink(callBack);
            //SetPictureShow(callBack);
            //SetLikeBtnAsync(callBack);
            //SetIsLikedBtnState(callBack);
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

        private async void SetWebViewSourceAsync(Uri uri) {
            try {
                var result = htmlReturn = await DoubanWebProcess.GetMDoubanResponseAsync(
                    path: uri.ToString(),
                    host: "m.douban.com",
                    reffer: "https://m.douban.com/");
                var doc = new HtmlDocument();
                doc.LoadHtml(result);
                var shouldNative = IfCanGetContent(doc.DocumentNode);
                if (shouldNative) {
                    webView.NavigateToString(GetContent(doc.DocumentNode));
                } else {
                    webView.Source = uri;
                    isNative = false;
                }
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
            return node.ContainsFormat("div", "class", "rich-note") ? NativeStringConnect(node, node.GetHtmlFormat("div", "class", "rich-note")) :
                node.ContainsFormat("div", "class", "full") ? NativeStringConnect(node, node.GetHtmlFormat("div", "class", "full")) :
                RemoveString(node);
        }

        private bool IfCanGetContent(HtmlNode node) {
            return node.ContainsFormat("div", "class", "rich-note") ? true :
                node.ContainsFormat("div", "class", "full") ? true :
                false;
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
            return XHtmlHelpers.CreateDefaultHtml(value.Replace("\n", "<br/>"));
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
        bool isDOMLoaded;
        bool isFromInfoClick;
        string htmlReturn;
        string nativeString;
        FrameType frameType;
        #endregion
    }
}
