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

namespace Douban.UWP.NET.Controls {
    public sealed partial class FiveStarControl : UserControl {
        public FiveStarControl() {
            this.InitializeComponent();
            this.Main.DataContext = this;
        }

        public static readonly DependencyProperty RatingValueProperty = DependencyProperty.Register("RatingValue", typeof(double), typeof(FiveStarControl), null);
        public double RatingValue {
            get { return (double)GetValue(RatingValueProperty); }
            set { SetValue(RatingValueProperty, value); }
        }

        public static readonly DependencyProperty ShowValueVisibilityProperty = DependencyProperty.Register("ShowValueVisibility", typeof(double), typeof(FiveStarControl), PropertyMetadata.Create(Visibility.Visible));
        public Visibility ShowValueVisibility {
            get { return (Visibility)GetValue(ShowValueVisibilityProperty); }
            set { SetValue(ShowValueVisibilityProperty, value); }
        }

    }
}
