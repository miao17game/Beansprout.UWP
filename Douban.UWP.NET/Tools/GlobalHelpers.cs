using static Wallace.UWP.Helpers.Tools.UWPStates;
using static Douban.UWP.NET.Resources.AppResources;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Douban.UWP.Core.Models;
using Douban.UWP.NET.Resources;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Newtonsoft.Json.Linq;

namespace Douban.UWP.NET.Tools {
    public static class GlobalHelpers {

        #region Global methods

        public static LoginStatusBag GetLoginStatus(HtmlAgilityPack.HtmlDocument doc) {
            var ima = doc.DocumentNode
                    .SelectSingleNode("//div[@id='db-usr-profile']")
                    .SelectSingleNode("div[@class='pic']")
                    .SelectSingleNode("a")
                    .SelectSingleNode("img");
            var basic_info_div = doc.DocumentNode.SelectSingleNode("//div[@class='basic-info']");
            var bigHead = basic_info_div != null ? basic_info_div.SelectSingleNode("img[@class='userface']").Attributes["src"].Value : null;
            var user_info_div = doc.DocumentNode.SelectSingleNode("//div[@class='user-info']");
            var location = user_info_div != null ? user_info_div.SelectSingleNode("a").Attributes["href"].Value : null;
            var location_string = user_info_div != null ? user_info_div.SelectSingleNode("a").InnerText : null;
            var des_span = doc.DocumentNode.SelectSingleNode("//span[@id='intro_display']");
            return new LoginStatusBag {
                ImageUrl = ima.Attributes["src"].Value,
                UserName = ima.Attributes["alt"].Value,
                Description = des_span != null ? des_span.InnerText : GetUIString("Lazy_for_no_description"),
                BigHeadUrl = bigHead,
                LocationString = location_string,
                LocationUrl = location,
            };
        }

        public static LoginStatusBag GetLoginStatus(string webResult) {
            JObject jo = JObject.Parse(webResult);
            var loc = jo["loc"];
            var banners = jo["profile_banner"];
            return new LoginStatusBag {
                UserName = jo["name"].Value<string>(),
                BigHeadUrl = jo["large_avatar"].Value<string>(),
                Description = jo["intro"].Value<string>(),
                ImageUrl = jo["avatar"].Value<string>(),
                LocationString = loc.HasValues ? loc["name"].Value<string>() : null,
                LocationUrl = null,
                APIUserinfos = new APIUserinfos {
                    //AbstractName = jo["abstract"].Value<string>(),
                    ArkPublishedCount = jo["ark_published_count"].Value<uint>(),
                    Avatar = jo["avatar"].Value<string>(),
                    Birthday = jo["birthday"].Value<string>(),
                    CanDonate = jo["can_donate"].Value<bool>(),
                    CanSetOriginal = jo["can_set_original"].Value<bool>(),
                    CollectedSubjectsCount = jo["collected_subjects_count"].Value<uint>(),
                    DramasCount = jo["dramas_count"].Value<uint>(),
                    Followed = jo["followed"].Value<bool>(),
                    FollowersCount = jo["followers_count"].Value<uint>(),
                    FollowingCount = jo["following_count"].Value<uint>(),
                    FollowingDouListCount = jo["following_doulist_count"].Value<uint>(),
                    Gender = jo["gender"].Value<string>(),
                    GroupChatCount = jo["group_chat_count"].Value<uint>(),
                    HasUserHotModule = jo["has_user_hot_module"].Value<bool>(),
                    ID = jo["id"].Value<string>(),
                    InBlackList = jo["in_blacklist"].Value<bool>(),
                    Introductions = jo["intro"].Value<string>(),
                    IsNormal = jo["is_normal"].Value<bool>(),
                    JoinedGroupCount = jo["joined_group_count"].Value<uint>(),
                    Kind = jo["kind"].Value<string>(),
                    LargeAvatar = jo["large_avatar"].Value<string>(),
                    LocationID = loc.HasValues? loc["id"].Value<string>():null,
                    LocationName = loc.HasValues? loc["name"].Value<string>():null,
                    LocationUid = loc.HasValues? loc["uid"].Value<string>():null,
                    NotesCount = jo["notes_count"].Value<uint>(),
                    OwnedDouListCount = jo["owned_doulist_count"].Value<uint>(),
                    PhotoAlbumsCount = jo["photo_albums_count"].Value<uint>(),
                    ProfileBannerLarge = banners.HasValues? banners["large"].Value<string>():null,
                    ProfileBannerNormal = banners.HasValues ? banners["normal"].Value<string>() : null,
                    RegisterTime = jo["reg_time"].Value<string>(),
                    Remark = jo["remark"].Value<string>(),
                    SetiChannelCount = jo["seti_channel_count"].Value<uint>(),
                    StatusesCount = jo["statuses_count"].Value<uint>(),
                    Type = jo["type"].Value<string>(),
                    UpdatedProfile = jo["updated_profile"].Value<bool>(),
                    Uri = jo["uri"].Value<string>(),
                    Url = jo["url"].Value<string>(),
                    UserHotModuleEnabled = jo["user_hot_module_enabled"].Value<bool>(),
                    UserUid = jo["uid"].Value<string>(),
                    VerifyReason = jo["verify_reason"].Value<string>(),
                    VerifyType = jo["verify_type"].Value<string>(),
                }
            };
        }

