using Wallace.UWP.Helpers.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Wallace.UWP.Helpers.Controls {
    public partial class ToastSmoothBase : UserControl {
        protected Popup DialogPopup;

        protected string TextContent;
        protected TimeSpan WholeTime;

        public static readonly DependencyProperty ToastBackgroundProperty = DependencyProperty.Register("ToastBackground", typeof(Brush), typeof(ToastSmoothBase), null);
        public Brush ToastBackground {
            get { return GetValue(ToastBackgroundProperty) as Brush; }
            set { SetValue(ToastBackgroundProperty, value); }
        }

        public static readonly DependencyProperty ToastForegroundProperty = DependencyProperty.Register("ToastForeground", typeof(Brush), typeof(ToastSmoothBase), null);
        public Brush ToastForeground {
            get { return GetValue(ToastForegroundProperty) as Brush; }
            set { SetValue(ToastForegroundProperty, value); }
        }

        public ToastSmoothBase ( ) {
            this . InitializeComponent ( );
            DialogPopup = new Popup ( );
            this . Width = Window . Current . Bounds .Width;
            this . Height = Window . Current . Bounds . Height;
            DialogPopup . Child = this;
            this . Loaded += NotifyPopup_Loaded;
            this . Unloaded += NotifyPopup_Unloaded;
        }

        public ToastSmoothBase ( string content , TimeSpan showTime ) : this() {
            this . TextContent = content;
            this . WholeTime = showTime;
        }

        public ToastSmoothBase ( string content ) : this(content, TimeSpan.FromSeconds(2)) { }

        public void Show ( ) {
            this . DialogPopup . IsOpen = true;
        }

        private void NotifyPopup_Loaded ( object sender , RoutedEventArgs e ) {
            this . tbNotify . Text = TextContent;
            this . In . Begin ( );
            this . In . Completed += SbIn_Completed;
            Window . Current . SizeChanged += Current_SizeChanged;
        }

        private void SbIn_Completed ( object sender , object e ) {
            this . Out . BeginTime = this . WholeTime;
            this . Out . Completed += SbOut_Completed;
            this . Out . Begin ( );
        }

        private void SbOut_Completed ( object sender , object e ) {
            this . DialogPopup . IsOpen = false;
        }

        private void Current_SizeChanged ( object sender , Windows . UI . Core . WindowSizeChangedEventArgs e ) {
            this . Width = e . Size . Width;
            this . Height = e . Size . Height;
        }

        private void NotifyPopup_Unloaded ( object sender , RoutedEventArgs e ) {
            Window . Current . SizeChanged -= Current_SizeChanged;
        }
    }
}
