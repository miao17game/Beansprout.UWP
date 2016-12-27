using static Wallace.UWP.Helpers.Tools.UWPStates;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Douban.UWP.Core.Models;
using Douban.UWP.NET.Pages;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Wallace.UWP.Helpers;
using Douban.UWP.Core.Tools;
using Windows.UI.Xaml;

namespace Douban.UWP.NET.Resources {
    /// <summary>
    /// Resources Helper 
    /// </summary>
    public static class AppResources {

        #region Controls Management
        public static TextBlock NavigateTitleBlock { get; set; }
        public static MainPage Current { get; set; }
        public static Frame MainContentFrame { get; set; }
        public static Frame MainLeftPartFrame { get; set; }
        public static Frame MainLoginFrame { get; set; }
        public static ProgressRing BaseListRing { get; set; }
        public static ListBox HamburgerBox { get; set; }
        public static Popup MainLoginPopup { get; set; }
        public static Image DoubanLoading { get; set; }
        #endregion

        #region Global Resources Properties

        public static bool IsGlobalDark { get { return (bool?)SettingsHelper.ReadSettingsValue(SettingsConstants.IsDarkThemeOrNot) ?? true; } }
        public static bool IsDivideScreen { get { return (bool?)SettingsHelper.ReadSettingsValue(SettingsSelect.IsDivideScreen) ?? true; }}
        public static double DivideNumber { get { return (double?)SettingsHelper.ReadSettingsValue(SettingsSelect.SplitViewMode) ?? 0.6; }}

        private static LoginStatusBag _loginStatus = new LoginStatusBag();
        public static LoginStatusBag LoginStatus {
            get { return _loginStatus; }
            set { _loginStatus = value; }
        }

        private static bool _isFirstOpen = true;
        public static bool IsFirstOpen {
            get { return _isFirstOpen; }
            set { _isFirstOpen = value; }
        }

        public static bool IsLogined { get; set; }
        public static string LoginResult { get; set; }

        #endregion

        #region Navigate Methods
        public delegate void NavigationEventHandler(object sender, NavigateParameter parameter, Frame frame, Type type);
        public static NavigationEventHandler NavigateToBase = (sender, parameter, frame, type) => { frame.Navigate(type, parameter); };
        #endregion

        #region Type

        public static Type GetPageType(NavigateType type) { return PageTypeCollection.ContainsKey(type) ? PageTypeCollection[type] : null; }
        static private IDictionary<NavigateType, Type> pagesMaps;
        public static IDictionary<NavigateType, Type> PageTypeCollection {
            get {
                return pagesMaps ?? new Func<IDictionary<NavigateType, Type>>(()=> {
                pagesMaps = new Dictionary<NavigateType, Type> {
                    { NavigateType.Settings,typeof(SettingsPage)},
                    { NavigateType.Login,typeof(LoginPage)},
                    { NavigateType.UserInfo,typeof(UserInfoPage)},
                    { NavigateType.Index, typeof(ListInfosPage)},
                    { NavigateType.ItemClick, typeof(WebContentPage)},
                    { NavigateType.Webview, typeof(WebViewPage)},
                };
                return pagesMaps;
            }).Invoke(); }
        }

        #endregion

        #region Frame

        public static Frame GetFrameInstance(NavigateType type) { return FrameMaps.ContainsKey(type) ? FrameMaps[type] : null; }
        static private IDictionary<NavigateType, Frame> frameMaps;
        public static IDictionary<NavigateType, Frame> FrameMaps {
            get {
                return frameMaps ?? new Func<IDictionary<NavigateType, Frame>>(()=> {
                    frameMaps = new Dictionary<NavigateType, Frame> {
                        { NavigateType.Settings, MainLeftPartFrame},
                        { NavigateType.Login, MainLoginFrame},
                        { NavigateType.UserInfo, MainContentFrame},
                        { NavigateType.Index, MainLeftPartFrame},
                        { NavigateType.ItemClick, MainContentFrame},
                        { NavigateType.Webview, MainLeftPartFrame},
                    };
                    return frameMaps;
                }).Invoke();
            }
        }

        #region Hamburger resources
        static private IList<NavigationBar> navigationListMap;
        public static IList<NavigationBar> HamburgerResList {
            get {
                return navigationListMap ?? new Func<IList<NavigationBar>>(() => {
                    navigationListMap = new List<NavigationBar> {
                        new NavigationBar {
                            Title = GetUIString("DB_INDEX"),
                            PathUri = new Uri("https://m.douban.com/"),
                            NaviType = NavigateType.Index,
                        },
                        new NavigationBar {
                            Title = GetUIString("DB_BOOK"),
                            PathUri = new Uri("https://book.douban.com/"),
                            NaviType = NavigateType.Webview,
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
                    return navigationListMap;
                }).Invoke();
            }
        }
        #endregion

        #endregion

        //#region Child page for cache

        //public static void AddBaseListPageInstance(string key, BaseListPage instance) { if (!baseListPageMap.ContainsKey(key)) { baseListPageMap.Add(key, instance); } }
        //public static BaseListPage GetPageInstance(string key) { return baseListPageMap.ContainsKey(key) ? baseListPageMap[key] : null; }
        //public static bool IfContainsPageInstance(string key) { return baseListPageMap.ContainsKey(key); }
        //static private Dictionary<string, BaseListPage> baseListPageMap = new Dictionary<string, BaseListPage> {
        //};

        //#endregion
    }
}