        public static void ResetLoginStatus() {
            Current.ResetUserStatus();
        }

        /// <summary>
        /// Change the page layout by the settings item : "Divide Screen Mode"
        /// </summary>
        /// <param name="currentFramePage">current child page instance</param>
        /// <param name="rangeNum">default number of the range to divide, is 800</param>
        /// <param name="divideNum">the percent value of divide</param>
        /// <param name="defaultDivide">defalut percent value, is 0.6</param>
        /// <param name="isDivideScreen">make sure if need to divide screen</param>
        public static void DivideWindowRange(
            Page currentFramePage,
            double divideNum,
            double rangeNum = 800,
            double defaultDivide = 0.6,
            bool isDivideScreen = true) {

            SetChildPageMargin(currentPage: currentFramePage, matchNumber: VisibleWidth, isDivideScreen: isDivideScreen);

            if (IsMobile) {
                currentFramePage.Width = VisibleWidth;
                Current.Frame.SizeChanged += (sender, args) => { currentFramePage.Width = VisibleWidth; };
            } else {
                if (!isDivideScreen) {
                    currentFramePage.Width = VisibleWidth;
                    Current.Frame.SizeChanged += (sender, args) => { currentFramePage.Width = VisibleWidth; };
                    return;
                }
                if (divideNum <= 0 || divideNum > 1)
                    divideNum = defaultDivide;
                var nowWidth = VisibleWidth;
                currentFramePage.Width = nowWidth > rangeNum ? divideNum * nowWidth : nowWidth;
                Current.Frame.SizeChanged += (sender, args) => {
                    var nowWidthEx = VisibleWidth;
                    currentFramePage.Width = nowWidthEx > rangeNum ? divideNum * nowWidthEx : nowWidthEx;
                };
            }
        }

        /// <summary>
        /// Make the page more adaptive to the settings item : "Divide Screen Mode"
        /// </summary>
        /// <param name="currentPage">current child page instance</param>
        /// <param name="matchNumber">baseGrid's width of current page </param>
        /// <param name="rangeNumber">default number of the range to divide, is 800</param>
        /// <param name="isDivideScreen">make sure if need to divide screen</param>
        public static void SetChildPageMargin(
            Page currentPage,
            double matchNumber,
            bool isDivideScreen,
            double rangeNumber = 800) {
            if (matchNumber > rangeNumber && !IsMobile && isDivideScreen)
                currentPage.Margin = new Thickness(1, 0, 0, 0);
            else
                currentPage.Margin = new Thickness(0, 0, 0, 0);
        }

        public static SolidColorBrush GetColorRandom(int num) {
            return new SolidColorBrush() {
                Color =
                num == 0 ? Color.FromArgb(255, 244, 78, 97) :
                num == 1 ? Color.FromArgb(255, 255, 193, 63) :
                num == 2 ? Color.FromArgb(255, 49, 199, 155) :
                num == 3 ? Color.FromArgb(255, 255, 63, 138) :
                num == 4 ? Color.FromArgb(255, 255, 120, 63) :
                num == 5 ? Color.FromArgb(255, 255, 67, 63) :
                num == 6 ? Color.FromArgb(255, 222, 135, 119) :
                num == 7 ? Color.FromArgb(255, 53, 132, 154) :
                num == 8 ? Color.FromArgb(255, 75, 21, 173) :
                num == 9 ? Color.FromArgb(255, 217, 6, 94) :
                num == 10 ? Color.FromArgb(255, 60, 188, 98) :
                num == 11 ? Color.FromArgb(255, 97, 17, 171) :
                num == 12 ? Color.FromArgb(255, 254, 183, 8) :
                num == 13 ? Color.FromArgb(255, 69, 90, 172) :
                num == 14 ? Color.FromArgb(255, 141, 4, 33) :
                Color.FromArgb(255, 82, 82, 82)
            };
        } 

