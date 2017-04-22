using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.System.Profile;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Wallace.UWP.Helpers.Tools {
    /// <summary>
    /// UWP states Helper
    /// </summary>
    public static class UWPStates {

        #region Properties and state
        private static ResourceLoader resLoader = new ResourceLoader();
        #endregion

        public static SystemNavigationManager NavigateManager { get { return SystemNavigationManager.GetForCurrentView(); } }
        public static ApplicationView AppView { get { return ApplicationView.GetForCurrentView(); } }
        public static ResourceLoader ResLoader { get { return resLoader ?? new ResourceLoader(); } }

        /// <summary>
        /// Current window height with NavigationBar and StatusBar
        /// </summary>
        public static double WindowWidth { get { return Window.Current.Bounds.Width; } }
        /// <summary>
        /// Current window width with NavigationBar and StatusBar
        /// </summary>
        public static double WindowHeight { get { return Window.Current.Bounds.Height; } }
        /// <summary>
        /// Current window height without NavigationBar and StatusBar
        /// </summary>
        public static double VisibleHeight { get { return ApplicationView.GetForCurrentView().VisibleBounds.Height; } }
        /// <summary>
        /// Current window width without NavigationBar and StatusBar
        /// </summary>
        public static double VisibleWidth { get { return ApplicationView.GetForCurrentView().VisibleBounds.Width; } }

        public static bool IsMobile { get { return AnalyticsInfo.VersionInfo.DeviceFamily.Equals("Windows.Mobile"); } }

        public static void SetVisibility(this FrameworkElement element, bool IsVisible) { element.Visibility = IsVisible? Visibility.Visible : Visibility.Collapsed; }

        public static string GetUIString(string id) { return ResLoader.GetString(id); }

        public static ulong SDKVersion { get { return (ulong.Parse(AnalyticsInfo.VersionInfo.DeviceFamilyVersion) & 0x00000000FFFF0000L) >> 16; } }

        public static string GetSystemVersion { get { return AnalyticsInfo.VersionInfo.DeviceFamilyVersion; } }

        public static string GetSystemOSBuild() {
            string sv = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            ulong v = ulong.Parse(sv);
            ulong v3 = (v & 0x00000000FFFF0000L) >> 16;
            ulong v4 = (v & 0x000000000000FFFFL);
            return $"{v3}.{v4}";
        }

        /// <summary>
        /// Adaptive the screen when you app running on a mobile device with vitual navigation bar.
        /// </summary>
        /// <param name="page"></param>
        public static void AdapteVitualNavigationBarWithoutStatusBar(Page page) {
            page.Width = VisibleWidth;
            var wholeHeight = WindowHeight;
            var wholeWidth = WindowWidth;
            if (wholeHeight < wholeWidth) {
                page.Height = VisibleHeight;
                page.Width = VisibleWidth + 48;
                page.Margin =
                    page.Width - wholeWidth > -0.1 ?
                    new Thickness(0, 0, 0, 0) :
                    new Thickness(-24, 0, 0, 0);
            } else {
                page.Height = VisibleHeight + 24;
                page.Margin =
                    page.Height - wholeHeight > -0.1 ?
                    new Thickness(0, 0, 0, 0) :
                    new Thickness(0, -48, 0, 0);
            }
        }

    }
}
