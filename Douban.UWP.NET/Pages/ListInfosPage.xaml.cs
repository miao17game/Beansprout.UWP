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
using Douban.UWP.NET.Tools;
using System.Threading.Tasks;
using Douban.UWP.NET.Controls;
using System.Diagnostics;
using Douban.UWP.Core.Models.ImageModels;
using System.Text.RegularExpressions;

namespace Douban.UWP.NET.Pages {

    public sealed partial class ListInfosPage : Page {
        public ListInfosPage() {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            if (IsFirstOpen)
                await Task.Delay(1000);
            if (IsFirstOpen) { IsFirstOpen = false; }
            ListViewResources.Source = new DoubanIncrementalContext<IndexItem>(FetchMoreResourcesAsync);
            SetFlipResourcesAsync();
        }

        public async void SetFlipResourcesAsync() {
            try {
                var result = await DoubanWebProcess.GetMDoubanResponseAsync(
                    path : "https://m.douban.com/rexxar/api/v2/promos?page=selection", 
                    host: "m.douban.com",
                    reffer: null);
                JObject jo = JObject.Parse(result);
                var promos = jo["promos"];
                if (promos.HasValues) {
                    var newList = new List<PromosItem>();
                    promos.Children().ToList().ForEach(singleton => {
                        var notif = singleton["notification"];
                        var uri = UriDecoder.GetUrlFromUri(singleton["uri"].Value<string>());
                        newList.Add(new PromosItem {
                            ImageSrc = new Uri(singleton["image"].Value<string>()),
                            Image = singleton["image"].Value<string>(),
                            NotificationCount = notif.HasValues ? notif["count"].Value<uint>() : 0,
                            NotificationVersion = notif.HasValues ? notif["version"].Value<string>() : null,
                            Text = singleton["text"].Value<string>(),
                            Uri = uri,
                            ID = singleton["id"].Value<string>(),
                        });
                    });
                    FlipResouces.Source = newList;
                    InitFlipTimer(newList);
                }
            } catch { /* Ignore */}
        }

        private void InitFlipTimer(List<PromosItem> list) {
            DispatcherTimer timer = new DispatcherTimer{ Interval = new TimeSpan(0, 0, 0, 7) };
            timer.Tick += (obj, args) => { if (MyFlip.SelectedIndex < list.Count - 1) MyFlip.SelectedIndex++; else MyFlip.SelectedIndex = 0; };
            timer.Start();
        }

        private void FlipInnerButton_Click(object sender, RoutedEventArgs e) {
            var path = (sender as Button).CommandParameter as string;
            var text = (sender as Button).Content as string;
            if (path == null)
                return;
            var succeed = Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out var newUri);
            if (!succeed)
                return;
            NavigateToBase?.Invoke(
                sender,
                new Core.Models.NavigateParameter { ToUri = newUri, Title = text },
                GetFrameInstance(Core.Models.FrameType.Content),
                GetPageType(Core.Models.NavigateType.ItemClick));
        }

