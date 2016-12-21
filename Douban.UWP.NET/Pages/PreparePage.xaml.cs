using Wallace.UWP.Helpers;
using Wallace.UWP.Helpers.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Wallace.UWP.Helpers.Controls;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.Profile;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Douban.UWP.NET.Pages {

    public sealed partial class PreparePage : UpSlidePage {
        private TranslateTransform translateT;
        DispatcherTimer WeocomeTimer;
        private bool isColorfulOrNot;
        private bool isDarkOrNot;

        public PreparePage ( ) {
            translateT = this . RenderTransform as TranslateTransform;
            this . InitializeComponent ( );
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e) {
            isColorfulOrNot = (bool?)SettingsHelper.ReadSettingsValue(SettingsConstants.IsColorfulOrNot) ?? false;
            isDarkOrNot= (bool?)SettingsHelper.ReadSettingsValue(SettingsConstants.IsDarkThemeOrNot) ?? false;
            if (StatusBarInit.HaveAddMobileExtensions()) { StatusBarInit.InitInnerMobileStatusBar(true); }
            StatusBarInit.InitDesktopStatusBar(!isDarkOrNot, Colors.Black, Color.FromArgb(255, 67, 104, 203), Colors.White, Color.FromArgb(255, 202, 0, 62));
            StatusBarInit.InitMobileStatusBar(!isDarkOrNot, Colors.Black, Color.FromArgb(255, 67, 104, 203), Colors.White, Color.FromArgb(255, 202, 0, 62));
            InitSliderTimer();
            OutIMG.BeginTime = new TimeSpan(0, 0, 0, 0, 800);
            OutREC.BeginTime = new TimeSpan(0, 0, 0, 0, 800);
            OutIMG.SpeedRatio = 0.07;
            OutREC.SpeedRatio = 0.07;
            OutIMG.Begin();
            OutREC.Begin();
        }

        private void InitSliderTimer ( ) {
            WeocomeTimer = new DispatcherTimer ( );
            WeocomeTimer . Tick += dispatcherTimer_TickSliderbarMP;
            WeocomeTimer . Interval = new TimeSpan ( 0 , 0 , 0 , 0 , 100 );
            WeocomeTimer . Start ( );
        }
        
        void dispatcherTimer_TickSliderbarMP ( object sender , object e ) {
            progressBar . Value += 120; 
            if ( progressBar.Value - progressBar.Maximum >-0.00001 ) {
                WeocomeTimer . Stop ( );
                SetAnimation ( );
            }
        }

        private void SetAnimation ( ) {
            var storyboard = new Storyboard ( );
            var doubleanimation = new DoubleAnimation ( ) { Duration = new Duration ( TimeSpan . FromMilliseconds ( 520 ) ) , From = translateT . Y , To = -this.ActualHeight };
            doubleanimation . EasingFunction = new CubicEase ( ) { EasingMode = EasingMode . EaseOut };
            doubleanimation . Completed += Doubleanimation_Completed;
            Storyboard . SetTarget ( doubleanimation , translateT );
            Storyboard . SetTargetProperty ( doubleanimation , "Y" );
            storyboard . Children . Add ( doubleanimation );
            storyboard . Begin ( );
        }

        private void Doubleanimation_Completed ( object sender , object e ) {
            this . Frame . Content = null;
            if ( translateT == null )
                this . RenderTransform = new TranslateTransform ( );
            translateT . Y = 0;
            RequestedTheme = isDarkOrNot ? ElementTheme.Dark : ElementTheme.Light;
            //MainPage.Current.ChangeStatusBar(isColorfulOrNot, isLightOrNot);
        }
    }
}
