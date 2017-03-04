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

namespace Douban.UWP.NET.Pages.TypeWebPage {

    public sealed partial class GenericNativeWebPage : Page {
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
                //var shouldNative = IfCanGetContent(doc.DocumentNode);
                //if (shouldNative) {
                //    WebView.NavigateToString(GetContent(doc.DocumentNode));
                //    SetTitleAndDescForShare(doc);
                //    SetAuthorGrid();
                //} else {
                //    WebView.Source = uri;
                //    BarGrid.SetVisibility(false);
                //    isNative = false;
                //}
            } catch {
                webView.Source = uri;
            }
        }

        /// <summary>
        /// Design the javascript for webview when first DOMCompleted.
        /// </summary>
        /// <param name="js">scripts</param>
        private void DefineJsFunction(out string js) {
            var bag = new NativeJavascriptBag();
            bag.AppendScript(NativeJavascriptHelper.ActionLinkExpand)
                .AppendScript(NativeJavascriptHelper.ImageClick)
                .AppendScript(NativeJavascriptHelper.LikeBtnClick)
                .AppendScript(NativeJavascriptHelper.MobileScrollEvent);
            js = bag.Script;
        }

        bool isNative;
        bool isDOMLoaded;
        bool isFromInfoClick;
        string htmlReturn;
        Uri currentUri;
        FrameType frameType;

        private void webView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args) {

        }

        private void webView_ContentLoading(WebView sender, WebViewContentLoadingEventArgs args) {

        }

        private void webView_SizeChanged(object sender, SizeChangedEventArgs e) {

        }

        private void webView_ScriptNotify(object sender, NotifyEventArgs e) {

        }
    }
}
