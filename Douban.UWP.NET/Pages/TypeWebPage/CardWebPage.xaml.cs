#region Using
using static Wallace.UWP.Helpers.Tools.UWPStates;
using static Douban.UWP.NET.Resources.AppResources;

using Douban.UWP.Core.Tools;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Windows.UI;
using Wallace.UWP.Helpers;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Douban.UWP.NET.Models;
using Windows.UI.Xaml.Media.Animation;
using Wallace.UWP.Helpers.SDK;
using Windows.ApplicationModel.DataTransfer;
using Wallace.UWP.Helpers.Tools;
using Douban.UWP.Core.Tools.PersonalExpressions;
#endregion

namespace Douban.UWP.NET.Pages.TypeWebPage {

    public sealed partial class CardWebPage : BaseContentPage {

        public CardWebPage() {
            this.InitializeComponent();
            DoubanLoading.SetVisibility(false);
            var shareManager = DataTransferManager.GetForCurrentView();
            shareManager.DataRequested += DataTransferManager_DataRequested;
        }

        #region Events

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
            if (isNative = args.IsNative)
                SetWebViewSourceAsync(currentUri = args.ToUri);
            else
                WebView.Source = currentUri = args.ToUri;
        }

        private void FullContentBtn_Click(object sender, RoutedEventArgs e) {
            NavigateToBase?.Invoke(
                       null,
                       new NavigateParameter { FrameType = FrameType.UpContent, ToUri = currentUri, Title = GetUIString("LinkContent") },
                       GetFrameInstance(FrameType.UpContent),
                       GetPageType(NavigateType.Undefined));
        }

        private void BaseHamburgerButton_Click(object sender, RoutedEventArgs e) {
            if (WebView.CanGoBack)
                WebView.GoBack();
            else
                PageSlideOutStart(VisibleWidth > FormatNumber ? false : true);
        }

        private void RelativePanel_Loaded(object sender, RoutedEventArgs e) {
            defaultHeight = (sender as RelativePanel).ActualHeight;
        }

        private void AuthorLinkBtn_Click(object sender, RoutedEventArgs e) {
            var uri = (sender as Button).CommandParameter as Uri;
            if (uri == null)
                return;
            var res = new Regex(@"people/.+?/").Match(uri.ToString()).Value;
            if (res == "")
                return;
            NavigateToBase?.Invoke(
                       null,
                       new NavigateParameter { FrameType = FrameType.UpContent, UserUid = new Regex(@"[0-9]{5,}").Match(res).Value },
                       GetFrameInstance(FrameType.UpContent),
                       GetPageType(NavigateType.UserInfo));
        }

        private void CommentsBtn_Click(object sender, RoutedEventArgs e) {
            ReportHelper.ReportAttentionAsync(GetUIString("StillInDeveloping"));
        }

        private async void LikedBtn_ClickAsync(object sender, RoutedEventArgs e) {
            var js = @"
                    var likebtn = document.getElementById('yeslike-btn');
                    if(likebtn!=null){
                        likebtn.click();
                    }
                    var dislikebtn = document.getElementById('dislike-btn');
                    if(dislikebtn!=null){
                        dislikebtn.click();
                    }";
            await WebView.InvokeScriptAsync("eval", new[] { js });
        }

