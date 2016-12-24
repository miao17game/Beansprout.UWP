﻿using static Wallace.UWP.Helpers.Tools.UWPStates;
using static Douban.UWP.NET.Resources.AppResources;

using Douban.UWP.Core.Models.ListModel;
using Douban.UWP.Core.Tools;
using Newtonsoft.Json.Linq;
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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Douban.UWP.NET.Pages {
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ListInfosPage : Page {
        public ListInfosPage() {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            var target = "https://m.douban.com/rexxar/api/v2/recommend_feed?alt=json&next_date=2016-12-25&loc_id=&gender=&birthday=&udid=&for_mobile=true";
            var result = await DoubanWebProcess.GetMDoubanResponseAsync(target);
            JObject jo = JObject.Parse(result);
            var count = jo["recommend_feeds"];
            ICollection<IndexItem> list = new List<IndexItem>();
            if (count.HasValues) {
                count.Children().ToList().ForEach(singleton => {
                    var author = singleton["author"];
                    var column = singleton["column"];
                    var comments = singleton["comments"];
                    var morePic = singleton["more_pic_urls"];
                    List<Uri> more_pic = new List<Uri>();
                    if (morePic.HasValues) {
                        morePic.ToList().ForEach(pic => {
                            if (pic.Value<string>() != "")
                                more_pic.Add(new Uri(pic.Value<string>()));
                        });
                    }
                    var type = 
                    singleton["cover_url"].Value<string>() == "" ? IndexItem.ItemType.Paragraph : 
                    singleton["photos_count"].Value<uint>() == 0 ? IndexItem.ItemType.Normal :
                    IndexItem.ItemType.Gallary;
                    list.Add(new IndexItem {
                        Type = type,
                        ReadCount = singleton["read_count"].Value<uint>(),
                        PhotosCount = singleton["photos_count"].Value<uint>(),
                        ImpressionUrl = singleton["photos_count"].Value<string>(),
                        Title = singleton["title"].Value<string>(),
                        LikersCount = singleton["likers_count"].Value<uint>(),
                        AuthorAvatar = author.HasValues ? author["avatar"].Value<string>()!=""? new Uri(author["avatar"].Value<string>()) : null : null,
                        AuthorName = author.HasValues ? author["name"].Value<string>() : null,
                        PathUrl = singleton["uri"].Value<string>(),
                        Cover = singleton["cover_url"].Value<string>() != "" ? new Uri(singleton["cover_url"].Value<string>()) : null,
                        Source = singleton["source"].Value<string>(),
                        ColumnUrl = column.HasValues ? column["uri"].Value<string>() : null,
                        ColumnId = column.HasValues ? column["id"].Value<int?>() : null,
                        ColumnCover = column.HasValues ? column["cover_url"].Value<string>() != "" ? new Uri(column["cover_url"].Value<string>()) : null : null,
                        ColumnName = column.HasValues ?  column["name"].Value<string>() : null,
                        CommentCount = singleton["comment_count"].Value<uint>(),
                        OutSource = singleton["outsource"].Value<object>(),
                        Comments = null,
                        Action =singleton["action"].Value<object>(),
                        Description = singleton["desc"].Value<string>(),
                        MorePictures = more_pic,
                        ID = singleton["id"].Value<uint>(),
                        MonitorUrl = singleton["monitor_url"].HasValues ? singleton["monitor_url"].Value<string>():null,
                    });
                });
            }
            ListViewResources.Source = list;
            DoubanLoading.SetVisibility(false);
        }
    }
}
