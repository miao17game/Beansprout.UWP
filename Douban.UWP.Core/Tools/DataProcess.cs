using DBCSCodePage;
using Wallace.UWP.Helpers.Controls;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

using static Wallace.UWP.Helpers.Tools.UWPStates;
using Douban.UWP.Core.Tools.PersonalExpressions;
using Douban.UWP.Core.Models.ListModel;
using Newtonsoft.Json.Linq;

namespace Douban.UWP.Core.Tools {
    public static class DataProcess {

        #region Properties and State
        
        #endregion

        public static Uri ConvertToUri(string str) { return !string.IsNullOrEmpty(str) ? new Uri(str) : null; }

        public  static ItemGroup<T> SetGroupItem<T>(JObject jo, JToken sub_collection) {
            return new ItemGroup<T> {
                Id = sub_collection["id"].Value<string>(),
                HasCover = sub_collection["cover_url"].Value<string>() != "" ? true : false,
                CoverUrl = sub_collection["cover_url"].Value<string>(),
                GroupInnerUri = sub_collection["uri"].Value<string>(),
                SharingUrl = sub_collection["sharing_url"].Value<string>(),
                Description = sub_collection["description"].Value<string>(),
                GroupPathUrl = sub_collection["url"].Value<string>(),
                GroupName = sub_collection["name"].Value<string>(),
                NowCount = jo["count"].Value<uint>(),
                StartIndex = jo["start"].Value<uint>(),
                Total = jo["total"].Value<uint>(),
                Items = new List<T>(),
            };
        }

        public static void SetEachSingleton(ItemGroup<MovieItem> gmodel, JToken singleton) {
            try {
                var type = ListItemBase.ItemType.Movie;
                var Rating = singleton["rating"];
                var Cover = singleton["cover"];
                var Directors = singleton["directors"];
                var Actors = singleton["actors"];
                var Actions = singleton["actions"];
                IList<string> dir_list = new List<string>();
                if (Directors.HasValues) { Directors.ToList().ForEach(pic => { if (pic.Value<string>() != "") { dir_list.Add(pic.Value<string>()); } }); }
                IList<string> act_list = new List<string>();
                if (Actors.HasValues) { Actors.ToList().ForEach(pic => { if (pic.Value<string>() != "") { act_list.Add(pic.Value<string>()); } }); }
                IList<string> acsi_list = new List<string>();
                if (Actions.HasValues) { Actions.ToList().ForEach(pic => { if (pic.Value<string>() != "") { acsi_list.Add(pic.Value<string>()); } }); }
                gmodel.Items.Add(new MovieItem {
                    Type = type,
                    OriginalPrice = singleton["original_price"].Value<object>(),
                    Info = singleton["info"].Value<string>(),
                    RatingCount = Rating.HasValues ? Rating["count"].Value<uint>() : 0,
                    RatingMax = Rating.HasValues ? Rating["max"].Value<uint>() : 0,
                    RatingValue = Rating.HasValues ? Rating["value"].Value<double>() : 0.0,
                    Description = singleton["description"].Value<string>(),
                    Title = singleton["title"].Value<string>(),
                    DispatchUrl = singleton["url"].Value<string>(),
                    ReviewerName = singleton["reviewer_name"].Value<string>(),
                    Price = singleton["price"].Value<object>(),
                    HasCover = Cover.HasValues ? Cover["url"].Value<string>() != "" ? true : false : false,
                    CoverUrl = Cover.HasValues ? Cover["url"].Value<string>() != "" ? new Uri(Cover["url"].Value<string>()) : null : null,
                    CoverWidth = Cover.HasValues ? Cover["width"].Value<double>() : 0.0,
                    CoverHeight = Cover.HasValues ? Cover["height"].Value<double>() : 0.0,
                    CoverShape = Cover.HasValues ? Cover["shape"].Value<string>() : null,
                    PathInnerUri = singleton["uri"].Value<string>(),
                    ID = singleton["id"].Value<uint>(),
                    PathUrl = "https://m.douban.com/movie/subject/" + singleton["id"].Value<string>() + "/",
                    SubType = singleton["subtype"].Value<string>(),
                    Directors = dir_list,
                    Actors = act_list,
                    Actions = acsi_list,
                    Date = singleton["date"].Value<object>(),
                    Label = singleton["label"].Value<object>(),
                    TypeString = singleton["type"].Value<string>(),
                    ReleaseDate = singleton["release_date"].Value<string>(),
                });
            } catch { /* Ignore, item error. */ }
        }

