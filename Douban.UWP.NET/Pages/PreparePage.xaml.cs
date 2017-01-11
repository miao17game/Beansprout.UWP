using static Wallace.UWP.Helpers.Tools.UWPStates;
using static Douban.UWP.NET.Resources.AppResources;

using Wallace.UWP.Helpers.Tools;
using System;
using System.IO;
using System.Linq;
using Wallace.UWP.Helpers.Controls;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Douban.UWP.NET.Pages {

    public sealed partial class PreparePage : UpSlidePage {
        private TranslateTransform translateT;
        DispatcherTimer WeocomeTimer;
        private bool isDarkOrNot;

        public PreparePage ( ) {
            translateT = this . RenderTransform as TranslateTransform;
            this . InitializeComponent ( );
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e) {
            InitSliderTimer();
            OutIMG.BeginTime = new TimeSpan(0, 0, 0, 0, 800);
            OutREC.BeginTime = new TimeSpan(0, 0, 0, 0, 800);
            OutIMG.SpeedRatio = 0.07;
            OutREC.SpeedRatio = 0.07;
            OutIMG.Begin();
            OutREC.Begin();
            isDarkOrNot= IsGlobalDark;
            if (StatusBarInit.HaveAddMobileExtensions()) { StatusBarInit.InitInnerMobileStatusBar(true); }
            StatusBarInit.InitDesktopStatusBar(false, Colors.Black, Color.FromArgb(255, 67, 104, 203), Colors.White, Color.FromArgb(255, 202, 0, 62));
            StatusBarInit.InitMobileStatusBar(false, Colors.Black, Color.FromArgb(255, 67, 104, 203), Colors.White, Color.FromArgb(255, 202, 0, 62));
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

        private void SetAnimation() {
            var storyboard = new Storyboard();
            var doubleanimation = new DoubleAnimation {
                Duration = new Duration(TimeSpan.FromMilliseconds(520)),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
                From = translateT.Y, To = -this.ActualHeight
            };
            doubleanimation.Completed += Doubleanimation_Completed;
            Storyboard.SetTarget(doubleanimation, translateT);
            Storyboard.SetTargetProperty(doubleanimation, "Y");
            storyboard.Children.Add(doubleanimation);
            storyboard.Begin();
        }

        private void Doubleanimation_Completed ( object sender , object e ) {
            this . Frame . Content = null;
            if ( translateT == null )
                this . RenderTransform = new TranslateTransform ( );
            translateT . Y = 0;
            RequestedTheme = isDarkOrNot ? ElementTheme.Dark : ElementTheme.Light;
        }
    }
}
