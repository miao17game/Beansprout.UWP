using Douban.UWP.Core.Models;
using Douban.UWP.Core.Tools;
using Douban.UWP.NET.Controls;
using Douban.UWP.NET.Resources;
using Douban.UWP.NET.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Wallace.UWP.Helpers;
using Wallace.UWP.Helpers.Tools;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Douban.UWP.NET {

    sealed partial class App : Application {

        public string FromToastArgument;

        public App() {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.UnhandledException += OnUnhandledException;
        }

        private void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e) {
            e.Handled = true;
            ReportHelper.ReportAttentionAsync(UWPStates.GetUIString("Error") + ": \n" + e.Exception.Message);
        }

        /// <summary>
        /// Should be called from OnActivated and OnLaunched
        /// </summary>
        private void RegisterExceptionHandlingSynchronizationContext() {
            ExceptionHandlingSynchronizationContext
                .Register()
                .UnhandledException += SynchronizationContext_UnhandledException;
        }

        private void SynchronizationContext_UnhandledException(object sender, Wallace.UWP.Helpers.Tools.UnhandledExceptionEventArgs e) {
            e.Handled = true;
            ReportHelper.ReportAttentionAsync(UWPStates.GetUIString("Error")  + ": \n" + e.Exception.Message);
        }

        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 将在启动应用程序以打开特定文件等情况下使用。
        /// </summary>
        /// <param name="e">有关启动请求和过程的详细信息。</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e) {

            InitAppStateWhenFirstDeployment();
            RegisterExceptionHandlingSynchronizationContext();

            Frame rootFrame = Window.Current.Content as Frame;

            // 不要在窗口已包含内容时重复应用程序初始化，
            // 只需确保窗口处于活动状态
            if (rootFrame == null) {
                // 创建要充当导航上下文的框架，并导航到第一页
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated) {
                    //TODO: 从之前挂起的应用程序加载状态
                }

                // 将框架放在当前窗口中
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false) {
                if (rootFrame.Content == null) {
                    // 当导航堆栈尚未还原时，导航到第一页，
                    // 并通过将所需信息作为导航参数传入来配置
                    // 参数
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // 确保当前窗口处于活动状态
                Window.Current.Activate();
            }

            try {
                await TilesHelper.GetNewsAsync(ignoreTime: true);
            } catch { /* Ignore */ }

        }

        private static void InitAppStateWhenFirstDeployment() {
            if ((bool?)SettingsHelper.ReadSettingsValue(SettingsSelect.IsFirstLoadApp) ?? true) {
                ApplicationLanguages.PrimaryLanguageOverride =
                    (string)SettingsHelper.ReadSettingsValue(SettingsSelect.Language) ??
                    ConstFields.Chinese_CN;
                SettingsHelper.SaveSettingsValue(SettingsSelect.Language, ConstFields.Chinese_CN);
                SettingsHelper.SaveSettingsValue(SettingsSelect.IsFirstLoadApp, false);
            }
        }

        protected override void OnActivated(IActivatedEventArgs args) {
            RegisterExceptionHandlingSynchronizationContext();
            if (args.Kind == ActivationKind.ToastNotification) {
                var toastArgs = args as ToastNotificationActivatedEventArgs;
                FromToastArgument = toastArgs.Argument;
                Frame root = Window.Current.Content as Frame;
                if (root == null) {
                    root = new Frame();
                    Window.Current.Content = root;
                }
                if (root.Content == null) {
                    root.Navigate(typeof(MainPage), toastArgs);
                } else {
                    try {
                        var decode = JsonHelper.FromJson<ToastParameters>(FromToastArgument);
                        if (decode != null) {
                            AppResources.NavigateToBase?.Invoke(
                                null,
                                new NavigateParameter {
                                    ToUri = new Uri(decode.Uri),
                                    Title = decode.Title,
                                    IsFromInfoClick = true,
                                    IsNative = true,
                                    FrameType = FrameType.Content
                                },
                                AppResources.GetFrameInstance(FrameType.Content),
                                AppResources.GetPageType(NavigateType.ItemClickNative));
                        }
                    } catch { /* I do not want my app to be shut down. */}
                }
                Window.Current.Activate();
            }
        }

            void OnNavigationFailed(object sender, NavigationFailedEventArgs e) {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// 在将要挂起应用程序执行时调用。  在不知道应用程序
        /// 无需知道应用程序会被终止还是会恢复，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起请求的详细信息。</param>
        private void OnSuspending(object sender, SuspendingEventArgs e) {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: 保存应用程序状态并停止任何后台活动
            deferral.Complete();
        }
    }
}