            #region Handler of ListView Scroll 

            public static ScrollViewer GetScrollViewer(DependencyObject depObj) {
            if (depObj is ScrollViewer)
                return depObj as ScrollViewer;
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++) {
                var child = VisualTreeHelper.GetChild(depObj, i);
                var result = GetScrollViewer(child);
                if (result != null)
                    return result;
            }
            return null;
        }

        public static PivotItem GetPVItemViewer(DependencyObject depObj, ref int num) {
            if (depObj is PivotItem) {
                if (num == 0)
                    return depObj as PivotItem;
                else
                    num--;
                return null;
            }

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++) {
                var child = VisualTreeHelper.GetChild(depObj, i);
                var result = GetPVItemViewer(child, ref num);
                if (result != null)
                    return result;
            }
            return null;
        }

        #endregion

        #region Dropped
        //public static bool IsNeedLoginOrNot { get { return !LoginCache.IsInsert || IsMoreThan30Minutes(LoginCache.CacheMiliTime, DateTime.Now); } }

        ///// <summary>
        ///// well....you can know what i am doing by the name of the method......
        ///// </summary>
        ///// <param name="oldTime"></param>
        ///// <param name="newTime"></param>
        ///// <returns></returns>
        //private static bool IsMoreThan30Minutes(DateTime oldTime, DateTime newTime) {
        //    return
        //        newTime.Subtract(new DateTime(1970, 1, 1, 8, 0, 0)).TotalSeconds -
        //        oldTime.Subtract(new DateTime(1970, 1, 1, 8, 0, 0)).TotalSeconds >=
        //        1800;
        //}

        /// <summary>
        /// If you have not login, this method will redirect you to re-login popup to finish login-action.
        /// </summary>
        /// <param name = "fromUri" >return-to</param>
        /// <param name = "fromFetchType" >return-dataType</param>
        /// <param name = "returnMessage" >return-messageBag</param>
        /// <param name = "fromNaviType" >return-navigateType</param>
        //public static void ReLoginIfStatusIsInvalid(
        //    Uri fromUri,
        //    DataFetchType fromFetchType = DataFetchType.NULL,
        //    object returnMessage = null,
        //    NavigateType fromNaviType = NavigateType.Webview) {

        //    if (!IsNeedLoginOrNot)
        //        return;
        //    Current.ReLoginPopup.IsOpen = true;
        //    Current.NavigateToBase?.Invoke(
        //        null,
        //        new NavigateParameter {
        //            ToFetchType = DataFetchType.Index_ReLogin,
        //            ToUri = new Uri(LoginPath),
        //            MessageBag = GetUIString("LNU_Index_LS"),
        //            NaviType = NavigateType.ReLogin,
        //            MessageToReturn = new ReturnParameter {
        //                FromUri = fromUri,
        //                FromFetchType = fromFetchType,
        //                ReturnMessage = returnMessage,
        //                FromNaviType = fromNaviType,
        //            },
        //        },
        //        GetFrameInstance(NavigateType.ReLogin),
        //        GetPageType(NavigateType.ReLogin));
        //}

        ///// <summary>
        ///// Change app title route string.
        ///// </summary>
        ///// <param name="value">the new value to be written into App title.</param>
        //public static void ChangeTitlePath(string value) {
        //    Current.NavigateTitlePath.Text = value;
        //}

        ///// <summary>
        ///// Change app title route string.
        ///// </summary>
        ///// <param name="number">the route point need to be changed</param>
        ///// <param name="value">value to be written into the target point</param>
        //public static void ChangeTitlePath(uint number, string value) {
        //    if (number < 1 || number > 3)
        //        return;
        //    switch (number) {
        //        case 1: NaviPathTitle.Route01 = value; break;
        //        case 2: NaviPathTitle.Route02 = value; break;
        //        case 3: NaviPathTitle.Route03 = value; break;
        //    }
        //    Current.NavigateTitlePath.Text = NaviPathTitle.RoutePath;
        //}
        #endregion

        #endregion

    }
}
