﻿using static Wallace.UWP.Helpers.Tools.UWPStates;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Douban.UWP.Core.Models;
using Douban.UWP.NET.Resources;

namespace Douban.UWP.NET.Tools {
    public static class GlobalHelpers {

        #region Global methods

        public static LoginStatusBag GetLoginStatus(HtmlAgilityPack.HtmlDocument doc) {
            var ima = doc.DocumentNode
                    .SelectSingleNode("//div[@id='db-usr-profile']")
                    .SelectSingleNode("div[@class='pic']")
                    .SelectSingleNode("a")
                    .SelectSingleNode("img");
            return new LoginStatusBag { ImageUrl = new Uri(ima.Attributes["src"].Value), UserName = ima.Attributes["alt"].Value };
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
                AppResources.Current.Frame.SizeChanged += (sender, args) => { currentFramePage.Width = VisibleWidth; };
            } else {
                if (!isDivideScreen) {
                    currentFramePage.Width = VisibleWidth;
                    AppResources.Current.Frame.SizeChanged += (sender, args) => { currentFramePage.Width = VisibleWidth; };
                    return;
                }
                if (divideNum <= 0 || divideNum > 1)
                    divideNum = defaultDivide;
                var nowWidth = VisibleWidth;
                currentFramePage.Width = nowWidth > rangeNum ? divideNum * nowWidth : nowWidth;
                AppResources.Current.Frame.SizeChanged += (sender, args) => {
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
                currentPage.Margin = new Thickness(3, 0, 0, 0);
            else
                currentPage.Margin = new Thickness(0, 0, 0, 0);
        }

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