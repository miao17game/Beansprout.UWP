using static Wallace.UWP.Helpers.Tools.UWPStates;
using static Douban.UWP.NET.Resources.AppResources;

using Douban.UWP.Core.Models.ListModel;
using Douban.UWP.Core.Tools;
using Douban.UWP.NET.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json.Linq;
using Windows.UI;
using Douban.UWP.Core.Models;

namespace Douban.UWP.NET.Pages {

    public sealed partial class MovieIndexPage : Page {
        public MovieIndexPage() {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            DoubanLoading.SetVisibility(false);
            var InThearerResult = await SetGridViewResourcesAsync("movie_showing");
            InTheaterResources.Source = InThearerResult != null ? InThearerResult.Items : null;
            var WatchOnlineResult = await SetGridViewResourcesAsync("movie_free_stream");
            WatchOnlineResources.Source = WatchOnlineResult != null ? WatchOnlineResult.Items : null;
            var LatestResult = await SetGridViewResourcesAsync("movie_latest");
            LatestResources.Source = LatestResult != null ? LatestResult.Items : null;
            var webResult = await DoubanWebProcess.GetMDoubanResponseAsync("https://m.douban.com/movie/");
            SetWrapPanelResources(webResult);
            SetFilterResources(webResult);
        }

        private void SetWrapPanelResources(string webResult) {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(webResult);
            var wrapList = new List<MovieGroupItem>();
            var lis = doc.DocumentNode
                    .SelectSingleNode("//section[@class='interests']")
                    .SelectSingleNode("div[@class='section-content']")
                    .SelectSingleNode("ul")
                    .SelectNodes("li");
            lis.ToList().ForEach(singleton => {
                var actionL = singleton.SelectSingleNode("a");
                if (actionL != null)
                    wrapList.Add(new MovieGroupItem { GroupName = actionL.InnerText, GroupPathUrl = actionL.Attributes["href"].Value, });
            });
            var randomer = new Random();
            wrapList.ForEach(i => {
                var color = GlobalHelpers.GetColorRandom((randomer.Next())%15);
                var button = new Button { Content = i.GroupName, Background = new SolidColorBrush(Colors.Transparent), Foreground = color };
                button.Click += (obj, args) => NavigateToBase?.Invoke(
                    null,
                    new NavigateParameter { ToUri = new Uri(i.GroupPathUrl) },
                    GetFrameInstance(NavigateType.DouList),
                    GetPageType(NavigateType.DouList));
                WrapPanel.Children.Add(new Border {
                    CornerRadius = new CornerRadius(3),
                    BorderBrush = color,
                    BorderThickness = new Thickness(1),
                    Margin = new Thickness(3),
                    Child = button
                });
            });
        }

        private void SetFilterResources(string webResult) {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(webResult);
            var filterList = new List<MovieGroupItem>();
            var lis = doc.DocumentNode
                    .SelectSingleNode("//section[@class='types']")
                    .SelectSingleNode("div[@class='section-content']")
                    .SelectSingleNode("ul[@class='type-list']")
                    .SelectNodes("li");
            lis.ToList().ForEach(singleton => {
                var actionL = singleton.SelectSingleNode("a");
                if (actionL != null)
                    filterList.Add(new MovieGroupItem { GroupName = actionL.InnerText, GroupPathUrl = "https://m.douban.com" + actionL.Attributes["href"].Value, });
            });
            FilterResources.Source = filterList;
        }

        private async Task<MovieGroupItem> SetGridViewResourcesAsync(string groupName) {
            return await FetchMessageFromAPIAsync(
                formatAPI: FormatPath,
                group: groupName,
                count: 13,
                loc_id: "108288");
        }

        private async Task<MovieGroupItem> FetchMessageFromAPIAsync(
            string formatAPI, 
            string group,
            string loc_id,
            uint start = 0, 
            uint count = 8, 
            int offset = 0) {
            var gmodel = default(MovieGroupItem);
            try {
                var minised = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                var result = await DoubanWebProcess.GetMDoubanResponseAsync(string.Format(formatAPI, new object[] { group, start, count, loc_id, minised }),
                    "frodo.douban.com",
                    "https://m.douban.com/movie/");
                if (result == null) {
                    FireLoadingAnimations("WebActionError");
                    return gmodel;
                }
                JObject jo = JObject.Parse(result.Substring(7, result.Length - 8));
                gmodel = SetGroupResources(jo, gmodel);
                gmodel = SetSingletonResources(jo, gmodel);
            } catch { ReportHelper.ReportAttention(GetUIString("UnknownError")); }
            IncrementalLoadingBorder.SetVisibility(false);
            return gmodel;
        }

        private MovieGroupItem SetGroupResources(JObject jObject, MovieGroupItem gModel) {
            var sub_collection = jObject["subject_collection"];
            if (sub_collection == null || !sub_collection.HasValues) {
                FireLoadingAnimations("FetchJsonDataError");
                return gModel;
            }
            try {
                gModel = DataProcess.SetGroupItem(jObject, sub_collection);
            } catch { /* Ignore, item error. */ }
            return gModel;
        }

        private MovieGroupItem SetSingletonResources(JObject jObject, MovieGroupItem gModel) {
            var feeds = jObject["subject_collection_items"];
            if (feeds == null || !feeds.HasValues) {
                FireLoadingAnimations("FetchJsonDataError");
                return gModel;
            }
            if (feeds.HasValues)
                feeds.Children().ToList().ForEach(singleton => DataProcess.SetEachSingleton(gModel, singleton));
            return gModel;
        }

        private void FireLoadingAnimations(string UIString) {
            ReportHelper.ReportAttention(GetUIString(UIString));
            DoubanLoading.SetVisibility(false);
            IncrementalLoadingBorder.SetVisibility(false);
        }

        private void MoreButton_Click(object sender, RoutedEventArgs e) {

        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e) {
            var item = e.ClickedItem as MovieItem;
            if (item == null || item.PathUrl == null)
                return;
            NavigateToBase?.Invoke(
                null,
                new NavigateParameter { ToUri = new Uri(item.PathUrl) },
                GetFrameInstance(NavigateType.MovieContent),
                GetPageType(NavigateType.MovieContent));
        }

        private void FilterGridView_ItemClick(object sender, ItemClickEventArgs e) {
            var item = e.ClickedItem as MovieGroupItem;
            if (item == null || item.GroupPathUrl == null)
                return;
            NavigateToBase?.Invoke(
                null,
                new NavigateParameter { ToUri = new Uri(item.GroupPathUrl) },
                GetFrameInstance(NavigateType.MovieFilter),
                GetPageType(NavigateType.MovieFilter));
        }

        private void StackPanel_SizeChanged(object sender, SizeChangedEventArgs e) {
            WrapPanel.Width = (sender as StackPanel).ActualWidth;
        }

        #region Properties

        string FormatPath = "https://frodo.douban.com/jsonp/subject_collection/{0}/items?os=windows&callback=jsonp3&start={1}&count={2}&loc_id={3}&_={4}";

        #endregion
        
    }
}
