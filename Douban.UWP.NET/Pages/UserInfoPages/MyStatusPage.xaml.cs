using static Wallace.UWP.Helpers.Tools.UWPStates;
using static Douban.UWP.NET.Resources.AppResources;

using Douban.UWP.Core.Models;
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
using Wallace.UWP.Helpers;
using Douban.UWP.NET.Tools;
using Douban.UWP.Core.Tools;
using Newtonsoft.Json.Linq;
using Douban.UWP.Core.Models.LifeStreamModels;
using System.Threading.Tasks;

namespace Douban.UWP.NET.Pages {

    public sealed partial class MyStatusPage : BaseContentPage {
        public MyStatusPage() {
            this.InitializeComponent();
        }

        protected override void InitPageState() {
            base.InitPageState();
            GlobalHelpers.SetChildPageMargin(this, matchNumber: VisibleWidth, isDivideScreen: IsDivideScreen);
            GlobalHelpers.DivideWindowRange(this, DivideNumber, isDivideScreen: IsDivideScreen);
        }

        #region Events

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            if (LoginStatus != null && LoginStatus.APIUserinfos != null) {
                uid = LoginStatus.APIUserinfos.UserUid;
                ListResources.Source = new DoubanIncrementalContext<StatusItem>(FetchMoreResourcesAsync);
            }
        }

        private void BaseHamburgerButton_Click(object sender, RoutedEventArgs e) {
            PageSlideOutStart(VisibleWidth > 800 ? false : true);
        }

        private void ContentList_ItemClick(object sender, ItemClickEventArgs e) {
            var iten = e.ClickedItem as StatusItem;
            if (iten == null)
                return;
            Uri.TryCreate(string.Format(NavigateUrlFormat, uid, iten.ID), UriKind.RelativeOrAbsolute, out var uri);
            NavigateToBase?.Invoke(
                null,
                new NavigateParameter { Title = iten.Activity, ToUri = uri, IsFromInfoClick = true },
                UserInfoDetails,
                GetPageType(NavigateType.InfoItemClick));
            UserInfoPopup.IsOpen = false;
        }

        #endregion

        #region Methods

        private async Task<IList<StatusItem>> FetchMessageFromAPIAsync(string target) {
            IList<StatusItem> list = new List<StatusItem>();
            try {
                var result = await DoubanWebProcess.GetMDoubanResponseAsync(target);
                if (result == null) {
                    ReportHelper.ReportAttention(GetUIString("WebActionError"));
                    DoubanLoading.SetVisibility(false);
                    IncrementalLoadingBorder.SetVisibility(false);
                    return list;
                }
                JObject jo = JObject.Parse(result);
                var feeds = jo["items"];
                if (feeds == null ) {
                    ReportHelper.ReportAttention(GetUIString("FetchJsonDataError"));
                    DoubanLoading.SetVisibility(false);
                    IncrementalLoadingBorder.SetVisibility(false);
                    return list;
                }
                if (feeds.HasValues) {
                    feeds.Children().ToList().ForEach(singleton => {
                        try {
                            var status = singleton["status"];
                            if (status.HasValues) {
                                var item = CreateDefaultStatusItem(status);
                                var author = status["author"];
                                var images = status["images"];
                                var entities = status["entities"];
                                var card = status["card"];
                                if (author.HasValues)
                                    item.Author = InitAuthorStatus(author, author["loc"]);
                                item.Images = new List<PictureItemBase>();
                                if (images.HasValues) {
                                    item.HasImages = true;
                                    images.Children().ToList().ForEach(each => item.Images.Add(CreatePictureBaseItem(each)));
                                } else
                                    item.Images.Add(CreateNoPictureBase());
                                if (card.HasValues) {
                                    item.HasCard = true;
                                    item.Card = CreateNewCardInstance(card);
                                    if (card["rating"] != null) {
                                        item.Card.HasRating = true;
                                        item.Card.Rating = card["rating"].Value<double>();
                                    }
                                }
                                item.Comments = null;
                                item.Entities = null;
                                item.ParentStatus = null;
                                list.Add(item);
                            }
                        } catch { /* Ignore, item error. */ }
                    });
                }
            } catch { ReportHelper.ReportAttention(GetUIString("UnknownError")); }
            IncrementalLoadingBorder.SetVisibility(false);
            max_id = list.Count != 0 ? (list[list.Count-1].ID - 1).ToString() : "SHOULD_STOP";
            if(list.Count == 0)
                ReportHelper.ReportAttention(GetUIString("SHOULD_STOP"));
            return list;
        }

