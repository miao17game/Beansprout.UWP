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

        protected override void InitPageState() {
            base.InitPageState();
            GlobalHelpers.DivideWindowRange(this, DivideNumber, isDivideScreen: IsDivideScreen);
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

        public override void PageSlideOutStart(bool isToLeft) {
            base.PageSlideOutStart(isToLeft);
            Scroll.Navigate(new Uri("https://www.none-wallace-767fc6vh7653df0jb.com/no_wallace_085sgdfg7447fddds65.jpg"));
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e) {
            GlobalHelpers.SetChildPageMargin(this, matchNumber: VisibleWidth, isDivideScreen: IsDivideScreen);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            SetPageLoadingStatus();
            var args = e.Parameter as NavigateParameter;
            isFromInfoClick = args.IsFromInfoClick;
            if (!isFromInfoClick)
                GlobalHelpers.SetChildPageMargin(this, matchNumber: VisibleWidth, isDivideScreen: IsDivideScreen);
            if (args == null)
                return;
            if (args.Title != null)
                navigateTitlePath.Text = args.Title;
            SetWebViewSourceAsync(currentUri = args.ToUri);
        }

        private async void SetWebViewSourceAsync(Uri uri) {
            try {
                var result = await DoubanWebProcess.GetMDoubanResponseAsync(uri.ToString());
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(result);
                Scroll.NavigateToString(ConnectString(RemoveString(doc)));
            } catch {
                Scroll.Source = uri;
            }
        }

        private string RemoveString(HtmlDocument doc) {
            var value = doc.DocumentNode;
            var navi = value.SelectSingleNode("//div[@id='TalionNav']");
            if (navi != null)
                navi.Remove();
            var top = value.SelectSingleNode("//section[@class='promo_top_banner']");
            if (top != null)
                top.Remove();
            var tags = value.SelectSingleNode("//section[@class='tags']");
            if (tags != null)
                tags.Remove();
            var comments = value.SelectSingleNode("//section[@class='note-comments']");
            if (comments != null)
                comments.Remove();
            var center = value.SelectSingleNode("//section[@class='center']");
            if (center != null)
                center.Remove();
            var userNote = value.SelectSingleNode("//section[@class='user-notes']");
            if (userNote != null)
                userNote.Remove();
            var relatedMore = value.SelectSingleNode("//section[@class='related-more']");
            if (relatedMore != null)
                relatedMore.Remove();
            var toHomePage = value.SelectSingleNode("//div[@class='tohomepage']");
            if (toHomePage != null)
                toHomePage.Remove();
            var themesWidgets = value.SelectSingleNode("//section[@id='ThemesWidget']");
            if (themesWidgets != null)
                themesWidgets.Remove();
            var downloadApp = value.SelectSingleNode("//div[@class='download-app']");
            if (downloadApp != null)
                downloadApp.Remove();
            return value.InnerHtml;
        }

        private string ConnectString(string value) {
            return value;
        }

        private void SetPageLoadingStatus() {
            DoubanLoading.SetVisibility(false);
            IncrementalLoadingBorder.SetVisibility(true);
            IncrementalLoading.SetVisibility(true);
        }

        private void FullContentBtn_Click(object sender, RoutedEventArgs e) {
            Scroll.Source = currentUri;
        }

        private void BaseHamburgerButton_Click(object sender, RoutedEventArgs e) {
            if (Scroll.CanGoBack)
                Scroll.GoBack();
            else
                PageSlideOutStart(VisibleWidth > 800 ? false : true);
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
        bool isFromInfoClick = false;
        #endregion
    }
}
