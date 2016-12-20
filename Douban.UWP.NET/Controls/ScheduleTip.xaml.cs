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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Douban.UWP.NET.Controls {
    public sealed partial class ScheduleTip : UserControl {
        public ScheduleTip() {
            this.InitializeComponent();
            this.DataContext = this; // important !!!!
            InnerButton = this.ClickButton;
        }

        public static readonly DependencyProperty TipTitleProperty = DependencyProperty.Register("TipTitle", typeof(string), typeof(ScheduleTip), null);
        public string TipTitle {
            get { return GetValue(TipTitleProperty) as string; }
            set { SetValue(TipTitleProperty, value); }
        }

        public static readonly DependencyProperty TipLectureProperty = DependencyProperty.Register("TipLecture", typeof(string), typeof(ScheduleTip), null);
        public string TipLecture {
            get { return GetValue(TipLectureProperty) as string; }
            set { SetValue(TipLectureProperty, value); }
        }

        public static readonly DependencyProperty TipPlaceProperty = DependencyProperty.Register("TipPlace", typeof(string), typeof(ScheduleTip), null);
        public string TipPlace {
            get { return GetValue(TipPlaceProperty) as string; }
            set { SetValue(TipPlaceProperty, value); }
        }

        public static readonly DependencyProperty TipFrontProperty = DependencyProperty.Register("TipFront", typeof(string), typeof(ScheduleTip), null);
        public string TipFront {
            get { return GetValue(TipFrontProperty) as string; }
            set { SetValue(TipFrontProperty, value); }
        }

        public static readonly DependencyProperty ClickVisibleProperty = DependencyProperty.Register("ClickVisible", typeof(Visibility), typeof(ScheduleTip), null);
        public Visibility ClickVisible {
            get { return (Visibility)GetValue(ClickVisibleProperty); }
            set { SetValue(ClickVisibleProperty, value); }
        }

        public Button InnerButton;

    }
}
