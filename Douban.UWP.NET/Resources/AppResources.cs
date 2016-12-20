using static Wallace.UWP.Helpers.Tools.UWPStates;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Douban.UWP.Core.Models;

namespace Douban.UWP.NET.Resources {
    /// <summary>
    /// Resources Helper 
    /// </summary>
    public static class AppResources {

        #region Hamburger resources
        public static List<NavigationBar> HamburgerResList { get { return navigationListMap; } }
        static private List<NavigationBar> navigationListMap = new List<NavigationBar> {
                new NavigationBar {
                    Title = GetUIString("DB_INDEX"),
                    PathUri = new Uri("https://www.douban.com/"),
                    NaviType = NavigateType.Index,
                    FetchType = DataFetchType.Index,
                },
                new NavigationBar {
                    Title = GetUIString("DB_BOOK"),
                    PathUri = new Uri("https://book.douban.com/"),
                    NaviType = NavigateType.Index,
                },
                new NavigationBar {
                    Title = GetUIString("DB_MOVIE"),
                    PathUri = new Uri("https://movie.douban.com/"),
                    NaviType = NavigateType.Webview
                },
                new NavigationBar {
                    Title = GetUIString("DB_MUSIC"),
                    PathUri = new Uri("https://music.douban.com/"),
                    NaviType = NavigateType.Webview
                },
                new NavigationBar {
                    Title = GetUIString("DB_LOCATION"),
                    PathUri = new Uri("https://www.douban.com/location/"),
                    NaviType = NavigateType.Webview
                },
                new NavigationBar {
                    Title = GetUIString("DB_GROUP"),
                    PathUri = new Uri("https://www.douban.com/group/"),
                    NaviType = NavigateType.Webview
                },
                new NavigationBar {
                    Title = GetUIString("DB_READ"),
                    PathUri = new Uri("https://read.douban.com/?dcs=top-nav&amp;dcm=douban"),
                    NaviType = NavigateType.Webview
                },
                new NavigationBar {
                    Title = GetUIString("DB_FM"),
                    PathUri = new Uri("https://douban.fm/?from_=shire_top_nav"),
                    NaviType = NavigateType.Webview
                },
                new NavigationBar {
                    Title = GetUIString("DB_DONGXI"),
                    PathUri = new Uri("https://dongxi.douban.com/?dcs=top-nav&amp;dcm=douban"),
                    NaviType = NavigateType.Webview
                },
                new NavigationBar {
                    Title = GetUIString("DB_MARKET"),
                    PathUri = new Uri("https://market.douban.com/?utm_campaign=douban_top_nav&amp;utm_source=douban&amp;utm_medium=pc_web"),
                    NaviType = NavigateType.Webview
                },
                new NavigationBar {
                    Title = GetUIString("DB_MORE"),
                    PathUri = new Uri("https://www.douban.com/"),
                    NaviType = NavigateType.Webview
                },
            };
        #endregion
    }
}
