using System;
using System . Collections . Generic;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using Windows . UI;
using Windows . UI . ViewManagement;
using Windows . Foundation;
using Windows . Foundation . Collections;
using Windows . System . Profile;
using Windows . UI . Core;
using Windows . UI . Xaml;
using Windows . UI . Xaml . Controls;
using Windows . UI . Xaml . Controls . Primitives;
using Windows . UI . Xaml . Data;
using Windows . UI . Xaml . Input;
using Windows . UI . Xaml . Media;
using Windows . UI . Xaml . Navigation;

namespace Wallace.UWP.Helpers. Tools {
    /// <summary>
    /// 设置Mobile和Desktop的状态栏风格
    /// </summary>
    public static class StatusBarInit {
        /// <summary>
        /// 判断当前为Mobile增加拓展
        /// </summary>
        /// <returns></returns>
        public static bool HaveAddMobileExtensions() { return Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"); }

        public static bool IsMobile => Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons") ? true : false;

        /// <summary>
        /// 为准备画面初始化Desktop任务栏
        /// </summary>
        public static void InitDesktopStatusBar (bool IsLightTheme) {
            if (!IsLightTheme) {
                SetTitleBarSelfView(0, 32, 32, 32, Colors.White, Colors.LightGray, Colors.Gray);
                SetTitleBarButtonSelfView(0, 32, 32, 32, Colors.White);
                SetTitleBarButtonHPIView(Colors.SteelBlue, Colors.White, Colors.SteelBlue, Colors.White, Colors.DarkGray, Colors.Gray);
            } else {
                SetTitleBarSelfView(0, 246, 246, 246, Colors.Black, Colors.LightGray, Colors.Gray);
                SetTitleBarButtonSelfView(0, 246, 246, 246, Colors.Black);
                SetTitleBarButtonHPIView(Colors.SteelBlue, Colors.Black, Colors.SteelBlue, Colors.White, Colors.DarkGray, Colors.Gray);
            }
        }

        /// <summary>
        /// 为准备画面初始化Desktop任务栏
        /// </summary>
        public static void InitDesktopStatusBar(bool IsLightTheme, Color ForeColorLight, Color BackColorLight, Color ForeColorDark, Color BackColorDark) {
            if (IsLightTheme) {
                SetTitleBarSelfView(BackColorLight, ForeColorLight, Colors.LightGray, Colors.Gray);
                SetTitleBarButtonSelfView(Colors.Transparent, ForeColorLight);
                SetTitleBarButtonHPIView(Color.FromArgb(255, 254, 154, 4), Colors.White, Color.FromArgb(255, 254, 154, 4), Colors.White, Colors.Transparent, Colors.Gray);
            } else {
                SetTitleBarSelfView(BackColorDark, ForeColorDark, Colors.LightGray, Colors.Gray);
                SetTitleBarButtonSelfView(Colors.Transparent, ForeColorDark);
                SetTitleBarButtonHPIView(Color.FromArgb(255, 254, 154, 4), Colors.Black, Color.FromArgb(255, 254, 154, 4), Colors.White, Colors.Transparent, Colors.Gray);
            }
        }

        /// <summary>
        /// 初始化Mobile任务栏
        /// </summary>
        /// <param name="IsLightTheme">是否为Light主题</param>
        /// <param name="ForeColorLight"></param>
        /// <param name="BackColorLight"></param>
        /// <param name="ForeColorDark"></param>
        /// <param name="BackColorDark"></param>
        public static void InitMobileStatusBar (bool IsLightTheme, Color ForeColorLight, Color BackColorLight, Color ForeColorDark, Color BackColorDark) {
            if (!Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                return;
            StatusBar statusBar = StatusBar.GetForCurrentView();
            statusBar.BackgroundColor = !IsLightTheme ? BackColorDark : BackColorLight;
            statusBar.ForegroundColor = !IsLightTheme ? ForeColorDark : ForeColorLight;
        }

        /// <summary>
        /// 初始化沉浸式Desktop任务栏
        /// </summary>
        public static void InitInnerDesktopStatusBar(bool NeedToInner) {
            // core Titlebar settings
            var coreTitleBar = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().TitleBar;
            if (NeedToInner){
                coreTitleBar.ExtendViewIntoTitleBar = true;
                return;
            }
            coreTitleBar.ExtendViewIntoTitleBar = false;
        }

        /// <summary>
        /// 初始化沉浸式Mobile任务栏
        /// </summary>
        public static void InitInnerMobileStatusBar(bool NeedToInner) {
            if (!Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                return;
            StatusBar statusBar = StatusBar.GetForCurrentView();
            if (NeedToInner) {
                ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
                statusBar.BackgroundOpacity = 0;
                return;
            }
            ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseVisible);
            statusBar.BackgroundOpacity = 1;
        }

        /// <summary>
        ///  设置Desktop任务栏按钮特殊风格 ///
        ///  1）指针上方时的背景 2）前景
        ///  3）按钮按下的背景 4）前景
        ///  5）失去焦点时的背景 6）前景
        /// </summary>
        /// <param name="AppView">应用程序视图</param>
        /// <param name="HoverBack">指针上方时的背景</param>
        /// <param name="HoverFore">指针上方时的前景</param>
        /// <param name="PrsdBack">按钮按下的背景</param>
        /// <param name="PrsdFore">按钮按下的前景</param>
        /// <param name="InacBack">失去焦点时的背景</param>
        /// <param name="InacFore">失去焦点时的前景</param>
        public static void SetTitleBarButtonHPIView ( Color HoverBack , Color HoverFore , Color PrsdBack , Color PrsdFore , Color InacBack , Color InacFore ) {
            var AppView = ApplicationView . GetForCurrentView ( );
            AppView . TitleBar . ButtonHoverBackgroundColor = HoverBack;
            AppView . TitleBar . ButtonHoverForegroundColor = HoverFore;
            AppView . TitleBar . ButtonPressedBackgroundColor = PrsdBack;
            AppView . TitleBar . ButtonPressedForegroundColor = PrsdFore;
            AppView . TitleBar . ButtonInactiveBackgroundColor = InacBack;
            AppView . TitleBar . ButtonInactiveForegroundColor = InacFore;
        }

        /// <summary>
        /// 设置Desktop任务栏按钮初始风格 ///
        /// 分别设置ARGB和前景色
        /// </summary>
        /// <param name="AppView">应用试视图</param>
        /// <param name="alpha">透明度</param>
        /// <param name="R">红</param>
        /// <param name="G">绿</param>
        /// <param name="B">蓝</param>
        /// <param name="ForeColor">前景色</param>
        public static void SetTitleBarButtonSelfView ( byte alpha , byte R , byte G , byte B , Color ForeColor ) {
            var AppView = ApplicationView . GetForCurrentView ( );
            AppView . TitleBar . ButtonBackgroundColor = Color . FromArgb ( alpha , R , G , B );
            AppView . TitleBar . ButtonForegroundColor = ForeColor;
        }

        /// <summary>
        /// 设置Desktop任务栏按钮初始风格 ///
        /// 分别设置背景色和前景色
        /// </summary>
        /// <param name="ForeColor">背景色</param>
        /// <param name="ForeColor">前景色</param>
        public static void SetTitleBarButtonSelfView(Color BackColor, Color ForeColor) {
            var AppView = ApplicationView.GetForCurrentView();
            AppView.TitleBar.ButtonBackgroundColor = BackColor;
            AppView.TitleBar.ButtonForegroundColor = ForeColor;
        }

        /// <summary>
        /// 设置Desktop任务栏初始风格
        /// 1）分别设置ARGB和前景色
        /// 2）设置失去焦点的背景和前景
        /// </summary>
        /// <param name="AppView"></param>
        /// <param name="alpha"></param>
        /// <param name="R"></param>
        /// <param name="G"></param>
        /// <param name="B"></param>
        /// <param name="ForeColor"></param>
        /// <param name="InacBack"></param>
        /// <param name="InacFore"></param>
        public static void SetTitleBarSelfView ( byte alpha , byte R , byte G , byte B , Color ForeColor , Color InacBack , Color InacFore ) {
            var AppView = ApplicationView . GetForCurrentView ( );
            AppView . TitleBar . BackgroundColor = Color . FromArgb ( alpha , R , G , B );
            AppView . TitleBar . ForegroundColor = ForeColor;
            AppView . TitleBar . InactiveBackgroundColor = InacBack;
            AppView . TitleBar . InactiveForegroundColor = InacFore;
        }

        /// <summary>
        /// 设置Desktop任务栏初始风格
        /// 1）分别设置背景色和前景色
        /// 2）设置失去焦点的背景和前景
        /// </summary>
        /// <param name="BackColor"></param>
        /// <param name="ForeColor"></param>
        /// <param name="InacBack"></param>
        /// <param name="InacFore"></param>
        public static void SetTitleBarSelfView(Color BackColor, Color ForeColor, Color InacBack, Color InacFore) {
            var AppView = ApplicationView.GetForCurrentView();
            AppView.TitleBar.BackgroundColor = BackColor;
            AppView.TitleBar.ForegroundColor = ForeColor;
            AppView.TitleBar.InactiveBackgroundColor = InacBack;
            AppView.TitleBar.InactiveForegroundColor = InacFore;
        }
    }
}