        private StatusCard CreateNewCardInstance(JToken card) {
            return new StatusCard {
                HasRating = false,
                Url = card["url"].Value<string>(),
                Uri = card["uri"].Value<string>(),
                SubTitle = card["subtitle"].Value<string>(),
                Title = card["title"].Value<string>(),
                HasImage = card["image"].HasValues ? true : false,
                Image = card["image"].HasValues ? CreatePictureBaseItem(card["image"]) : CreateNoPictureBase(),
            };
        }

        private StatusItem CreateDefaultStatusItem(JToken status) {
            return new StatusItem() {
                Type = InfosItemBase.JsonType.Status,
                HasImages = false,
                HasCard = false,
                HasComment = false,
                HasEntity = false,
                HasText = status["text"].Value<string>() == "" ? false : true,
                Text = status["text"].Value<string>(),
                ResharesCounts = status["reshares_count"].Value<string>(),
                ResharersCounts = status["resharers_count"].Value<string>(),
                SharingUrl = status["sharing_url"].Value<string>(),
                Liked = status["liked"].Value<bool>(),
                Uri = status["uri"].Value<string>(),
                LikersCounts = status["like_count"].Value<string>(),
                Time = status["create_time"].Value<string>(),
                ReshareID = status["reshare_id"].Value<string>(),
                Activity = status["activity"].Value<string>() != "" ? status["activity"].Value<string>(): GetUIString("add_a_status"),
                CommentsCounts = status["comments_count"].Value<string>(),
                ResharedStatus = status["reshared_status"].Value<string>(),
                ID = status["id"].Value<int>()
            };
        }

        private PictureItemBase CreateNoPictureBase() {
            return new PictureItemBase {
                Normal = new Uri(NoPictureUrl),
                Large = new Uri(NoPictureUrl),
            };
        }

        private PictureItemBase CreatePictureBaseItem(JToken each) {
            var normal = each["normal"];
            var large = each["large"];
            return new PictureItemBase {
                Normal = new Uri((normal != null && normal.HasValues) ? normal["url"].Value<string>() : NoPictureUrl),
                Large = new Uri((large != null && large.HasValues) ? large["url"].Value<string>() : NoPictureUrl),
            };
        }

        private AuthorStatus InitAuthorStatus(JToken author, JToken location) {
            return new AuthorStatus {
                Kind = author["kind"].Value<string>(),
                Name = author["name"].Value<string>(),
                Url = author["url"].Value<string>(),
                Gender = author["gender"].Value<string>(),
                Abstract = author["abstract"].Value<string>(),
                Uri = author["uri"].Value<string>(),
                Avatar = author["avatar"].Value<string>(),
                LargeAvatar = author["large_avatar"].Value<string>(),
                Type = author["type"].Value<string>(),
                ID = author["id"].Value<string>(),
                Uid = author["uid"].Value<string>(),
                LocationID = location.HasValues ? location["id"].Value<string>() : null,
                LocationName = location.HasValues ? location["name"].Value<string>() : null,
                LocationUid = location.HasValues ? location["uid"].Value<string>() : null,
            };
        }

        private async Task<IList<StatusItem>> FetchMoreResourcesAsync(int offset) {
            if (max_id == "SHOULD_STOP")
                return new List<StatusItem>();
            IncrementalLoadingBorder.SetVisibility(true);
            return await FetchMessageFromAPIAsync(string.Format(Host, uid, max_id, 10));
        }

        #endregion

        #region Properties and state

        string uid;
        string max_id = "0";
        const string Host = "https://m.douban.com/rexxar/api/v2/status/user_timeline/{0}?max_id={1}&count={2}&for_mobile=1";
        const string NoPictureUrl = "https://www.none-wallace-767fc6vh7653df0jb.com/no_wallace_085sgdfg7447fddds65.jpg";
        const string NavigateUrlFormat = "https://m.douban.com/people/{0}/status/{1}/";

        #endregion

    }
}