        private async Task<IList<IndexItem>> FetchMessageFromAPIAsync(string target, int offset = 0) {
            IList<IndexItem> list = new List<IndexItem>();
            try {
                var result = await DoubanWebProcess.GetMDoubanResponseAsync(
                    path: target,
                    host: "m.douban.com",
                    reffer: "https://m.douban.com/");
                if (result == null) {
                    ReportHelper.ReportAttentionAsync(GetUIString("WebActionError"));
                    IncrementalLoadingBorder.SetVisibility(false);
                    return list;
                }
                JObject jo = JObject.Parse(result);
                var feeds = jo["recommend_feeds"];
                if (feeds == null || !feeds.HasValues) {
                    ReportHelper.ReportAttentionAsync(GetUIString("FetchJsonDataError"));
                    IncrementalLoadingBorder.SetVisibility(false);
                    return list;
                }
                if (feeds.HasValues) {
                    list.Add(new IndexItem {
                        Type = IndexItem.ItemType.DateBlock,
                        ThisDate = DateTime.Now.AddDays(-offset).ToString("yyyy-MM-dd") ,
                        MorePictures = new List<Uri> { new Uri(NoPictureUrl), new Uri(NoPictureUrl) }
                    });
                    feeds.Children().ToList().ForEach(singleton => {
                        try {
                            var targets = singleton["target"];
                            var card = singleton["card"];
                            if (targets != null) {
                                var author = targets["author"];
                                var morePic = targets["more_pic_urls"];
                                IList<Uri> more_pic = new List<Uri>();
                                if (morePic.HasValues) 
                                    morePic.ToList().ForEach(pic => { if (pic.Value<string>() != "") more_pic.Add(new Uri(pic.Value<string>())); });
                                else
                                    more_pic.Add(new Uri(NoPictureUrl)); more_pic.Add(new Uri(NoPictureUrl));
                                var type =
                                targets["cover_url"].Value<string>() == "" ? IndexItem.ItemType.Paragraph :
                                targets["photos_count"].Value<uint>() == 0 ? IndexItem.ItemType.Normal :
                                IndexItem.ItemType.Gallary;
                                list.Add(new IndexItem {
                                    Type = type,
                                    ReadCount = targets["read_count"].Value<uint>(),
                                    PhotosCount = targets["photos_count"].Value<uint>(),
                                    ImpressionUrl = targets["impression_url"].Value<string>(),
                                    Title = singleton["title"].Value<string>(),
                                    LikersCount = 0,
                                    AuthorAvatar = author != null && author.HasValues ? author["avatar"].Value<string>() != "" ? new Uri(author["avatar"].Value<string>()) : null : null,
                                    AuthorName = author != null && author.HasValues ? author["name"].Value<string>() : null,
                                    PathUrl = targets["uri"].Value<string>(),
                                    HasCover = targets["cover_url"].Value<string>() != "" ? true : false,
                                    Cover = targets["cover_url"].Value<string>() != "" ? new Uri(targets["cover_url"].Value<string>()) : null,
                                    Source = singleton["source"].Value<string>(),

                                    ColumnName = singleton["source_cn"].Value<string>() == "" ?
                                    (card != null && card.HasValues) ? 
                                    card["name"].Value<string>() : 
                                    GetUIString("AppName") : 
                                    singleton["source_cn"].Value<string>(),

                                    CommentCount = 0,
                                    OutSource = null,
                                    Comments = null,
                                    Action = null,
                                    Description = targets["desc"].Value<string>(),
                                    MorePictures = more_pic,
                                    ID = singleton["id"].Value<uint>(),
                                    MonitorUrl = targets["monitor_url"].HasValues ? targets["monitor_url"].Value<string>() : null,
                                });
                            }
                        } catch { /* Ignore, item error. */ }
                    });
                }
            } catch { ReportHelper.ReportAttentionAsync(GetUIString("UnknownError")); }
            IncrementalLoadingBorder.SetVisibility(false);
            return list;
        }

        private async Task<IList<IndexItem>> FetchMoreResourcesAsync(int offset) {
            IncrementalLoadingBorder.SetVisibility(true);
            var date = DateTime.Now.AddDays(1-offset).ToString("yyyy-MM-dd");
            var Host = "https://m.douban.com/rexxar/api/v2/recommend_feed?alt=json&next_date={0}&loc_id=&gender=&birthday=&udid=&for_mobile=true";
            return await FetchMessageFromAPIAsync(string.Format(Host, date), offset);
        }

        private void IndexList_ItemClick(object sender, ItemClickEventArgs e) {
            var item = e.ClickedItem as IndexItem;
            if (item == null || item.Type == IndexItem.ItemType.DateBlock)
                return;
            NavigateToBase?.Invoke(
                sender,
                new Core.Models.NavigateParameter {
                    ToUri = item.PathUrl != null ?
                    new Uri(UriDecoder.GetUrlFromUri(item.PathUrl)) : 
                    null ,
                    Title = item.Title,
                    IsNative = true},
                GetFrameInstance(Core.Models.FrameType.Content),
                GetPageType(Core.Models.NavigateType.ItemClickNative));
        }

        private void ScrollViewerChangedForFlip(object sender, ScrollViewerViewChangedEventArgs e) {
            try {
                if ((sender as ScrollViewer).VerticalOffset <= 240)
                    MyFlip.Margin = new Thickness(0, -(sender as ScrollViewer).VerticalOffset, 0, 0);
                if ((sender as ScrollViewer).VerticalOffset > 240)
                    MyFlip.Margin = new Thickness(0, -240, 0, 0);
            } catch { Debug.WriteLine("Save scroll positions error."); }
        }

        private void IndexList_Loaded(object sender, RoutedEventArgs e) {
            scroll = GlobalHelpers.GetScrollViewer(IndexList);
            scroll.ViewChanged += ScrollViewerChangedForFlip;
        }

        #region Properties
        private ScrollViewer scroll;
        private string NoPictureUrl = "https://www.none-wallace-767fc6vh7653df0jb.com/no_wallace_085sgdfg7447fddds65.jpg";
        #endregion

    }
}
