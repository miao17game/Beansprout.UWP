﻿using static Wallace.UWP.Helpers.Tools.UWPStates;

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
using Douban.UWP.NET.Pages.TypeWebPage;
using Douban.UWP.NET.Pages.SingletonPages.FMPages;
using Douban.UWP.NET.Tools;
using Douban.UWP.Core.Models.FMModels;
using Douban.UWP.NET.Pages.SubjectCollectionPages;
using Douban.UWP.NET.Pages.SubjectCollectionPages.MoviePages;

namespace Douban.UWP.NET.Resources {
    /// <summary>
    /// Resources Helper 
    /// </summary>
    public static class AppResources {

        #region Controls Management
        public static TextBlock NavigateTitleBlock { get; set; }
        public static MainPage Current { get; set; }
        public static Frame MainMetroFrame { get; set; }
        public static Frame MainUpContentFrame { get; set; }
        public static Frame MainUserInfosFrame { get; set; }
        public static Frame MainContentFrame { get; set; }
        public static Frame MainLeftPartFrame { get; set; }
        public static Frame MainLoginFrame { get; set; }
        public static Frame UserInfoDetails { get; set; }
        public static ProgressRing BaseListRing { get; set; }
        public static ListBox HamburgerBox { get; set; }
        public static Popup MainLoginPopup { get; set; }
        public static Popup UserInfoPopup { get; set; }
        public static Image DoubanLoading { get; set; }
        #endregion

        #region Global Resources Properties

        private static bool? isGlobalDark;
        public static bool IsGlobalDark {
            get {
                return isGlobalDark ?? new Func<bool>(() => {
                    isGlobalDark = (bool?)SettingsHelper.ReadSettingsValue(SettingsConstants.IsDarkThemeOrNot) ?? true;
                    return isGlobalDark.Value;
                }).Invoke();
            }
            set { isGlobalDark = value; }
        }

        private static bool? isDivideScreen;
        public static bool IsDivideScreen {
            get {
                return isDivideScreen ?? new Func<bool>(()=> {
                    isDivideScreen = (bool?)SettingsHelper.ReadSettingsValue(SettingsSelect.IsDivideScreen) ?? true;
                    return isDivideScreen.Value;
                }).Invoke();
            }
            set { isDivideScreen = value; }
        }

        private static bool? isProjectNEON;
        public static bool IsProjectNEON {
            get {
                return isProjectNEON ?? new Func<bool>(() => {
                    isProjectNEON = (bool?)SettingsHelper.ReadSettingsValue(SettingsSelect.IsProjectNEON) ?? true;
                    return isProjectNEON.Value;
                }).Invoke();
            }
            set { isProjectNEON = value; }
        }

        private static double? divideNumber;
        public static double DivideNumber {
            get {
                return divideNumber ?? new Func<double>(() => {
                    divideNumber = (double?)SettingsHelper.ReadSettingsValue(SettingsSelect.SplitViewMode) ?? 0.6;
                    return divideNumber.Value;
                }).Invoke();
            }
            set { divideNumber = value; }
        }

        private static string _userID;
        public static string UserID {
            get { return _userID ?? (_userID = SettingsHelper.ReadSettingsValue(SettingsSelect.UserID) as string); }
            set { _userID = value; }
        }

        private static string _accessToken;
        public static string AccessToken {
            get { return _accessToken ?? (_accessToken = SettingsHelper.ReadSettingsValue(SettingsSelect.AccessToken) as string); }
            set { _accessToken = value; }
        }

        private static string _refreshToken;
        public static string RefreshToken {
            get { return _refreshToken ?? (_refreshToken = SettingsHelper.ReadSettingsValue(SettingsSelect.RefreshToken) as string); }
            set { _refreshToken = value; }
        }

        private static string metroList;
        public static void SetMetroList(string value) { metroList = value; }
        public async static Task<string> GetMetroListAsync() {
            return metroList ?? await new Func<Task<string>>(async () => {
                metroList = (await CacheHelpers.ReadSpecificCacheValueAsync(CacheSelect.MetroList)) ?? "";
                return metroList;
            }).Invoke();
        }

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