        public static void SetEachSingleton(ItemGroup<BookItem> gmodel, JToken singleton) {
            try {
                var type = ListItemBase.ItemType.Book;
                var Rating = singleton["rating"];
                var Cover = singleton["cover"];
                var Actions = singleton["actions"];
                IList<string> acsi_list = new List<string>();
                if (Actions.HasValues) { Actions.ToList().ForEach(pic => { if (pic.Value<string>() != "") { acsi_list.Add(pic.Value<string>()); } }); }
                gmodel.Items.Add(new BookItem {
                    Type = type,
                    OriginalPrice = singleton["original_price"].Value<object>(),
                    Info = singleton["info"].Value<string>(),
                    RatingCount = Rating.HasValues ? Rating["count"].Value<uint>() : 0,
                    RatingMax = Rating.HasValues ? Rating["max"].Value<uint>() : 0,
                    RatingValue = Rating.HasValues ? Rating["value"].Value<double>() : 0.0,
                    Description = singleton["description"].Value<string>(),
                    Title = singleton["title"].Value<string>(),
                    DispatchUrl = singleton["url"].Value<string>(),
                    ReviewerName = singleton["reviewer_name"].Value<string>(),
                    Price = singleton["price"].Value<string>(),
                    HasCover = Cover.HasValues ? Cover["url"].Value<string>() != "" ? true : false : false,
                    CoverUrl = Cover.HasValues ? Cover["url"].Value<string>() != "" ? new Uri(Cover["url"].Value<string>()) : null : null,
                    CoverWidth = Cover.HasValues ? Cover["width"].Value<double>() : 0.0,
                    CoverHeight = Cover.HasValues ? Cover["height"].Value<double>() : 0.0,
                    CoverShape = Cover.HasValues ? Cover["shape"].Value<string>() : null,
                    PathInnerUri = singleton["uri"].Value<string>(),
                    ID = singleton["id"].Value<string>(),
                    PathUrl = singleton["url"].Value<string>(),
                    SubType = singleton["subtype"].Value<string>(),
                    Actions = acsi_list,
                    Date = singleton["date"].Value<object>(),
                    Label = singleton["label"].Value<object>(),
                    TypeString = singleton["type"].Value<string>(),
                    ReleaseDate = singleton["release_date"].Value<object>(),
                });
            } catch { /* Ignore, item error. */ }
        }

        public static void SetEachSingleton(ItemGroup<MusicItem> gmodel, JToken singleton) {
            try {
                var type = ListItemBase.ItemType.Book;
                var Rating = singleton["rating"];
                var Cover = singleton["cover"];
                var Actions = singleton["actions"];
                IList<string> acsi_list = new List<string>();
                if (Actions.HasValues) { Actions.ToList().ForEach(pic => { if (pic.Value<string>() != "") { acsi_list.Add(pic.Value<string>()); } }); }
                gmodel.Items.Add(new MusicItem {
                    Type = type,
                    OriginalPrice = singleton["original_price"].Value<object>(),
                    Info = singleton["info"].Value<string>(),
                    RatingCount = Rating.HasValues ? Rating["count"].Value<uint>() : 0,
                    RatingMax = Rating.HasValues ? Rating["max"].Value<uint>() : 0,
                    RatingValue = Rating.HasValues ? Rating["value"].Value<double>() : 0.0,
                    Description = singleton["description"].Value<string>(),
                    Title = singleton["title"].Value<string>(),
                    DispatchUrl = singleton["url"].Value<string>(),
                    ReviewerName = singleton["reviewer_name"].Value<string>(),
                    Price = singleton["price"].Value<string>(),
                    HasCover = Cover.HasValues ? Cover["url"].Value<string>() != "" ? true : false : false,
                    CoverUrl = Cover.HasValues ? Cover["url"].Value<string>() != "" ? new Uri(Cover["url"].Value<string>()) : null : null,
                    CoverWidth = Cover.HasValues ? Cover["width"].Value<double>() : 0.0,
                    CoverHeight = Cover.HasValues ? Cover["height"].Value<double>() : 0.0,
                    CoverShape = Cover.HasValues ? Cover["shape"].Value<string>() : null,
                    PathInnerUri = singleton["uri"].Value<string>(),
                    ID = singleton["id"].Value<string>(),
                    PathUrl = "https://m.douban.com/music/subject/" + singleton["id"].Value<string>() + "/",
                    SubType = singleton["subtype"].Value<string>(),
                    Actions = acsi_list,
                    Date = singleton["date"].Value<object>(),
                    Label = singleton["label"].Value<object>(),
                    TypeString = singleton["type"].Value<string>(),
                    ReleaseDate = singleton["release_date"].Value<object>(),
                });
            } catch { /* Ignore, item error. */ }
        }

    }
}
