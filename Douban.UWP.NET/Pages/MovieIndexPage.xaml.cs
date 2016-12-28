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

namespace Douban.UWP.NET.Pages {

    public sealed partial class MovieIndexPage : Page {
        public MovieIndexPage() {
            this.InitializeComponent();
            //GetResourcesAsync();
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e) {

        }

        protected async override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            DoubanLoading.SetVisibility(false);
            InTheaterResources.Source = (await FetchMessageFromAPIAsync(
                "https://frodo.douban.com/jsonp/subject_collection/{0}/items?os=windows&callback=jsonp1&start={1}&count={2}&loc_id={3}&_={4}",
                "movie_showing",
                0,
                8,
                "108288",
                (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds)).Items;
        }

        private async Task<MovieGroupItem> FetchMessageFromAPIAsync(string formatAPI, string group, uint start, uint count, string loc_id, double minised, int offset = 0) {
            var gmodel = default(MovieGroupItem);
            //try {
                var result = await DoubanWebProcess.GetMDoubanResponseAsync(string.Format(formatAPI, new object[] { group, start, count, loc_id, minised }),
                    "frodo.douban.com",
                    "https://m.douban.com/movie/");
                if (result == null) {
                    ReportHelper.ReportAttention(GetUIString("WebActionError"));
                    DoubanLoading.SetVisibility(false);
                    IncrementalLoadingBorder.SetVisibility(false);
                    return gmodel;
                }
                JObject jo = JObject.Parse(result.Substring(7, result.Length - 8));
                var sub_collection = jo["subject_collection"];
                gmodel = new MovieGroupItem {
                    Id = jo["id"].Value<string>(),
                    HasCover = jo["cover_url"].Value<string>() != "" ? true : false,
                    CoverUrl = jo["cover_url"].Value<string>(),
                    GroupInnerUri = jo["uri"].Value<string>(),
                    SharingUrl = jo["sharing_url"].Value<string>(),
                    Description = jo["description"].Value<string>(),
                    GroupPathUrl = jo["url"].Value<string>(),
                    GroupName = sub_collection.HasValues ? sub_collection["name"].Value<string>() : null,
                    NowCount = jo["count"].Value<uint>(),
                    StartIndex = jo["start"].Value<uint>(),
                    Total = jo["total"].Value<uint>(),
                };
                var feeds = jo["subject_collection_items"];
                if (feeds == null || !feeds.HasValues) {
                    ReportHelper.ReportAttention(GetUIString("FetchJsonDataError"));
                    DoubanLoading.SetVisibility(false);
                    IncrementalLoadingBorder.SetVisibility(false);
                    return gmodel;
                }
                if (feeds.HasValues) {
                    gmodel.Items = new List<MovieItem>();
                    feeds.Children().ToList().ForEach(singleton => {
                        //try {
                            var type = MovieItem.ItemType.Movie;
                            var Rating = singleton["rating"];
                            var Cover = singleton["cover"];
                            var Directors = singleton["directors"];
                            var Actors = singleton["actors"];
                            IList<string> dir_list = new List<string>();
                            if (Directors.HasValues) { Directors.ToList().ForEach(pic => { if (pic.Value<string>() != "") { dir_list.Add(pic.Value<string>()); } }); }
                            IList<string> act_list = new List<string>();
                            if (Actors.HasValues) { Directors.ToList().ForEach(pic => { if (pic.Value<string>() != "") { act_list.Add(pic.Value<string>()); } }); }
                            gmodel.Items.Add(new MovieItem {
                                Type = type,
                                OriginalPrice = singleton["original_price"].Value<object>(),
                                Info = singleton["info"].Value<string>(),
                                RatingCount = Rating.HasValues ? Rating["count"].Value<uint>() : 0,
                                RatingMax = Rating.HasValues ? Rating["max"].Value<uint>() : 0,
                                RatingValue = Rating.HasValues ? Rating["value"].Value<double>() : 0.0,
                                Description = singleton["description"].Value<string>(),
                                Title = singleton["title"].Value<string>(),
                                PathUrl = singleton["url"].Value<string>(),
                                ReviewerName = singleton["reviewer_name"].Value<string>(),
                                Price = singleton["price"].Value<object>(),
                                HasCover = Cover.HasValues ? Cover["url"].Value<string>() != "" ? true : false : false,
                                CoverUrl = Cover.HasValues ? Cover["url"].Value<string>() != "" ? new Uri(Cover["url"].Value<string>()) : null : null,
                                CoverWidth = Cover.HasValues ? Cover["width"].Value<double>() : 0.0,
                                CoverHeight = Cover.HasValues ? Cover["height"].Value<double>() : 0.0,
                                CoverShape = Cover.HasValues ? Cover["shape"].Value<string>() : null,
                                PathInnerUri = singleton["uri"].Value<string>(),
                                Actions = null,
                                ID = singleton["id"].Value<uint>(),
                                SubType = singleton["subtype"].Value<string>(),
                                Directors = dir_list,
                                Actors = act_list,
                                Date = singleton["date"].Value<object>(),
                                Label = singleton["label"].Value<object>(),
                                TypeString = singleton["type"].Value<string>(),
                                ReleaseDate = singleton["release_date"].Value<string>(),
                            });
                        //} catch { /* Ignore, item error. */ }
                    });
                }
            //} catch { ReportHelper.ReportAttention(GetUIString("UnknownError")); }
            IncrementalLoadingBorder.SetVisibility(false);
            return gmodel;
        }

    }
}