        private async void ShareBtn_ClickAsync(object sender, RoutedEventArgs e) {
            var name = (sender as MenuFlyoutItem).Name;
            switch (name) {
                case "WeixinShare":
                case "WeixinTimeLineShare":
                    await ShareToWeixinAsync(name);
                    break;
                case "WeiboShare":
                    await ShareBySystemAPIAsync(name);
                    break;
                case "OthersShare":
                    await ShareBySystemAPIAsync(name);
                    break;
                default:
                    ReportHelper.ReportAttentionAsync(GetUIString("StillInDeveloping"));
                    break;
            }
        }

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args) {
            var request = args.Request;
            request.Data.Properties.Description = description;
            request.Data.Properties.Title = title;
            request.Data.Properties.ApplicationName = GetUIString("AppName");
            request.Data.Properties.Thumbnail = ImageHelpers.CreateThumbnailFromUri(new Uri(thumb));
            request.Data.SetBitmap(RandomAccessStreamReference.CreateFromUri(new Uri(thumb)));
            request.Data.SetStorageItems(new List<StorageFile> { tempImageForShare });
            request.Data.SetText(EscapeReplace.ToEscape(title) + " \n " + currentUri.ToString());
            shareType = ShareType.System;
        }

        private void Scroll_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e) {
            ChangeBarGridAnima(ref defaultHeight, (sender as ScrollViewer).VerticalOffset);
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
            InitStoryBoard();
            DefineJsFunction(out var js);
            await sender.InvokeScriptAsync("eval", new[] { js });
        }

        private void WebView_SizeChangedAsync(object sender, SizeChangedEventArgs e) {
            if (!isDOMLoaded)
                return;
            if (IsMobile)
                return;
            isNeedChange = false;
            if (isChangeFinished) {
                isChangeFinished = false;
                return;
            }
            WebViewHeightTimerInit();
        }

        private void WebView_ScriptNotifyAsync(object sender, NotifyEventArgs e) {
            var callBack = JsonHelper.FromJson<string>(e.Value);

            //Debug.WriteLine("Notify CallBack ---->  :  " + e.Value);

            SetScroolHeight(callBack);
            SetScroolTop(callBack);
            SetActionLink(callBack);
            SetPictureShow(callBack);
            SetLikeBtnAsync(callBack);
            SetIsLikedBtnState(callBack);
        }

        private async void ImageSaveButton_ClickAsync(object sender, RoutedEventArgs e) {
            SoftwareBitmap sb = await DownloadImageAsync((sender as Button).CommandParameter as string);
            if (sb != null) {
                SoftwareBitmapSource source = new SoftwareBitmapSource();
                await source.SetBitmapAsync(sb);
                var name = await WriteToFileAsync(sb);
                new ToastSmooth(GetUIString("SaveImageSuccess")).Show();
            }
        }

        private void ImageControlButton_Click(object sender, RoutedEventArgs e) {
            ImagePopup.IsOpen = false;
        }

        private void ImagePopup_SizeChanged(object sender, SizeChangedEventArgs e) {
            ImagePopupBorder.Width = (sender as Popup).ActualWidth;
            ImagePopupBorder.Height = (sender as Popup).ActualHeight;
        }

        #endregion

        #region Methods

        #region Commen

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
            DoubanLoading.SetVisibility(false);
            IncrementalLoadingBorder.SetVisibility(true);
            IncrementalLoading.SetVisibility(true);
        }

        private async void SetWebViewSourceAsync(Uri uri) {
            try {
                var result = htmlReturn = await DoubanWebProcess.GetMDoubanResponseAsync(uri.ToString());
                var doc = new HtmlDocument();
                doc.LoadHtml(result);
                WebView.NavigateToString(GetContent(doc.DocumentNode));
                SetTitleAndDescForShare(doc);
                SetAuthorGrid();
            } catch {
                WebView.Source = uri;
            }
        }

        private void SetTitleAndDescForShare(HtmlDocument doc) {
            try { title = doc.DocumentNode.SelectSingleNode("//title").InnerText.Replace(" ", "").Replace("\n", ""); } catch { title = ""; }
            try { description = doc.DocumentNode.SelectSingleNode("meta", "name", "description", true).InnerText.Replace(" ", "").Replace("\n", ""); } catch { description = ""; }
            try { thumb = doc.DocumentNode.SelectSingleNode("meta", "property", "weixin:image", true).Attributes["content"].Value; } catch { thumb = ""; }
        }

        private void SetAuthorGrid() {
            try {
                var docm = new HtmlDocument();
                docm.LoadHtml(htmlReturn);
                var author = docm.DocumentNode.SelectSingleNode("section", "class", "author", isIgnoreGeneration: true);
                var context = BarGrid.DataContext as AuthorVM;
                if (context == null)
                    return;
                var action = author.SelectSingleNode("a", "class", "note-author");
                Uri.TryCreate(action.SelectSingleNode("img").Attributes["data-src"].Value, UriKind.RelativeOrAbsolute, out var imgUri);
                Uri.TryCreate(htmlFormatHead + action.Attributes["href"].Value, UriKind.RelativeOrAbsolute, out var linkUri);
                context.Image = imgUri;
                context.Link = linkUri;
                var author_infos = action.SelectSingleNode("div","class","author-info");
                var author_details = author_infos.SelectSingleNode("div","class","author-details");
                context.UserName = author_infos.SelectSingleNode("span","class","author-name").InnerText;
                context.Notes = author_details.SelectSingleNode("span","class","notes").InnerText;
                context.Albums = author_details.SelectSingleNode("span","class","albums").InnerText;
            } catch { /* Ignore */ }
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

        #endregion

        #region Shares

        private async Task ShareToWeixinAsync(string name) {
            shareType = name == "WeixinShare" ? 
                ShareType.WechatSession : 
                ShareType.WechatTimeLine;
            var doc = new HtmlDocument();
            doc.LoadHtml(nativeString);
            var bytes = await SDKHelpers.ReadResFromImageAsync(thumb);
            await SDKHelpers.SendWechatShareToUserChoiceRequestAsync(
                url : currentUri.ToString(), 
                title : title, 
                thumb : bytes, 
                desc : description, 
                toTimeLine : shareType);
        }

        private async Task ShareBySystemAPIAsync(string name) {
            shareType = name == "WeiboShare" ? 
                ShareType.Weibo : 
                ShareType.System;
            
            tempImageForShare = await ImageHelpers.GetSoftwareBitmapFromUriStringAsync(thumb);
            DataTransferManager.ShowShareUI();
        }

        #endregion

        #region Web Native

        /// <summary>
        /// Design the javascript for webview when first DOMCompleted.
        /// </summary>
        /// <param name="js">scripts</param>
        private void DefineJsFunction(out string js) {
            js = DoWorkForActionLink() + DoWorkForImages() + DoWorkForLikeBtn() + DoWorkForMobileScrollEvent();
            if (IsMobile)
                WebView.Height = defaultHeight;
            else
                js += DoWorkForScrollHide();
        }

        /// <summary>
        /// Get the native-done web string.
        /// </summary>
        /// <param name="node">htmlRoot</param>
        /// <returns></returns>
        private string GetContent(HtmlNode node) {
            return node.ContainsFormat("div", "class", "rich-note") ? NativeStringConnect(node, node.GetHtmlFormat("div", "class", "rich-note")) :
                node.ContainsFormat("div", "class", "full") ? NativeStringConnect(node, node.GetHtmlFormat("div", "class", "full")) :
                ConnectString(RemoveString(node));
        }

        /// <summary>
        /// Connect body-string and anyother useful html-nodes
        /// </summary>
        /// <param name="node"></param>
        /// <param name="content">body-content</param>
        /// <returns></returns>
        private string NativeStringConnect(HtmlNode node, string content) {
            return nativeString = WebStringNative(
                //GetSectionByClass(node, "header").Replace("  ", "") +
                content +
                GetSectionByClass(node, "author").Replace("  ", ""));
        }

        /// <summary>
        /// Global Native-work for the web-string.
        /// </summary>
        /// <param name="value">web string contrnt</param>
        /// <returns></returns>
        private string WebStringNative(string value) {
            var maxWidthPercent = "100";
            return XHtmlHelpers.CreateHtml(value.Replace("\n", "<br/>"), IsGlobalDark)
                .Replace(@"<img data-src", $@"<img style='max-width:{maxWidthPercent}%' src")  // adapt image size
                .Replace(@"<div class='cc'>", @"<div>")  // commen div class
                .Replace(@"href=""/", $@"href=""{htmlFormatHead}")  // correct url fprmat
                .Replace(@"<div class=""like-btn""", @"<div class=""like-btn"" id='yeslike-btn'")
                .Replace(@"<div class=""like-btn """, @"<div class=""like-btn"" id='yeslike-btn'")  // add id to like-btn
                .Replace(@"<div class=""like-btn active""", @"<div class=""like-btn active"" id='dislike-btn'");  // add id to dislike-btn
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
        /// Connect something, but still in undefined...
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string ConnectString(string value) {
            return value;
        }

        private void ChangeBarGridAnima(ref double oldOne, double newOne) {
            if (!isMobileAnimaCompleted)
                return;
            if (newOne - oldOne < -20 && BarGrid.Visibility == Visibility.Collapsed) {
                isMobileAnimaCompleted = false;
                BarGrid.SetVisibility(true);
                Scroll.ViewChanged -= Scroll_ViewChanged;
                BtnStackSlideIn.Begin();
            } else if (newOne - oldOne > 20 && BarGrid.Visibility == Visibility.Visible) {
                isMobileAnimaCompleted = false;
                Scroll.ViewChanged -= Scroll_ViewChanged;
                BtnStackSlideOut.Begin();
            }
            oldOne = newOne;
        }

        private void ChangeLikedBtnStateByBoolen(bool is_liked) {
            LikedBtn.Foreground = new SolidColorBrush(is_liked ? Color.FromArgb(255, 217, 6, 94) : Colors.White);
        }

        #region BarGrid Animations

        private void InitStoryBoard() {
            transToBarGrid = BarGrid.RenderTransform as TranslateTransform;
            if (transToBarGrid == null) BarGrid.RenderTransform = transToBarGrid = new TranslateTransform();

            doubleAnimation = new DoubleAnimation {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
                Duration = new Duration(TimeSpan.FromMilliseconds(520)),
                From = 150,
                To = 0,
            };
            doubleAnimation.Completed += DoublAnimationIn_Completed;
            BtnStackSlideIn = new Storyboard();
            Storyboard.SetTarget(doubleAnimation, transToBarGrid);
            Storyboard.SetTargetProperty(doubleAnimation, "Y");
            BtnStackSlideIn.Children.Add(doubleAnimation);

            doubleAnimation = new DoubleAnimation {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
                Duration = new Duration(TimeSpan.FromMilliseconds(520)),
                From = 0,
                To = 150,
            };
            doubleAnimation.Completed += DoublAnimationOut_Completed;
            BtnStackSlideOut = new Storyboard();
            Storyboard.SetTarget(doubleAnimation, transToBarGrid);
            Storyboard.SetTargetProperty(doubleAnimation, "Y");
            BtnStackSlideOut.Children.Add(doubleAnimation);
        }

        private void DoublAnimationIn_Completed(object sender, object e) {
            Scroll.ViewChanged += Scroll_ViewChanged;
            isMobileAnimaCompleted = true;
        }

        private void DoublAnimationOut_Completed(object sender, object e) {
            BarGrid.SetVisibility(false);
            Scroll.ViewChanged += Scroll_ViewChanged;
            isMobileAnimaCompleted = true;
        }

        #endregion

        #region Get HtmlNode by Format

        private string GetSectionByClass(HtmlNode node, string className) {
            return node.GetHtmlFormat("section", "class", className);
        }

        private string GetDivByClass(HtmlNode node, string className) {
            return node.GetHtmlFormat("div", "class", className);
        }

        #endregion

        #region Scroll Height Change

        private async void ChangeWebViewHeightAsync() {
            var js = @"window.external.notify(JSON.stringify('scrollheight:'+document.body.scrollHeight));";
            await WebView.InvokeScriptAsync("eval", new[] { js });
        }

        #endregion

        #region Regex of CallBack

        private void SetScroolHeight(string callBack) {
            var scrollMatch = new Regex(@"scrollheight:.+").Match(callBack);
            if (scrollMatch.Value == "")
                return;
            var formatStr = scrollMatch.Value.Substring(13);
            try {
                WebView.Height = Convert.ToDouble(formatStr);
            } catch { /* Ignore */ }
        }

        private void SetScroolTop(string callBack) {
            var scrollMatch = new Regex(@"scrolltop:.+").Match(callBack);
            if (scrollMatch.Value == "")
                return;
            var formatStr = scrollMatch.Value.Substring(10);
            try {
                ChangeBarGridAnima(ref defaultHeight, Convert.ToDouble(formatStr));
            } catch { /* Ignore */ }
        }

        private void SetActionLink(string callBack) {
            var actionMatch = new Regex(@"actionlink:.+").Match(callBack);
            if (actionMatch.Value == "")
                return;
            var formatStr = actionMatch.Value.Substring(11);
            Uri.TryCreate(formatStr, UriKind.Absolute, out var uri);
            if (uri != null)
                NavigateToBase?.Invoke(
                       null,
                       new NavigateParameter { FrameType = FrameType.UpContent, Title = GetUIString("LinkContent"), ToUri = uri },
                       GetFrameInstance(FrameType.UpContent),
                       GetPageType(NavigateType.Undefined));
        }

        private void SetPictureShow(string callBack) {
            var pictureMatch = new Regex(@"picturelink:.+").Match(callBack);
            if (pictureMatch.Value == "")
                return;
            var formatStr = pictureMatch.Value.Substring(12);
            Uri.TryCreate(formatStr, UriKind.Absolute, out var uri);
            if (uri != null)
                ShowImageInScreen(uri);
        }

        private void SetIsLikedBtnState(string callBack) {
            var pictureMatch = new Regex(@"isliked:.+").Match(callBack);
            if (pictureMatch.Value == "")
                return;
            var formatStr = pictureMatch.Value.Substring(8);
            bool is_liked = Convert.ToBoolean(formatStr);
            ChangeLikedBtnStateByBoolen(is_liked);
        }

        private async void SetLikeBtnAsync(string callBack) {
            var likebtnMatch = new Regex(@"like-note-link:.+").Match(callBack);
            if (likebtnMatch.Value == "")
                return;
            var formatStr = "https://m.douban.com" + likebtnMatch.Value.Substring(15);
            Uri.TryCreate(formatStr, UriKind.Absolute, out var uri);
            if (uri != null)
                try {
                    var result = await DoubanWebProcess.GetMDoubanResponseAsync(uri.ToString());
                    var jo = JObject.Parse(result);
                    var data = jo["data"];
                    if (data != null && data.HasValues) {
                        var isliked = data["is_liked"].Value<bool>();
                        var number = data["n_likers"].Value<int>();
                        var className = isliked ? "like-btn active" : "like-btn";
                        var addStr = isliked ? "unlike" : "like";
                        var js = $@"
                            var likebtn = document.getElementById('yeslike-btn');
                            if(likebtn!=null){"{"}
                                likebtn.className = '{className}';
                                likebtn.innerText = {number};
                                likebtn.setAttribute('onclick','send_path_url(""like-note-link:'+ likebtn.getAttribute('data-url') +'/{addStr}"")');
                            {"}"}
                            var dislikebtn = document.getElementById('dislike-btn');
                            if(dislikebtn!=null){"{"}
                                dislikebtn.className = '{className}';
                                dislikebtn.innerText = {number};
                                dislikebtn.setAttribute('onclick','send_path_url(""like-note-link:'+ likebtn.getAttribute('data-url') +'/{addStr}"")');
                            {"}"}
                            ";
                        await WebView.InvokeScriptAsync("eval", new[] { js });
                        ChangeLikedBtnStateByBoolen(isliked);
                    }
                } catch { /* Ignore */ }
        }

        #endregion

        #region JS Functions

        private static string DoWorkForScrollHide() {
            return @"
                     document.body.style.overflow = 'hidden';
                     window.external.notify(JSON.stringify('scrollheight:'+document.body.scrollHeight));";
        }

        private static string DoWorkForActionLink() {
            return @"
                    var coll = document.getElementsByTagName('a');
                    for(i=0;i<coll.length;i++){
                        coll[i].setAttribute('onclick','send_path_url(""actionlink:'+ coll[i].getAttribute('href') +'"")');
                        coll[i].setAttribute('href','');
                    }
                ";
        }

        private static string DoWorkForImages() {
            return @"
                    var pics = document.getElementsByTagName('img');
                    for(i=0;i<pics.length;i++){
                        pics[i].setAttribute('onclick','send_path_url(""picturelink:'+ pics[i].getAttribute('src') +'"")');
                    }";
        }

        private static string DoWorkForLikeBtn() {
            return @"
                    var likebtn = document.getElementById('yeslike-btn');
                    if(likebtn!=null){
                        likebtn.setAttribute('onclick','send_path_url(""like-note-link:'+ likebtn.getAttribute('data-url') +'/like"")');
                        window.external.notify(JSON.stringify('isliked:'+'false'));
                    }
                    var dislikebtn = document.getElementById('dislike-btn');
                    if(dislikebtn!=null){
                        dislikebtn.setAttribute('onclick','send_path_url(""like-note-link:'+ dislikebtn.getAttribute('data-url') +'/unlike"")');
                        window.external.notify(JSON.stringify('isliked:'+'true'));
                    }";
        }

        private static string DoWorkForMobileScrollEvent() {
            return @"
                    window.onscroll = function () {  
                        window.external.notify(JSON.stringify('scrolltop:'+document.body.scrollTop));
                    };";
        }

        #endregion

        #endregion

        #region Show Images

        public void ShowImageInScreen(Uri imageSource) {
            ImageSaveButton.CommandParameter = imageSource.ToString();
            ImageScreen.Source = new BitmapImage(imageSource);
            ImagePopup.IsOpen = true;
        }

        private async Task<SoftwareBitmap> DownloadImageAsync(string url) {
            try {
                HttpClient hc = new HttpClient();
                HttpResponseMessage resp = await hc.GetAsync(new Uri(url));
                resp.EnsureSuccessStatusCode();
                IInputStream inputStream = await resp.Content.ReadAsInputStreamAsync();
                IRandomAccessStream memStream = new InMemoryRandomAccessStream();
                await RandomAccessStream.CopyAsync(inputStream, memStream);
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(memStream);
                SoftwareBitmap softBmp = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                return softBmp;
            } catch (Exception) {
                return null;
            }
        }

        public async Task<string> WriteToFileAsync(SoftwareBitmap softwareBitmap) {
            string fileName = "BEANSPROUTUWP" + "-" +
                Guid.NewGuid().ToString() + "-" +
                DateTime.Now.ToString("yyyy-MM--dd-hh-mm-ss") + ".jpg";

            if (softwareBitmap != null) {
                // save image file to cache
                StorageFile file = await (
                    await KnownFolders.PicturesLibrary.CreateFolderAsync("BeansproutUWP", CreationCollisionOption.OpenIfExists))
                    .CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite)) {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                    encoder.SetSoftwareBitmap(softwareBitmap);
                    await encoder.FlushAsync();
                }
            }

            return fileName;
        }

        #endregion

        #endregion

        #region Properties

        string htmlReturn;
        string nativeString;
        string description;
        string title;
        string thumb;
        bool isFromInfoClick = false;
        bool isNative = false;
        bool isDOMLoaded = false;
        bool isNeedChange = false;
        bool isChangeFinished = false;
        bool isMobileAnimaCompleted = true;
        double defaultHeight;
        StorageFile tempImageForShare;
        DispatcherTimer timerForWebView;
        Storyboard BtnStackSlideIn;
        Storyboard BtnStackSlideOut;
        DoubleAnimation doubleAnimation;
        TranslateTransform transToBarGrid;
        FrameType frameType;
        ShareType shareType = ShareType.System;
        const string htmlFormatHead = "https://m.douban.com/";

        #endregion

    }
}