        private static bool _has_fm_extensions;
        public static bool HasFMExtensions {
            get { return _has_fm_extensions; }
            set { _has_fm_extensions = value; }
        }

        private static double? _volumn;
        public static double Volumn {
            get {
                return _volumn ?? new Func<double>(() => {
                    _volumn = (double?)SettingsHelper.ReadSettingsValue(SettingsSelect.MusicServiceVolumn) ?? 1.0;
                    return _volumn.Value;
                }).Invoke();
            }
            set { _volumn = value; }
        }

        private const string default_toast_voice = @"ms-appx:///Voice/yiner.mp3";
        public static string DefaultToastVoice { get { return default_toast_voice; } }

        private static string _toast_voice;
        public static string ToastVoice {
            get { return _toast_voice ?? (_toast_voice = (SettingsHelper.ReadSettingsValue(SettingsSelect.ToastVoice) as string) ?? default_toast_voice); }
            set { _toast_voice = value; }
        }

        private static bool? _is_toast_enable;
        public static bool IsToastEnable {
            get {
                return _is_toast_enable ?? new Func<bool>(() => {
                    _is_toast_enable = (bool?)SettingsHelper.ReadSettingsValue(SettingsSelect.IsToastEnable) ?? true;
                    return _is_toast_enable.Value;
                }).Invoke();
            }
            set { _is_toast_enable = value; }
        }

        const string api_key = "02f7751a55066bcb08e65f4eff134361";
        public static string APIKey { get { return api_key; } }

        public const double FormatNumber = 800;
        public static bool IsLogined { get; set; }
        public static string LoginResult { get; set; }

        private static DoubanMusicService _service;
        public static DoubanMusicService Service { get { return _service ?? (_service = new DoubanMusicService()); } }

        private static DoubanDownloadService _downloader;
        public static DoubanDownloadService Downloader { get { return _downloader ?? (_downloader = new DoubanDownloadService()); } }

        public static MusicBoardParameter MusicIsCurrent { get; set; }

        #endregion

