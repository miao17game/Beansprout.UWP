using static Wallace.UWP.Helpers.Tools.UWPStates;
using static Douban.UWP.NET.Resources.AppResources;

using Douban.UWP.NET.Controls;
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
using Douban.UWP.Core.Models;
using HtmlAgilityPack;
using Douban.UWP.Core.Tools;
using Douban.UWP.NET.Tools;
using Douban.UWP.NET.Models;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Web.Http;
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace Douban.UWP.NET.Pages.SubjectCollectionPages.MoviePages {

    public sealed partial class MovieContentPage : BaseContentPage {
        public MovieContentPage() {
            this.InitializeComponent();
        }

        #region Events

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            if (!(e.Parameter is NavigateParameter))
                return;
            var args = e.Parameter as NavigateParameter;
            if (isFromInfoClick = args.IsFromInfoClick)
                GlobalHelpers.SetChildPageMargin(this, matchNumber: VisibleWidth, isDivideScreen: IsDivideScreen);
            frameType = args.FrameType;
            SetWebViewSourceAsync(currentUri = args.ToUri);
        }

        private void BaseHamburgerButton_Click(object sender, RoutedEventArgs e) {
            PageSlideOutStart(VisibleWidth > FormatNumber ? false : true);
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e) {
            model = this.DataContext as MovieContentVM;
        }

        private void FullContentBtn_Click(object sender, RoutedEventArgs e) {
            NavigateToBase?.Invoke(
                null,
                new NavigateParameter { FrameType = FrameType.UpContent, ToUri = currentUri, Title = GetUIString("LinkContent") },
                GetFrameInstance(FrameType.UpContent),
                GetPageType(NavigateType.Undefined));
        }

        private void WishBtn_Click(object sender, RoutedEventArgs e) {

        }

        private void CollectBtn_Click(object sender, RoutedEventArgs e) {

        }

        private void ImageGridView_ItemClick(object sender, ItemClickEventArgs e) {
            var path = e.ClickedItem as string;
            if (path == null)
                return;
            ShowImageInScreen(new Uri(path));
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e) {
            GlobalHelpers.SetChildPageMargin(this, matchNumber: VisibleWidth, isDivideScreen: IsDivideScreen);
            BackImage.Height = grid.ActualHeight;
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
                var result = htmlReturn = await DoubanWebProcess.GetMDoubanResponseAsync(
                    path: uri.ToString(),
                    host: "m.douban.com",
                    reffer: "https://m.douban.com/");
                var doc = new HtmlDocument();
                doc.LoadHtml(result);
                var root = doc.DocumentNode;
                if (model == null)
                    return;
                model.Title = root.GetNodeFormat("h1", "class", "title")?.InnerText;
                model.Cover = root.GetNodeFormat("img", "class", "cover").Attributes["src"].Value;
                var meta = root.GetNodeFormat("p", "class", "meta").InnerText.Replace(" ", "").Replace(@"\n", "");
                model.Meta = meta.Substring(1, meta.Length - 2).Replace("/", " / ");
                model.Rating = Convert.ToDouble(root.GetNodeFormat("meta", "itemprop", "ratingValue").Attributes["content"].Value);
                model.CommentersCount = root.GetNodeFormat("meta", "itemprop", "reviewCount").Attributes["content"].Value;
                var descrip_group = root.GetSectionNodeContentByClass("subject-intro");
                if (descrip_group != null) {
                    model.Intro = descrip_group.GetNodeFormat("div", "class", "bd", false).SelectSingleNode("p").InnerText.Replace("。", "。\n"); ;
                }
                var img_group = root.GetSectionNodeContentByClass("subject-pics");
                if (img_group != null) {
                    var new_list = new List<string>();
                    var lists = img_group
                        .GetNodeFormat("div", "class", "bd photo-list", false)
                        .GetNodeFormat("ul", "class", "wx-preview", false)
                        .SelectNodes("li[@class='pic']");
                    lists.ToList().ForEach(i => new_list.Add(i.SelectSingleNode("a").SelectSingleNode("img").Attributes["src"].Value));
                    model.ImageList = new_list;
                }

            } catch {
                System.Diagnostics.Debug.WriteLine("Bug");
            } finally { IncrementalLoadingBorder.SetVisibility(false); }
        }

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
        bool isFromInfoClick;
        FrameType frameType;
        MovieContentVM model;
        #endregion
    }
}