        #region Navigate Methods
        public delegate void NavigationEventHandler(object sender, NavigateParameterBase parameter, Frame frame, Type type);
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
                    { NavigateType.Search,typeof(SearchWebPage)},
                    { NavigateType.Login,typeof(LoginPage)},
                    { NavigateType.UserInfo,typeof(UserInfoPage)},
                    { NavigateType.InfoItemClick,typeof(WebContentPage)},
                    { NavigateType.Index, typeof(ListInfosPage)},
                    { NavigateType.Movie, typeof(MovieIndexPage)},
                    { NavigateType.TV, typeof(TVIndexPage)},
                    { NavigateType.Book, typeof(BookIndexPage)},
                    { NavigateType.Music, typeof(MusicIndexPage)},
                    { NavigateType.FM, typeof(FMWebViewPage)},
                    { NavigateType.FM_Extensions, typeof(FMPage)},
                    { NavigateType.FM_MHzSongList, typeof(FM_MHzSongListPage)},
                    { NavigateType.ItemClick, typeof(WebContentPage)},
                    { NavigateType.ItemClickNative, typeof(CardWebPage)},
                    { NavigateType.Webview, typeof(WebViewPage)},
                    { NavigateType.DouList, typeof(WebContentPage)},
                    { NavigateType.MovieContent, typeof(MovieContentPage)},
                    { NavigateType.MovieContentQuestion, typeof(WebContentPage)},
                    { NavigateType.MovieFilter, typeof(MovieCollectionPage)},
                    { NavigateType.BookContent, typeof(WebContentPage)},
                    { NavigateType.BookFilter, typeof(BookCollectionPage)},
                    { NavigateType.MusicContent, typeof(WebContentPage)},
                    { NavigateType.MusicFilter, typeof(MusicCollectionPage)},
                    { NavigateType.TVContent, typeof(WebContentPage)},
                    { NavigateType.TVFilter, typeof(TVCollectionPage)},
                    { NavigateType.Undefined, typeof(WebContentPage)},
                    { NavigateType.MusicBoard, typeof(FM_SongBoardPage)}
                };
                return pagesMaps;
            }).Invoke(); }
        }

        #endregion

        #region Frame

        public static Frame GetFrameInstance(FrameType type) { return FramesMaps.ContainsKey(type) ? FramesMaps[type] : null; }
        static private IDictionary<FrameType, Frame> framesMaps;
        public static IDictionary<FrameType, Frame> FramesMaps {
            get {
                return framesMaps ?? new Func<IDictionary<FrameType, Frame>>(() => {
                    framesMaps = new Dictionary<FrameType, Frame> {
                        { FrameType.Content, MainContentFrame },
                        { FrameType.Login, MainLoginFrame },
                        { FrameType.Metro, MainMetroFrame },
                        { FrameType.UpContent, MainUpContentFrame },
                        { FrameType.UserInfos, MainUserInfosFrame },
                        { FrameType.InfosDeatils, UserInfoDetails },
                        { FrameType.LeftPart, MainLeftPartFrame },
                    };
                    return framesMaps;
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
                            IdentityToken = "DB_INDEX",
                            Title = GetUIString("DB_INDEX"),
                            PathUri = new Uri("https://m.douban.com/"),
                            NaviType = NavigateType.Index,
                        },
                        new NavigationBar {
                            IdentityToken = "DB_BOOK",
                            Title = GetUIString("DB_BOOK"),
                            PathUri = new Uri("https://m.douban.com/book/"),
                            NaviType = NavigateType.Book,
                        },
                        new NavigationBar {
                            IdentityToken = "DB_MORE",
                            Title = GetUIString("DB_MORE"),
                            PathUri = new Uri("https://www.douban.com/"),
                            NaviType = NavigateType.TV
                        },
                        new NavigationBar {
                            IdentityToken = "DB_MOVIE",
                            Title = GetUIString("DB_MOVIE"),
                            PathUri = new Uri("https://m.douban.com/movie/"),
                            NaviType = NavigateType.Movie
                        },
                        new NavigationBar {
                            IdentityToken = "DB_MUSIC",
                            Title = GetUIString("DB_MUSIC"),
                            PathUri = new Uri("https://m.douban.com/music/"),
                            NaviType = NavigateType.Music
                            },
                        new NavigationBar {
                            IdentityToken = "DB_FM",
                            Title = GetUIString("DB_FM"),
                            PathUri = new Uri("https://douban.fm/?from_=shire_top_nav"),
                            NaviType = NavigateType.FM,
                        },
                        new NavigationBar {
                            IdentityToken = "DB_GROUP",
                            Title = GetUIString("DB_GROUP"),
                            PathUri = new Uri("https://www.douban.com/group/"),
                            NaviType = NavigateType.Webview
                        },
                        new NavigationBar {
                            IdentityToken = "DB_READ",
                            Title = GetUIString("DB_READ"),
                            PathUri = new Uri("https://read.douban.com/"),
                            NaviType = NavigateType.Webview
                        },
                         new NavigationBar {
                            IdentityToken = "DB_LOCATION",
                            Title = GetUIString("DB_LOCATION"),
                            PathUri = new Uri("https://www.douban.com/location/"),
                            NaviType = NavigateType.Webview
                        },
                        new NavigationBar {
                            IdentityToken = "DB_DONGXI",
                            Title = GetUIString("DB_DONGXI"),
                            PathUri = new Uri("https://dongxi.douban.com/?dcs=top-nav&amp;dcm=douban"),
                            NaviType = NavigateType.Webview
                        },
                        new NavigationBar {
                            IdentityToken = "DB_MARKET",
                            Title = GetUIString("DB_MARKET"),
                            PathUri = new Uri("https://market.douban.com/?utm_campaign=douban_top_nav&amp;utm_source=douban&amp;utm_medium=pc_web"),
                            NaviType = NavigateType.Webview
                        },
                    };
                    return navigationListMap;
                }).Invoke();
            }
        }
        #endregion

        #endregion

    }
}
